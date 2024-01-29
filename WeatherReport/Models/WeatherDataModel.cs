namespace WeatherReport.Models
{
    public class WeatherDataModel
    {
        public DateTime Timestamp { get; set; }
        public MainDetails Main { get; set; }
        public WindDetails Wind { get; set; }
        public CloudDetails Clouds { get; set; }
        public List<WeatherDetails> Weather { get; set; }
        public RainDetails? Rain { get; set; }
        public SnowDetails? Snow { get; set; }
    }
}

