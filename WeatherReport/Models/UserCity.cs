namespace WeatherReport.Models
{
    public class UserCity
    {
        public Guid UserId { get; set; }
        public Guid CityId { get; set; }

        public City City { get; set; }
        public User User { get; set; }
    }
}
