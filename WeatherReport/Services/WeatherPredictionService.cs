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
            var tempMinValues = PredictTemperature(trainingData.TwelveAM, 7, 3, 7,7).Predictions.ToList();
            var tempMaxValues = PredictTemperature(trainingData.ThreePM, 7, 3, 7,7).Predictions.ToList();
            var cloudinessValues = PredictCloudiness(dayTimeData, 7, 5, 14,21).Predictions.ToList();
            var rainValues = PredictRainProbability(combinedData, 7, 5, 21,21).Predictions.ToList();

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
                Temperature = PredictTemperature(trainingData, 1, 3,7,7).Predictions[0],
                Pressure = PredictPressure(trainingData, 1, 3, 10, 7).Predictions[0],
                Humidity = PredictHumidity(trainingData, 1, 3, 10, 7).Predictions[0],
                RainPresence = PredictRainProbability(trainingData,1,3,10,7).Predictions[0],
                Cloudiness = PredictCloudiness(trainingData, 1, 3, 10,7).Predictions[0],
                WindSpeed = PredictWindSpeed(trainingData, 1, 3, 10, 7).Predictions[0]
            };
            return prediction;
        }

        private ModelOutput PredictTemperature(IEnumerable<TrainingDataModel> trainingData, int horizon, int windowSize, int seriesLength, int trainSize)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipelinePrediction = _mlContext.Forecasting.ForecastBySsa(nameof(ModelOutput.Predictions), nameof(TrainingDataModel.Temperature), windowSize, seriesLength, trainSize, horizon:8, confidenceLevel:0.75f);
            var model = pipelinePrediction.Fit(dataView);
            var forecastContext = model.CreateTimeSeriesEngine<TrainingDataModel, ModelOutput>(_mlContext);
            var forecasts = forecastContext.Predict(horizon);

            return forecasts;
        }

        private ModelOutput PredictPressure(IEnumerable<TrainingDataModel> trainingData, int horizon, int windowSize, int seriesLength, int trainSize)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipelinePrediction = _mlContext.Forecasting.ForecastBySsa(nameof(ModelOutput.Predictions), nameof(TrainingDataModel.Pressure), windowSize, seriesLength, trainSize, horizon: 8);
            var model = pipelinePrediction.Fit(dataView);
            var forecastContext = model.CreateTimeSeriesEngine<TrainingDataModel, ModelOutput>(_mlContext);
            var forecasts = forecastContext.Predict(horizon);

            return forecasts;
        }

        private ModelOutput PredictHumidity(IEnumerable<TrainingDataModel> trainingData, int horizon, int windowSize, int seriesLength, int trainSize)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipelinePrediction = _mlContext.Forecasting.ForecastBySsa(nameof(ModelOutput.Predictions), nameof(TrainingDataModel.Humidity), windowSize, seriesLength, trainSize, horizon:8);
            var model = pipelinePrediction.Fit(dataView);
            var forecastContext = model.CreateTimeSeriesEngine<TrainingDataModel, ModelOutput>(_mlContext);
            var forecasts = forecastContext.Predict(horizon);

            return forecasts;
        }

        private ModelOutput PredictWindSpeed(IEnumerable<TrainingDataModel> trainingData, int horizon, int windowSize, int seriesLength, int trainSize)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipelinePrediction = _mlContext.Forecasting.ForecastBySsa(nameof(ModelOutput.Predictions), nameof(TrainingDataModel.WindSpeed), windowSize, seriesLength, trainSize, horizon: 8);
            var model = pipelinePrediction.Fit(dataView);
            var forecastContext = model.CreateTimeSeriesEngine<TrainingDataModel, ModelOutput>(_mlContext);
            var forecasts = forecastContext.Predict(horizon);

            return forecasts;
        }

        private ModelOutput PredictCloudiness(IEnumerable<TrainingDataModel> trainingData, int horizon, int windowSize, int seriesLength, int trainSize)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipelinePrediction = _mlContext.Forecasting.ForecastBySsa(nameof(ModelOutput.Predictions), nameof(TrainingDataModel.Cloudiness), windowSize, seriesLength, trainSize, horizon: 8);
            var model = pipelinePrediction.Fit(dataView);
            var forecastContext = model.CreateTimeSeriesEngine<TrainingDataModel, ModelOutput>(_mlContext);
            var forecasts = forecastContext.Predict(horizon);

            return forecasts;
        }
        private ModelOutput PredictRainProbability(IEnumerable<TrainingDataModel> trainingData, int horizon, int windowSize, int seriesLength, int trainSize)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
            var pipelinePrediction = _mlContext.Forecasting.ForecastBySsa(nameof(ModelOutput.Predictions), nameof(TrainingDataModel.RainPresence), windowSize, seriesLength, trainSize, horizon: 7, confidenceLevel:0.5f);
            var model = pipelinePrediction.Fit(dataView);
            var forecastContext = model.CreateTimeSeriesEngine<TrainingDataModel, ModelOutput>(_mlContext);
            var forecasts = forecastContext.Predict(horizon);


            return forecasts;
        }
    }
}
