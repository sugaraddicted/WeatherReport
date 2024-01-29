namespace WeatherReport.Models.Prediction
{
    public class TrainingDataModel
    {
        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public double Pressure { get; set; }
        public double WindSpeed { get; set; }
        public double Humidity { get; set; }
        public double RainPresence { get; set; }
        public double SnowPresence { get; set; }
    }
}
