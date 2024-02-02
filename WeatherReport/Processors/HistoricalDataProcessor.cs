using WeatherReport.Models.ApiResponse;
using WeatherReport.Models.Prediction;

namespace WeatherReport.Processors
{
    public class HistoricalDataProcessor
    {
        public TrainingSet ConvertHistoricalDataToTrainingSet(IEnumerable<WeatherDataModel> historicalData)
        {
            var trainingSet = new TrainingSet
            {
                SixAM = new List<TrainingDataModel>(),
                NineAM = new List<TrainingDataModel>(),
                TwelvePM = new List<TrainingDataModel>(),
                ThreePM = new List<TrainingDataModel>(),
                SixPM = new List<TrainingDataModel>(),
                NinePM = new List<TrainingDataModel>(),
                TwelveAM = new List<TrainingDataModel>()
            };

            var hourToListMap = new Dictionary<int, List<TrainingDataModel>>
            {
                { 6, trainingSet.SixAM },
                { 9, trainingSet.NineAM },
                { 12, trainingSet.TwelvePM },
                { 15, trainingSet.ThreePM },
                { 18, trainingSet.SixPM },
                { 21, trainingSet.NinePM },
                { 00, trainingSet.TwelveAM }
            };

            foreach (var dataPoint in historicalData)
            {
                var trainingData = dataPoint.ToTrainingDataModel();

                if (hourToListMap.TryGetValue(trainingData.Timestamp.Hour, out var hourList))
                {
                    hourList.Add(trainingData);
                }
            }

            return trainingSet;
        }
    }
}
