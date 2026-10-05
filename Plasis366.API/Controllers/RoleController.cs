using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();

            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Role role)
        {
            var createdRole = await _roleService.CreateAsync(role);

            return Ok(createdRole);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Role role)
        {
            if (id != role.RoleId)
                return BadRequest("Role ID mismatch.");

            var existingRole = await _roleService.GetByIdAsync(id);

            if (existingRole == null)
                return NotFound();

            var updatedRole = await _roleService.UpdateAsync(role);

            return Ok(updatedRole);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _roleService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Role deleted successfully.");
        }
    }
}
