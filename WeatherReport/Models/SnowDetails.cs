using Newtonsoft.Json;

namespace WeatherReport.Models
{
    public class SnowDetails
    {
        [JsonProperty("1h")]
        public double? OneHour { get; set; }

        [JsonProperty("3h")]
        public double? ThreeHours { get; set; }
    }
}
