using Microsoft.ML.Data;

namespace WeatherReport.Models.Prediction
{
    public class ModelOutput
    {
        [VectorType(2)]
        public float[] Predictions { get; set; }
    }
}
