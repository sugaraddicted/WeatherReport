﻿using Microsoft.ML.Data;

namespace WeatherReport.Models.Prediction
{
    public class TrainingDataModel
    {
        [LoadColumn(0)]
        public DateTime Timestamp { get; set; }
        [LoadColumn(1)]
        public float Temperature { get; set; }
        [LoadColumn(2)]
        public float Pressure { get; set; }
        [LoadColumn(3)]
        public float WindSpeed { get; set; }
        [LoadColumn(4)]
        public float Humidity { get; set; }
        [LoadColumn(5)]
        public float RainPresence { get; set; }
        [LoadColumn(6)]
        public float Cloudiness { get; set; }
    }
}
