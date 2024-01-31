using Newtonsoft.Json;

namespace WeatherReport.Models.ApiResponse
{
    public class RainDetails
    {
        [JsonProperty("1h")]
        public double? OneHour { get; set; }

        [JsonProperty("3h")]
        public double? ThreeHours { get; set; }
    }
}
