﻿namespace WeatherReport.Models
{
    public class City
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<UserCity> UserCities { get; set; }
    }
}
