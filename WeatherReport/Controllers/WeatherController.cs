using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WeatherReport.Data;
using WeatherReport.Models;
using WeatherReport.Services;

namespace WeatherReport.Controllers
{
    public class WeatherController : Controller
    {
        private readonly OpenWeatherApiService _weatherApiService;
        private readonly WeatherPredictionService _weatherForecastService;
        private readonly IHttpClientFactory _httpClientFactory;

        public WeatherController(OpenWeatherApiService weatherApiService, WeatherPredictionService weatherForecastService, IHttpClientFactory httpClientFactory)
        {
            _weatherApiService = weatherApiService;
            _weatherForecastService = weatherForecastService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("next-day-forecast")]
        public async Task<IActionResult> GetNextDayForecastByCityName([FromQuery] string cityName, [FromServices] IOptions<WeatherApiSettings> weatherApiSettings)
        {
             var apiKey = weatherApiSettings.Value.ApiKey;

             var geoCodingApiUrl = $"https://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=1&appid={apiKey}";
             var geoCodingResponse = await _weatherApiService.GetGeocodingDataAsync(geoCodingApiUrl);
             var location = geoCodingResponse?.FirstOrDefault();

             if (location == null)
             {
                 return NotFound($"City '{cityName}' not found");
             }

             var historicalDataWeek = await _weatherApiService.GetHistoricalDataAsync(location.Lat, location.Lon, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, apiKey);
             var historicalDataSecondWeek = await _weatherApiService.GetHistoricalDataAsync(location.Lat, location.Lon, DateTime.UtcNow.AddDays(-14), DateTime.UtcNow.AddDays(-7), apiKey);
             var historicalDataThirdWeek = await _weatherApiService.GetHistoricalDataAsync(location.Lat, location.Lon, DateTime.UtcNow.AddDays(-21), DateTime.UtcNow.AddDays(-14), apiKey);
             var historicalData = historicalDataWeek
                 .Concat(historicalDataSecondWeek)
                 .Concat(historicalDataThirdWeek)
                 .ToList();

            if (historicalData == null || !historicalData.Any())
             {
                 return NotFound($"No historical weather data found for '{cityName}'");
             }

             var weekForecast = _weatherForecastService.PredictWeatherNextDay(historicalDataWeek);

             return Ok(weekForecast);
        }

        [HttpGet("week-temperature-forecast")]
        public async Task<IActionResult> GetForecastOn14DaysByCityName([FromQuery] string cityName,  [FromServices] IOptions<WeatherApiSettings> weatherApiSettings)
        {
            var apiKey = weatherApiSettings.Value.ApiKey;

            var geoCodingApiUrl = $"https://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=1&appid={apiKey}";
            var geoCodingResponse = await _weatherApiService.GetGeocodingDataAsync(geoCodingApiUrl);
            var location = geoCodingResponse?.FirstOrDefault();

            if (location == null)
            {
                return NotFound($"City '{cityName}' not found");
            }

            var historicalDataWeek = await _weatherApiService.GetHistoricalDataAsync(location.Lat, location.Lon, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, apiKey);
            var historicalDataSecondWeek = await _weatherApiService.GetHistoricalDataAsync(location.Lat, location.Lon, DateTime.UtcNow.AddDays(-14), DateTime.UtcNow.AddDays(-7), apiKey);
            var historicalDataThirdWeek = await _weatherApiService.GetHistoricalDataAsync(location.Lat, location.Lon, DateTime.UtcNow.AddDays(-21), DateTime.UtcNow.AddDays(-14), apiKey);
            var historicalData = historicalDataWeek.Concat(historicalDataSecondWeek)
                .Concat(historicalDataThirdWeek)
                .ToList();

            if (historicalData == null || !historicalData.Any())
            {
                return NotFound($"No historical weather data found for '{cityName}'");
            }

            var weekTemperatureForecast = _weatherForecastService.GetWeekPrediction(historicalDataWeek);

            return Ok(weekTemperatureForecast);
        }
    }
}
