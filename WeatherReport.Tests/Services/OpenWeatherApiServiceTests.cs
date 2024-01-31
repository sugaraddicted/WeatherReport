using Moq;
using System.Net;
using Moq.Protected;
using WeatherReport.Services;
using WeatherReport.Models.ApiResponse;

namespace WeatherReport.Tests.Services
{
    [TestFixture]
    public class OpenWeatherApiServiceTests
    {
        private OpenWeatherApiService _openWeatherApiService;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _openWeatherApiService = new OpenWeatherApiService(_httpClient);
        }

        [Test]
        public async Task GetHistoricalDataAsync_SuccessfulRequest_ReturnsWeatherDataList()
        {
            // Arrange
            var lat = 123.45;
            var lon = 67.89;
            var start = DateTime.UtcNow.AddHours(-24);
            var end = DateTime.UtcNow;
            var apiKey = "your_api_key";

            var weatherApiData = new WeatherApiData
            {
                List = new List<WeatherDataModel>
                {
                    new WeatherDataModel {}
                }
            };

            var jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(weatherApiData);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse),
                });

            // Act
            var result = await _openWeatherApiService.GetHistoricalDataAsync(lat, lon, start, end, apiKey);

            // Assert
            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
            Assert.AreEqual(weatherApiData.List.Count, result.Count());
        }

        [Test]
        public void GetHistoricalDataAsync_HttpRequestException_ThrowsException()
        {
            // Arrange
            var lat = 123.45;
            var lon = 67.89;
            var start = DateTime.UtcNow.AddHours(-24);
            var end = DateTime.UtcNow;
            var apiKey = "api_key";


            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _openWeatherApiService.GetHistoricalDataAsync(lat, lon, start, end, apiKey));
        }
    }
}
