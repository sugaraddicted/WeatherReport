using Microsoft.ML.Data;

namespace WeatherReport.Models.Prediction
{
    public class ModelOutput
    {
        public DateTime Timestamp { get; set; }
        public double TemperatureForecast { get; set; }
        public double PressureForecast { get; set; }
        public double WindSpeedForecast { get; set; }
        public double HumidityForecast { get; set; }
        public double RainPossibilityForecast { get; set; }
        public double SnowPossibilityForecast { get; set; }
    }
}
