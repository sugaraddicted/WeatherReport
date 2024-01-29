using WeatherReport.Models;
using WeatherReport.Models.Prediction;

namespace WeatherReport.Processors
{
    public class HistoricalDataProcessor
    {
        public IEnumerable<TrainingDataModel> ProcessHistoricalData(IEnumerable<WeatherDataModel> historicalData)
        {
            var trainingDataList = new List<TrainingDataModel>();

            foreach (var dataPoint in historicalData)
            {
                var trainingData = new TrainingDataModel
                {
                    Timestamp = dataPoint.Timestamp,
                    Temperature = dataPoint.Main.Temp,
                    Pressure = dataPoint.Main.Pressure,
                    Humidity = dataPoint.Main.Humidity,
                    WindSpeed = dataPoint.Wind.Speed,
                    RainPresence = dataPoint.Rain?.OneHour.HasValue == true ? 1 : 0,
                    SnowPresence = dataPoint.Snow?.OneHour.HasValue == true ? 1 : 0,
                };

                trainingDataList.Add(trainingData);
            }

            return trainingDataList;
        }
    }
}
