namespace WeatherReport.Models.Prediction
{
    public class TrainingSet
    {
        public List<TrainingDataModel> SixAM { get; set; }
        public List<TrainingDataModel> NineAM { get; set; }
        public List<TrainingDataModel> TwelvePM { get; set; }
        public List<TrainingDataModel> ThreePM { get; set; }
        public List<TrainingDataModel> SixPM { get; set; }
        public List<TrainingDataModel> NinePM { get; set; }
        public List<TrainingDataModel> TwelveAM { get; set; }
    }
}
