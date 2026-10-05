using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userRoles = await _userRoleService.GetAllAsync();

            return Ok(userRoles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var userRole = await _userRoleService.GetByIdAsync(id);

            if (userRole == null)
                return NotFound();

            return Ok(userRole);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserRole userRole)
        {
            var createdUserRole =
                await _userRoleService.CreateAsync(userRole);

            return Ok(createdUserRole);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            UserRole userRole)
        {
            if (id != userRole.UserRoleId)
                return BadRequest("UserRole ID mismatch.");

            var existingUserRole =
                await _userRoleService.GetByIdAsync(id);

            if (existingUserRole == null)
                return NotFound();

            var updatedUserRole =
                await _userRoleService.UpdateAsync(userRole);

            return Ok(updatedUserRole);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result =
                await _userRoleService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("User role deleted successfully.");
        }

    }
}
