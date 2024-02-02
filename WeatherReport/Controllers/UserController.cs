using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeatherReport.Data;
using WeatherReport.Models;

namespace WeatherReport.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public UserController(UserManager<User> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("cities")]
        public async Task<ActionResult<List<string>>> GetUsersCities()
        {
            var user = await _userManager.GetUserAsync(User);
            var cities = _context.UserCities.Where(uc => uc.UserId == user.Id).Select(uc => uc.City).ToList();

            if (cities == null)
            {
                return NotFound();
            }

            var result = cities.Select(c => c.Name).ToList();
            return Ok(result);
        }
    }
}