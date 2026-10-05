using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TalukaController : ControllerBase
    {
        private readonly ITalukaService _talukaService;

        public TalukaController(ITalukaService talukaService)
        {
            _talukaService = talukaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var talukas = await _talukaService.GetAllAsync();

            return Ok(talukas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var taluka = await _talukaService.GetByIdAsync(id);

            if (taluka == null)
                return NotFound();

            return Ok(taluka);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Taluka taluka)
        {
            var createdTaluka = await _talukaService.CreateAsync(taluka);

            return Ok(createdTaluka);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Taluka taluka)
        {
            if (id != taluka.TalukaId)
                return BadRequest("Taluka ID mismatch.");

            var existingTaluka = await _talukaService.GetByIdAsync(id);

            if (existingTaluka == null)
                return NotFound();

            var updatedTaluka = await _talukaService.UpdateAsync(taluka);

            return Ok(updatedTaluka);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _talukaService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Taluka deleted successfully.");
        }
    }
}
