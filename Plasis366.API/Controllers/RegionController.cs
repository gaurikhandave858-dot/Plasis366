using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API
{
    [Route("api/[controller]")]
    [ApiController]
        public class RegionController : ControllerBase
        {
            private readonly IRegionService _regionService;

            public RegionController(IRegionService regionService)
            {
                _regionService = regionService;
            }

            // GET: api/Region
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var regions = await _regionService.GetAllAsync();

                return Ok(regions);
            }

            // GET: api/Region/1
            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(long id)
            {
                var region = await _regionService.GetByIdAsync(id);

                if (region == null)
                    return NotFound();

                return Ok(region);
            }

            // POST: api/Region
            [HttpPost]
            public async Task<IActionResult> Create(Region region)
            {
                var createdRegion = await _regionService.CreateAsync(region);

                return Ok(createdRegion);
            }

            // PUT: api/Region/1
            [HttpPut("{id}")]
            public async Task<IActionResult> Update(long id, Region region)
            {
                if (id != region.RegionId)
                    return BadRequest("Region ID mismatch.");

                var existingRegion = await _regionService.GetByIdAsync(id);

                if (existingRegion == null)
                    return NotFound();

                var updatedRegion = await _regionService.UpdateAsync(region);

                return Ok(updatedRegion);
            }

            // DELETE: api/Region/1
            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(long id)
            {
                var result = await _regionService.DeleteAsync(id);

                if (!result)
                    return NotFound();

                return Ok("Region deleted successfully.");
            }
        }
    }



