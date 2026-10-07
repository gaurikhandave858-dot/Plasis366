using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Application;
using Plasis366.Domain;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notification
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _notificationService.GetAllAsync();

            return Ok(notifications);
        }

        // GET: api/Notification/1
        [HttpGet("{notificationId}")]
        public async Task<IActionResult> GetById(long notificationId)
        {
            var notification = await _notificationService
                .GetByIdAsync(notificationId);

            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            return Ok(notification);
        }

        // POST: api/Notification
        [HttpPost]
        public async Task<IActionResult> Create(Notification notification)
        {
            var createdNotification = await _notificationService
                .CreateAsync(notification);

            return Ok(createdNotification);
        }

        // PUT: api/Notification/1
        [HttpPut("{notificationId}")]
        public async Task<IActionResult> Update(
            long notificationId,
            Notification notification)
        {
            if (notificationId != notification.NotificationId)
            {
                return BadRequest("Notification ID does not match.");
            }

            var existingNotification = await _notificationService
                .GetByIdAsync(notificationId);

            if (existingNotification == null)
            {
                return NotFound("Notification not found.");
            }

            var updatedNotification = await _notificationService
                .UpdateAsync(notification);

            return Ok(updatedNotification);
        }

        // DELETE: api/Notification/1
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> Delete(long notificationId)
        {
            var result = await _notificationService
                .DeleteAsync(notificationId);

            if (!result)
            {
                return NotFound("Notification not found.");
            }

            return Ok("Notification deleted successfully.");
        }
    }
}
