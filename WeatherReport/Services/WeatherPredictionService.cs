using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using WeatherReport.Models;
using WeatherReport.Models.Prediction;
using WeatherReport.Processors;

namespace WeatherReport.Services
{
    public class WeatherPredictionService
    {
        private readonly MLContext _mlContext;
        private ITransformer _trainedModel;

        public WeatherPredictionService()
        {
            _mlContext = new MLContext();
        }

        public IEnumerable<DayPrediction> PredictWeather(IEnumerable<WeatherDataModel> historicalData, int horizonInDays)
        {
            var trainingData = new HistoricalDataProcessor().ProcessHistoricalData(historicalData);
            TrainModel(trainingData);

            var predictions = new List<DayPrediction>();

            var lastTimestamp = historicalData.Max(data => data.Timestamp);

            for (int day = 1; day <= horizonInDays; day++)
            {
                var dayPrediction = new DayPrediction();

                var timestamps = new List<DateTime>
                {
                    lastTimestamp.AddHours(6),
                    lastTimestamp.AddHours(9),
                    lastTimestamp.AddHours(12),
                    lastTimestamp.AddHours(15),
                    lastTimestamp.AddHours(18),
                    lastTimestamp.AddHours(21),
                    lastTimestamp.AddHours(24)
                };

                foreach (var timestamp in timestamps)
                {
                    var prediction = Predict(timestamp);

                    SetPredictionBasedOnTimestamp(dayPrediction, timestamp, prediction);
                }

                predictions.Add(dayPrediction);
                lastTimestamp = timestamps.Max();
            }

            return predictions;
        }

        private void SetPredictionBasedOnTimestamp(DayPrediction dayPrediction, DateTime timestamp, ModelOutput prediction)
        {
            switch (timestamp.Hour)
            {
                case 6:
                    dayPrediction.SixAM = prediction;
                    break;
                case 9:
                    dayPrediction.NineAM = prediction;
                    break;
                case 12:
                    dayPrediction.TwelvePM = prediction;
                    break;
                case 15:
                    dayPrediction.ThreePM = prediction;
                    break;
                case 18:
                    dayPrediction.SixPM = prediction;
                    break;
                case 21:
                    dayPrediction.NinePM = prediction;
                    break;
                case 24:
                    dayPrediction.TwelveAM = prediction;
                    break;
            }
        }

        private ModelOutput Predict(DateTime timestamp)
        {
            var inputData = new TrainingDataModel
            {
                Timestamp = timestamp,
            };

            var engine = _trainedModel.CreateTimeSeriesEngine<TrainingDataModel, ModelOutput>(_mlContext);

            var prediction = engine.Predict(inputData);

            return prediction;
        }


        private void TrainModel(IEnumerable<TrainingDataModel> trainingData)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Timestamp")
                .Append(_mlContext.Transforms.Concatenate("Features",
                    "Temperature", "Pressure", "WindSpeed", "Humidity", "RainPresence", "SnowPresence"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToBinaryVector("Features"))
                .Append(_mlContext.Forecasting.ForecastBySsa("ForecastedValues", "Features", windowSize: 5,
                    seriesLength: 7,
                    trainSize: trainingData.Count(),
                    horizon: 7));

            var transformer = pipeline.Fit(dataView);

            _trainedModel = transformer;

            SaveModel();
        }

        private void SaveModel()
        {
            _mlContext.Model.Save(_trainedModel, null, "model.zip");
        }
    }
}
