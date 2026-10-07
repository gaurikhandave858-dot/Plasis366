using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectAttachmentController : ControllerBase
    {
        private readonly IProjectAttachmentService
            _projectAttachmentService;

        public ProjectAttachmentController(
            IProjectAttachmentService projectAttachmentService)
        {
            _projectAttachmentService = projectAttachmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var attachments =
                await _projectAttachmentService.GetAllAsync();

            return Ok(attachments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var attachment =
                await _projectAttachmentService.GetByIdAsync(id);

            if (attachment == null)
                return NotFound();

            return Ok(attachment);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            ProjectAttachment projectAttachment)
        {
            var createdAttachment =
                await _projectAttachmentService
                    .CreateAsync(projectAttachment);

            return Ok(createdAttachment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            ProjectAttachment projectAttachment)
        {
            if (id != projectAttachment.ProjectAttachmentId)
            {
                return BadRequest(
                    "Project Attachment ID in URL and body must match.");
            }

            var existingAttachment =
                await _projectAttachmentService.GetByIdAsync(id);

            if (existingAttachment == null)
                return NotFound();

            var updatedAttachment =
                await _projectAttachmentService
                    .UpdateAsync(projectAttachment);

            return Ok(updatedAttachment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result =
                await _projectAttachmentService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Project attachment deleted successfully.");
        }
    }
}
