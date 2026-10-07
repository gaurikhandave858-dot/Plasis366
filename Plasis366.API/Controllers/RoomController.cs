using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {

        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomService.GetAllAsync();

            return Ok(rooms);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var room = await _roomService.GetByIdAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(Room room)
        {
            var createdRoom =
                await _roomService.CreateAsync(room);

            return Ok(createdRoom);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            Room room)
        {
            if (id != room.RoomId)
            {
                return BadRequest(
                    "Room ID in URL and body must match.");
            }

            var existingRoom =
                await _roomService.GetByIdAsync(id);

            if (existingRoom == null)
            {
                return NotFound();
            }

            var updatedRoom =
                await _roomService.UpdateAsync(room);

            return Ok(updatedRoom);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result =
                await _roomService.DeleteAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return Ok("Room deleted successfully.");
        }
    }
}
