using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.API.Controllers
{
    // A user only ever sees and changes their own notifications.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public NotificationsController(ApplicationDbContext db)
        {
            _db = db;
        }

        private long? CurrentUserId()
        {
            var value = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(value, out var id) ? id : null;
        }

        private IQueryable<Notification> Mine(long userId) =>
            _db.Set<Notification>().Where(n => n.UserId == userId && n.IsActive);

        // The latest notifications plus how many are unread (for the bell badge)
        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var userId = CurrentUserId();
            if (userId == null) return Unauthorized();

            var unread = await Mine(userId.Value).CountAsync(n => !n.IsRead);

            var items = await Mine(userId.Value)
                .OrderByDescending(n => n.ModifiedDate ?? n.CreatedDate)
                .Take(30)
                .Select(n => new
                {
                    notificationId = n.NotificationId,
                    projectId = n.ProjectId,
                    title = n.Title,
                    message = n.Message,
                    isRead = n.IsRead,
                    created = n.ModifiedDate ?? n.CreatedDate
                })
                .ToListAsync();

            return Ok(new { unread, items });
        }

        [HttpPost("{notificationId:long}/read")]
        public async Task<IActionResult> MarkRead(long notificationId)
        {
            var userId = CurrentUserId();
            if (userId == null) return Unauthorized();

            var notification = await Mine(userId.Value)
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);
            if (notification == null)
                return NotFound(new { message = "Notification not found." });

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.Now;
                notification.ModifiedDate = DateTime.Now;
                notification.ModifiedBy = userId;
                await _db.SaveChangesAsync();
            }

            return Ok(new { notificationId });
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = CurrentUserId();
            if (userId == null) return Unauthorized();

            var now = DateTime.Now;
            var unread = await Mine(userId.Value).Where(n => !n.IsRead).ToListAsync();
            foreach (var n in unread)
            {
                n.IsRead = true;
                n.ReadDate = now;
                n.ModifiedDate = now;
                n.ModifiedBy = userId;
            }
            await _db.SaveChangesAsync();

            return Ok(new { marked = unread.Count });
        }
    }
}