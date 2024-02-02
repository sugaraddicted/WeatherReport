using Microsoft.AspNetCore.Http;
using WeatherReport.Models.ApiResponse;
using WeatherReport.Processors;

namespace WeatherReport.Tests.Processors
{
    public class HistoricalDataProcessorTests
    {
        private List<WeatherDataModel> historicalData;

        [SetUp]
        public void Setup()
        {
            string[] dateStrings = { "06:22:16", "09:22:16",
            "15:22:16" };

            historicalData = new List<WeatherDataModel>
        {
            new WeatherDataModel
            {
                Timestamp = DateTime.Parse(dateStrings[0]),
                Main = new MainDetails {},
                Wind = new WindDetails {},
                Clouds = new CloudDetails {},
                Rain = new RainDetails {}
            },
            new WeatherDataModel
            {
                Timestamp = DateTime.Parse(dateStrings[1]),
                Main = new MainDetails {},
                Wind = new WindDetails {},
                Clouds = new CloudDetails {},
                Rain = new RainDetails {}
            },
            new WeatherDataModel
            {
                Timestamp = DateTime.Parse(dateStrings[2]),
                Main = new MainDetails {},
                Wind = new WindDetails {},
                Clouds = new CloudDetails {},
                Rain = new RainDetails {}
            }
        };
        }

        [Test]
        public void ConvertHistoricalDataToTrainingSet_ReturnsCorrectlyMapedData()
        {
            // Arrange
            var historicalDataProcessor = new HistoricalDataProcessor();

            // Act
            var trainingSet = historicalDataProcessor.ConvertHistoricalDataToTrainingSet(historicalData);


            // Log the training set for debugging
            Console.WriteLine($"SixPM Count: {trainingSet.SixPM.Count}");
            Console.WriteLine($"NinePM Count: {trainingSet.NinePM.Count}");
            Console.WriteLine($"TwelvePM Count: {trainingSet.TwelvePM.Count}");

            // Assert
            Assert.That(trainingSet, Is.Not.Null);
            Assert.That(trainingSet.SixAM, Is.Not.Null);
            Assert.That(trainingSet.NineAM, Is.Not.Null);
            Assert.That(trainingSet.ThreePM, Is.Not.Null);

            Assert.That(trainingSet.SixAM.Count, Is.EqualTo(1));
            Assert.That(trainingSet.NineAM.Count, Is.EqualTo(1));
            Assert.That(trainingSet.ThreePM.Count, Is.EqualTo(1));
        }
    }
}
