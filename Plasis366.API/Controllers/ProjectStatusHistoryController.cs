using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectStatusHistoryController : ControllerBase
    {
        private readonly IProjectStatusHistoryService
            _projectStatusHistoryService;

        public ProjectStatusHistoryController(
            IProjectStatusHistoryService projectStatusHistoryService)
        {
            _projectStatusHistoryService =
                projectStatusHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var histories =
                await _projectStatusHistoryService.GetAllAsync();

            return Ok(histories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var history =
                await _projectStatusHistoryService.GetByIdAsync(id);

            if (history == null)
                return NotFound();

            return Ok(history);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] ProjectStatusHistory projectStatusHistory)
        {
            var createdHistory =
                await _projectStatusHistoryService
                    .CreateAsync(projectStatusHistory);

            return Ok(createdHistory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            [FromBody] ProjectStatusHistory projectStatusHistory)
        {
            if (id != projectStatusHistory.ProjectStatusHistoryId)
            {
                return BadRequest(
                    "Project Status History ID in URL and body must match.");
            }

            var existingHistory =
                await _projectStatusHistoryService.GetByIdAsync(id);

            if (existingHistory == null)
                return NotFound();

            var updatedHistory =
                await _projectStatusHistoryService
                    .UpdateAsync(projectStatusHistory);

            return Ok(updatedHistory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result =
                await _projectStatusHistoryService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Project status history deleted successfully.");
        }
    }
}
