using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.API.Controllers
{
    // The story of one project: every status change, oldest first.
    // Only the customer who owns the project and its assigned designer can read it.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectHistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ProjectHistoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        private long? CurrentUserId()
        {
            var value = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(value, out var id) ? id : null;
        }

        private long? CurrentTenantId() =>
            long.TryParse(User.FindFirstValue("tenantId"), out var id) ? id : null;

        [HttpGet("{projectId:long}")]
        public async Task<IActionResult> Get(long projectId)
        {
            var userId = CurrentUserId();
            var tenantId = CurrentTenantId();
            if (userId == null || tenantId == null) return Unauthorized();

            var allowed = await _db.Projects.AnyAsync(p =>
                p.ProjectId == projectId && p.IsActive && p.TenantId == tenantId &&
                (p.DesignerUserId == userId ||
                 _db.Customers.Any(c => c.CustomerId == p.CustomerId && c.UserId == userId && c.IsActive)));
            if (!allowed) return NotFound(new { message = "Project not found." });

            var items = await _db.Set<ProjectStatusHistory>()
                .Where(h => h.ProjectId == projectId && h.IsActive)
                .OrderBy(h => h.ProjectStatusHistoryId)
                .Select(h => new
                {
                    status = h.Status,
                    remarks = h.Remarks,
                    when = h.CreatedDate
                })
                .ToListAsync();

            return Ok(items);
        }
    }
}
