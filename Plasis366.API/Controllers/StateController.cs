using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;

        public StateController(IStateService stateService)
        {
            _stateService = stateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var states = await _stateService.GetAllAsync();

            return Ok(states);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var state = await _stateService.GetByIdAsync(id);

            if (state == null)
                return NotFound();

            return Ok(state);
        }

        [HttpPost]
        public async Task<IActionResult> Create(State state)
        {
            var createdState = await _stateService.CreateAsync(state);

            return Ok(createdState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, State state)
        {
            if (id != state.StateId)
                return BadRequest("State ID mismatch.");

            var existingState = await _stateService.GetByIdAsync(id);

            if (existingState == null)
                return NotFound();

            var updatedState = await _stateService.UpdateAsync(state);

            return Ok(updatedState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _stateService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("State deleted successfully.");
        }
    }
}
