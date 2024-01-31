using Newtonsoft.Json;
using WeatherReport.Models.ApiResponse;

namespace WeatherReport.Services
{
    public class OpenWeatherApiService
    {
        private readonly HttpClient _httpClient;

        public OpenWeatherApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<WeatherDataModel>> GetHistoricalDataAsync(double lat, double lon, DateTime start, DateTime end, string apiKey)
        {
            try
            {
                var apiUrl = $"https://history.openweathermap.org/data/2.5/history/city?lat={lat}&lon={lon}&type=hour&start={ToUnixTimestamp(start)}&end={ToUnixTimestamp(end)}&units=metric&appid={apiKey}";

                var response = await _httpClient.GetStringAsync(apiUrl);

                var weatherDataList = JsonConvert.DeserializeObject<WeatherApiData>(response)?.List;

                DateTime currentTimestamp = start;

                foreach (var dataPoint in weatherDataList)
                {
                    dataPoint.Timestamp = currentTimestamp;
                    currentTimestamp = currentTimestamp.AddHours(1);
                }

                return weatherDataList;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error occurred while making Historical Data API request", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception("Error occurred while deserializing Historical Data API response", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred", ex);
            }
        }

        public async Task<IEnumerable<LocationDataModel>> GetGeocodingDataAsync(string geoCodingApiUrl)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(geoCodingApiUrl);

                var locationDataList = JsonConvert.DeserializeObject<List<LocationDataModel>>(response);

                return locationDataList;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error occurred while making GeoCoding API request", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception("Error occurred while deserializing GeoCoding API response", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred", ex);
            }
        }

        private static long ToUnixTimestamp(DateTime dateTime)
        {
            return (long)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }

    public class WeatherApiData
    {
        public List<WeatherDataModel> List { get; set; }
    }
}
