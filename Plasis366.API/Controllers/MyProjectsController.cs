using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plasis366.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MyProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public MyProjectsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // The logged-in user's id comes from the token, never from the request
        private long? CurrentUserId()
        {
            var value = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(value, out var id) ? id : null;
        }

        private async Task<long?> CurrentCustomerId()
        {
            var userId = CurrentUserId();
            if (userId == null) return null;

            return await _db.Customers
                .Where(c => c.UserId == userId && c.IsActive)
                .Select(c => (long?)c.CustomerId)
                .FirstOrDefaultAsync();
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var customerId = await CurrentCustomerId();
            if (customerId == null)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only customers have a project dashboard." });

            var mine = _db.Projects.Where(p => p.CustomerId == customerId && p.IsActive);

            var counts = await mine
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            int CountOf(string status) => counts.Where(c => c.Status == status).Sum(c => c.Count);

            var recent = await Rows(mine).Take(5).ToListAsync();

            return Ok(new
            {
                stats = new
                {
                    total = counts.Sum(c => c.Count),
                    drafts = CountOf("Draft"),
                    designInProgress = CountOf("Design In Progress"),
                    completed = CountOf("Completed")
                },
                recent
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var customerId = await CurrentCustomerId();
            if (customerId == null)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only customers can list their projects." });

            var mine = _db.Projects.Where(p => p.CustomerId == customerId && p.IsActive);
            return Ok(await Rows(mine).ToListAsync());
        }

        private static IQueryable<ProjectRow> Rows(IQueryable<Domain.Project> query)
        {
            return query
                .OrderByDescending(p => p.ModifiedDate ?? p.CreatedDate)
                .Select(p => new ProjectRow(
                    p.ProjectId,
                    p.ProjectName,
                    p.Status,
                    p.Properties.Where(x => x.IsActive).Select(x => x.PropertyType).FirstOrDefault(),
                    p.Properties.Where(x => x.IsActive).Select(x => x.City != null ? x.City.CityName : null).FirstOrDefault(),
                    p.DesignerUser != null ? p.DesignerUser.FirstName + " " + p.DesignerUser.LastName : null,
                    p.ModifiedDate ?? p.CreatedDate));
        }

        public record ProjectRow(
            long ProjectId,
            string Name,
            string Status,
            string? PropertyType,
            string? City,
            string? Designer,
            DateTime Updated);
    }
}
