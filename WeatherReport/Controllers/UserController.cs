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

        [HttpPost("add-city")]
        public async Task<ActionResult> AddCity([FromBody] string cityName)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            City city = _context.Cities.Where(c => c.Name == cityName).FirstOrDefault();

            if (city == null)
            {
                city = new City() { Name = cityName };
                _context.Cities.Add(city);
                _context.SaveChanges();
            }

            UserCity userCity = _context.UserCities.Where(uc => uc.City.Name == cityName).FirstOrDefault();

            if (userCity == null)
            {
                userCity = new UserCity()
                {
                    UserId = user.Id,
                    CityId = city.Id
                };
                _context.UserCities.Add(userCity);
                _context.SaveChanges();
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("delete-city")]
        public async Task<ActionResult> DeleteCity([FromBody] string cityName)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            City city = _context.Cities.Where(c => c.Name == cityName).FirstOrDefault();
            if (city == null)
            {
                return NotFound();
            }

            var userCity = user.UserCities.Where(uc => uc.City == city).FirstOrDefault();
            if (userCity == null)
            {
                return NotFound();
            }

            user.UserCities.Remove(userCity);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }
    }
}