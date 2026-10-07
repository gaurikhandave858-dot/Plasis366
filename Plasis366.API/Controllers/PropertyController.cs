using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        // GET: api/Property
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var properties = await _propertyService.GetAllAsync();

            return Ok(properties);
        }

        // GET: api/Property/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var property = await _propertyService.GetByIdAsync(id);

            if (property == null)
            {
                return NotFound();
            }

            return Ok(property);
        }

        // POST: api/Property
        [HttpPost]
        public async Task<IActionResult> Create(Property property)
        {
            var createdProperty =
                await _propertyService.CreateAsync(property);

            return Ok(createdProperty);
        }

        // PUT: api/Property/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            Property property)
        {
            if (id != property.PropertyId)
            {
                return BadRequest(
                    "Property ID in URL and body must match.");
            }

            var existingProperty =
                await _propertyService.GetByIdAsync(id);

            if (existingProperty == null)
            {
                return NotFound();
            }

            var updatedProperty =
                await _propertyService.UpdateAsync(property);

            return Ok(updatedProperty);
        }

        // DELETE: api/Property/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result =
                await _propertyService.DeleteAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return Ok("Property deleted successfully.");
        }
    }
}
