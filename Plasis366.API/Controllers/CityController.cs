using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cities = await _cityService.GetAllAsync();

            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var city = await _cityService.GetByIdAsync(id);

            if (city == null)
                return NotFound();

            return Ok(city);
        }

        [HttpPost]
        public async Task<IActionResult> Create(City city)
        {
            var createdCity = await _cityService.CreateAsync(city);

            return Ok(createdCity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, City city)
        {
            if (id != city.CityId)
                return BadRequest("City ID mismatch.");

            var existingCity = await _cityService.GetByIdAsync(id);

            if (existingCity == null)
                return NotFound();

            var updatedCity = await _cityService.UpdateAsync(city);

            return Ok(updatedCity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _cityService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("City deleted successfully.");
        }
    }
}
