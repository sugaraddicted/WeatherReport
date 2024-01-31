using WeatherReport.Models.Prediction;

namespace WeatherReport.Models.ApiResponse
{
    public class WeatherDataModel
    {
        public DateTime Timestamp { get; set; }
        public MainDetails Main { get; set; }
        public WindDetails Wind { get; set; }
        public CloudDetails Clouds { get; set; }
        public RainDetails? Rain { get; set; }
        public SnowDetails? Snow { get; set; }

        public TrainingDataModel ToTrainingDataModel()
        {
            var trainingData = new TrainingDataModel
            {
                Timestamp = Timestamp,
                Temperature = Main.Temp,
                Pressure = Main.Pressure,
                Humidity = Main.Humidity,
                WindSpeed = Wind.Speed,
                Cloudiness = Clouds.All,
                RainPresence = Rain?.OneHour.HasValue == true ? 1 : 0,
            };
            return trainingData;
        }
    }
}

