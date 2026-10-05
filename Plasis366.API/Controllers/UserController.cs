using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            var createdUser = await _userService.CreateAsync(user);

            return Ok(createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, User user)
        {
            if (id != user.UserId)
                return BadRequest("User ID mismatch.");

            var existingUser = await _userService.GetByIdAsync(id);

            if (existingUser == null)
                return NotFound();

            var updatedUser = await _userService.UpdateAsync(user);

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _userService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("User deleted successfully.");
        }
    }
}
