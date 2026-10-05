using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _permissionService.GetAllAsync();

            return Ok(permissions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var permission = await _permissionService.GetByIdAsync(id);

            if (permission == null)
                return NotFound();

            return Ok(permission);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Permission permission)
        {
            var createdPermission =
                await _permissionService.CreateAsync(permission);

            return Ok(createdPermission);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            Permission permission)
        {
            if (id != permission.PermissionId)
                return BadRequest("Permission ID mismatch.");

            var existingPermission =
                await _permissionService.GetByIdAsync(id);

            if (existingPermission == null)
                return NotFound();

            var updatedPermission =
                await _permissionService.UpdateAsync(permission);

            return Ok(updatedPermission);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result =
                await _permissionService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Permission deleted successfully.");
        }
    }
}
