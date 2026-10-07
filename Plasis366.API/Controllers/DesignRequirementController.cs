using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignRequirementController : ControllerBase
    {
        private readonly IDesignRequirementService
           _designRequirementService;

        public DesignRequirementController(
            IDesignRequirementService designRequirementService)
        {
            _designRequirementService =
                designRequirementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requirements =
                await _designRequirementService.GetAllAsync();

            return Ok(requirements);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var requirement =
                await _designRequirementService.GetByIdAsync(id);

            if (requirement == null)
                return NotFound();

            return Ok(requirement);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            DesignRequirement designRequirement)
        {
            var createdRequirement =
                await _designRequirementService
                    .CreateAsync(designRequirement);

            return Ok(createdRequirement);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            DesignRequirement designRequirement)
        {
            if (id != designRequirement.DesignRequirementId)
            {
                return BadRequest(
                    "Design Requirement ID in URL and body must match.");
            }

            var existingRequirement =
                await _designRequirementService.GetByIdAsync(id);

            if (existingRequirement == null)
                return NotFound();

            var updatedRequirement =
                await _designRequirementService
                    .UpdateAsync(designRequirement);

            return Ok(updatedRequirement);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result =
                await _designRequirementService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Design requirement deleted successfully.");
        }
    }
}
