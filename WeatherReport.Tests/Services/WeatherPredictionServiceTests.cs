using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherReport.Models.ApiResponse;
using WeatherReport.Models.Prediction;
using WeatherReport.Services;

namespace WeatherReport.Tests.Services
{
    [TestFixture]
    public class WeatherPredictionServiceTests
    {
        private List<WeatherDataModel> historicalData;
        private List<TrainingDataModel> trainingData;

        [SetUp]
        public void Setup()
        {
            historicalData = new List<WeatherDataModel>
            {
                new WeatherDataModel
                {
                    Timestamp = DateTime.Now.AddHours(-3),
                    Main = new MainDetails
                    {
                        Temp = 4.45f,
                        Pressure = 1014,
                        Humidity = 74,
                        TempMin = 274.26f,
                        TempMax = 276.48f
                    },
                    Wind = new WindDetails { Speed = 2.16f, },
                    Clouds = new CloudDetails { All = 90 },
                    Rain = new RainDetails { OneHour = 0.9f }
                },
                new WeatherDataModel
                {
                    Timestamp = DateTime.Now.AddHours(-2),
                    Main = new MainDetails
                    {
                        Temp = 6.45f,
                        Pressure = 1014,
                        Humidity = 74,
                        TempMin = 274.26f,
                        TempMax = 276.48f
                    },
                    Wind = new WindDetails { Speed = 2.16f, },
                    Clouds = new CloudDetails { All = 90 },
                    Rain = new RainDetails { OneHour = 0.9f }
                },
                new WeatherDataModel
                {
                    Timestamp = DateTime.Now.AddHours(-1),
                    Main = new MainDetails
                    {
                        Temp = 5.45f,
                        Pressure = 1014,
                        Humidity = 74,
                        TempMin = 274.26f,
                        TempMax = 276.48f
                    },
                    Wind = new WindDetails { Speed = 2.16f, },
                    Clouds = new CloudDetails { All = 90 },
                    Rain = new RainDetails { OneHour = 0.9f }
                }
            };

            trainingData = new List<TrainingDataModel>();
            trainingData = historicalData.Select(data => data.ToTrainingDataModel()).ToList();
        }

        [Test]
        public void PredictWeekTemperature_ReturnsValidPredictions()
        {
            // Arrange
            var weatherPredictionService = new WeatherPredictionService();

            // Act
            var predictions = weatherPredictionService.GetWeekPrediction(historicalData);

            // Assert
            Assert.IsNotNull(predictions);
            Assert.That(predictions.Count, Is.EqualTo(7));
            Assert.That(predictions[0], Is.Not.EqualTo(null));
            Assert.That(predictions[6], Is.Not.EqualTo(null));
        }

        [Test]
        public void PredictWeatherNextDay_ReturnsValidPrediction()
        {
            // Arrange
            var weatherPredictionService = new WeatherPredictionService();

            // Act
            var prediction = weatherPredictionService.PredictWeatherNextDay(historicalData);

            // Assert
            Assert.IsNotNull(prediction);
        }

        [Test]
        public void GetPredictionFromTimeSet_ReturnsValidPredictionDataModel()
        {
            // Arrange
            var weatherPredictionService = new WeatherPredictionService();

            // Act
            var prediction = weatherPredictionService.GetPredictionForTimeOfDay(trainingData);

            // Assert
            Assert.IsNotNull(prediction);
        }

        [Test]
        public void Predict_ReturnsValidModelOutput()
        {
            // Arrange
            var weatherPredictionService = new WeatherPredictionService();

            // Act
            var modelOutput = weatherPredictionService.Predict(trainingData, "Temperature", 7, 3, 7, 7);

            // Assert
            Assert.IsNotNull(modelOutput);
            Assert.That(modelOutput.Predictions.Length == 7);
        }
    }
}
