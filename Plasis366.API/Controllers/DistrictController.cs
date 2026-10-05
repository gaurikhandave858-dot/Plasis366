using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private readonly IDistrictService _districtService;

        public DistrictController(IDistrictService districtService)
        {
            _districtService = districtService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var districts = await _districtService.GetAllAsync();

            return Ok(districts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var district = await _districtService.GetByIdAsync(id);

            if (district == null)
                return NotFound();

            return Ok(district);
        }

        [HttpPost]
        public async Task<IActionResult> Create(District district)
        {
            var createdDistrict =
                await _districtService.CreateAsync(district);

            return Ok(createdDistrict);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, District district)
        {
            if (id != district.DistrictId)
                return BadRequest("District ID mismatch.");

            var existingDistrict =
                await _districtService.GetByIdAsync(id);

            if (existingDistrict == null)
                return NotFound();

            var updatedDistrict =
                await _districtService.UpdateAsync(district);

            return Ok(updatedDistrict);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result =
                await _districtService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("District deleted successfully.");
        }
    }
}
