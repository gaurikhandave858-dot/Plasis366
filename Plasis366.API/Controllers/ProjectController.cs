using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectController(IProjectService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _service.GetAllAsync();

            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var project = await _service.GetByIdAsync(id);

            if (project == null)
                return NotFound();

            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Project project)
        {
            var createdProject =
                await _service.CreateAsync(project);

            return Ok(createdProject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            Project project)
        {
            if (id != project.ProjectId)
                return BadRequest("ID mismatch.");

            var updatedProject =
                await _service.UpdateAsync(project);

            return Ok(updatedProject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Project deleted successfully.");
        }
    }
}
