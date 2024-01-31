using Newtonsoft.Json;

namespace WeatherReport.Models.ApiResponse
{
    public class MainDetails
    {
        [JsonProperty("temp")]
        public float Temp { get; set; }

        [JsonProperty("feels_like")]
        public float FeelsLike { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }

        [JsonProperty("temp_min")]
        public float TempMin { get; set; }

        [JsonProperty("temp_max")]
        public float TempMax { get; set; }
    }

}
