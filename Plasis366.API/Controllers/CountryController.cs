using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var countries = await _countryService.GetAllAsync();

            return Ok(countries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var country = await _countryService.GetByIdAsync(id);

            if (country == null)
                return NotFound();

            return Ok(country);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Country country)
        {
            var createdCountry = await _countryService.CreateAsync(country);

            return Ok(createdCountry);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Country country)
        {
            if (id != country.CountryId)
                return BadRequest("Country ID mismatch.");

            var existingCountry = await _countryService.GetByIdAsync(id);

            if (existingCountry == null)
                return NotFound();

            var updatedCountry = await _countryService.UpdateAsync(country);

            return Ok(updatedCountry);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _countryService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Country deleted successfully.");
        }
    }
}
