namespace WeatherReport.Models.ApiResponse
{
    public class LocationDataModel
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public override string ToString()
        {
            return $"{Name}, {Country} - Latitude: {Lat}, Longitude: {Lon}";
        }
    }
}
