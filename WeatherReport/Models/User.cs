using Microsoft.AspNetCore.Identity;

namespace WeatherReport.Models
{
    public class User : IdentityUser<Guid>
    {
        public List<UserCity>? UserCities { get; set; }
    }
}
