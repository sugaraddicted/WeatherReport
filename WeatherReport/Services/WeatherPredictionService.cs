using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using WeatherReport.Models.ApiResponse;
using WeatherReport.Models.Prediction;
using WeatherReport.Processors;

namespace WeatherReport.Services
{
    public class WeatherPredictionService
    {
        private readonly MLContext _mlContext;

        public WeatherPredictionService()
        {
            _mlContext = new MLContext();
        }

        public List<DayOfWeekPrediction> PredictWeekTemperature(IEnumerable<WeatherDataModel> historicalData)
        {
            var trainingData = new HistoricalDataProcessor().ConvertHistoricalDataToTrainingSet(historicalData);
            var dayTimeData = trainingData.NineAM.Concat(trainingData.TwelvePM)
                .Concat(trainingData.ThreePM)
                .Concat(trainingData.SixPM).ToList();
            var combinedData = dayTimeData.Concat(trainingData.NinePM).Concat(trainingData.TwelveAM)
                .Concat(trainingData.SixAM);

            var predictions = new List<DayOfWeekPrediction>();
            var tempMinValues = Predict(trainingData.TwelveAM, nameof(TrainingDataModel.Temperature), 7, 3, 7, 7).Predictions.ToList();
            var tempMaxValues = Predict(trainingData.ThreePM, nameof(TrainingDataModel.Temperature), 7, 3, 7, 7).Predictions.ToList();
            var cloudinessValues = Predict(dayTimeData, nameof(TrainingDataModel.Cloudiness), 7, 5, 14, 21).Predictions.ToList();
            var rainValues = Predict(combinedData, nameof(TrainingDataModel.RainPresence), 7, 5, 21, 21).Predictions.ToList();

            for (int i = 0; i < 7; i++)
            {
                var prediction = new DayOfWeekPrediction()
                {
                    TemperatureMin = tempMinValues[i],
                    TemperatureMax = tempMaxValues[i],
                    Cloudiness = cloudinessValues[i],
                    RainPresence = rainValues[i]
                };
                predictions.Add(prediction);
            }
            return predictions;
        }

        public NextDayPrediction PredictWeatherNextDay(IEnumerable<WeatherDataModel> historicalData)
        {
            var trainingData = new HistoricalDataProcessor().ConvertHistoricalDataToTrainingSet(historicalData);

            var prediction = new  NextDayPrediction()
            {
                SixAM = GetPredictionFromTimeSet(trainingData.SixAM),
                NineAM = GetPredictionFromTimeSet(trainingData.NineAM),
                TwelvePM = GetPredictionFromTimeSet(trainingData.TwelvePM),
                ThreePM = GetPredictionFromTimeSet(trainingData.ThreePM),
                SixPM = GetPredictionFromTimeSet(trainingData.SixPM),
                NinePM = GetPredictionFromTimeSet(trainingData.NinePM),
                TwelveAM = GetPredictionFromTimeSet(trainingData.TwelveAM)
            };

            return prediction;
        }

        public PredictionDataModel GetPredictionFromTimeSet(List<TrainingDataModel> trainingData)
        {
            var prediction = new PredictionDataModel()
            {
                Temperature = Predict(trainingData, nameof(TrainingDataModel.Temperature), 1, 3, 7, 7).Predictions[0],
                Pressure = Predict(trainingData, nameof(TrainingDataModel.Pressure), 1, 3, 10, 7).Predictions[0],
                Humidity = Predict(trainingData, nameof(TrainingDataModel.Humidity), 1, 3, 10, 7).Predictions[0],
                RainPresence = Predict(trainingData, nameof(TrainingDataModel.RainPresence),1,3,10,7).Predictions[0],
                Cloudiness = Predict(trainingData, nameof(TrainingDataModel.Cloudiness), 1, 3, 10, 7).Predictions[0],
                WindSpeed = Predict(trainingData, nameof(TrainingDataModel.WindSpeed), 1, 3, 10, 7).Predictions[0]
            };
            return prediction;
        }

        private ModelOutput Predict(IEnumerable<TrainingDataModel> trainingData, string inputOutputColumnName,int horizon, int windowSize, int seriesLength, int trainSize)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
            var pipelinePrediction = _mlContext.Forecasting.ForecastBySsa(nameof(ModelOutput.Predictions), inputOutputColumnName, windowSize, seriesLength, trainSize, horizon: 7, confidenceLevel:0.5f);
            var model = pipelinePrediction.Fit(dataView);
            var forecastContext = model.CreateTimeSeriesEngine<TrainingDataModel, ModelOutput>(_mlContext);
            var forecasts = forecastContext.Predict(horizon);

            return forecasts;
        }
    }
}
