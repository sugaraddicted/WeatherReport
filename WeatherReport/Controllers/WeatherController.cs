using Microsoft.AspNetCore.Mvc;
using WeatherReport.Services;
using WeatherReport.Processors;

namespace WeatherReport.Controllers
{
    public class WeatherController : Controller
    {
        private readonly OpenWeatherApiService _weatherApiService;
        private readonly WeatherPredictionService _weatherForecastService;

        public WeatherController(OpenWeatherApiService weatherApiService, WeatherPredictionService weatherForecastService)
        {
            _weatherApiService = weatherApiService;
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet("week_forecast")]
        public async Task<IActionResult> GetForecastOn7DaysByCityName([FromQuery] string cityName)
        {

             var apiKey = "5fbba6c4a358f17842b403ea6a540bb1";

             var geoCodingApiUrl = $"https://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=1&appid={apiKey}";
             var geoCodingResponse = await _weatherApiService.GetGeocodingDataAsync(geoCodingApiUrl);
             var location = geoCodingResponse?.FirstOrDefault();

             if (location == null)
             {
                 return NotFound($"City '{cityName}' not found");
             }

             var historicalData = await _weatherApiService.GetHistoricalDataAsync(location.Lat, location.Lon, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, apiKey);

             if (historicalData == null || !historicalData.Any())
             {
                 return NotFound($"No historical weather data found for '{cityName}'");
             }

             var weekForecast = _weatherForecastService.PredictWeather(historicalData, 7);

             return Ok(weekForecast);
        }

        [HttpGet("two_weeks_forecast")]
        public async Task<IActionResult> GetForecastOn14DaysByCityName([FromQuery] string cityName)
        {
            return Ok($"Forecast for {cityName} successfully generated");
        }
    }
}
