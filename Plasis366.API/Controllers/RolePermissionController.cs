using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _service;

        public RolePermissionController(IRolePermissionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rolePermissions = await _service.GetAllAsync();

            return Ok(rolePermissions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var rolePermission = await _service.GetByIdAsync(id);

            if (rolePermission == null)
                return NotFound();

            return Ok(rolePermission);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RolePermission rolePermission)
        {
            var createdRolePermission =
                await _service.CreateAsync(rolePermission);

            return Ok(createdRolePermission);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            RolePermission rolePermission)
        {
            if (id != rolePermission.RolePermissionId)
                return BadRequest("ID mismatch.");

            var updatedRolePermission =
                await _service.UpdateAsync(rolePermission);

            return Ok(updatedRolePermission);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("RolePermission deleted successfully.");
        }

    }
}
