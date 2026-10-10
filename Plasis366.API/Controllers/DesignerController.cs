using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.API.Controllers
{
    // Everything here is for logged-in users who have the Designer role.
    // A designer sees only projects in their own studio (tenant) that are either
    // waiting for a designer (Submitted, unassigned) or already assigned to them.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Designer")]
    public class DesignerController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public DesignerController(ApplicationDbContext db, IConfiguration config, IWebHostEnvironment env)
        {
            _db = db;
            _config = config;
            _env = env;
        }

        // Both values come from the token, never from the request
        private long? CurrentUserId()
        {
            var value = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(value, out var id) ? id : null;
        }

        private long? CurrentTenantId()
        {
            return long.TryParse(User.FindFirstValue("tenantId"), out var id) ? id : null;
        }

        // Projects waiting for a designer in this studio
        private IQueryable<Project> Available(long tenantId) =>
            _db.Projects.Where(p => p.IsActive && p.TenantId == tenantId &&
                                    p.Status == "Submitted" && p.DesignerUserId == null);

        // Projects this designer has taken
        private IQueryable<Project> Mine(long tenantId, long userId) =>
            _db.Projects.Where(p => p.IsActive && p.TenantId == tenantId && p.DesignerUserId == userId);

        // A project the designer is allowed to open: available or theirs
        private IQueryable<Project> Visible(long tenantId, long userId) =>
            _db.Projects.Where(p => p.IsActive && p.TenantId == tenantId &&
                (p.DesignerUserId == userId || (p.DesignerUserId == null && p.Status == "Submitted")));

        private IQueryable<DesignerRow> Rows(IQueryable<Project> query) =>
            query
                .OrderByDescending(p => p.ModifiedDate ?? p.CreatedDate)
                .Select(p => new DesignerRow(
                    p.ProjectId,
                    p.ProjectCode,
                    p.ProjectName,
                    p.Status,
                    p.BudgetMin,
                    p.BudgetMax,
                    _db.Customers
                        .Where(c => c.CustomerId == p.CustomerId)
                        .Select(c => c.FirstName + " " + c.LastName)
                        .FirstOrDefault(),
                    p.Properties
                        .Where(x => x.IsActive)
                        .OrderBy(x => x.PropertyId)
                        .Select(x => x.PropertyType)
                        .FirstOrDefault(),
                    p.ModifiedDate ?? p.CreatedDate,
                    p.DesignerUserId));

        // ---------- Dashboard and lists ----------

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var tenantId = CurrentTenantId();
            var userId = CurrentUserId();
            if (tenantId == null || userId == null) return Unauthorized();

            var available = await Available(tenantId.Value).CountAsync();

            var mineCounts = await Mine(tenantId.Value, userId.Value)
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            int CountOf(params string[] statuses) =>
                mineCounts.Where(c => statuses.Contains(c.Status)).Sum(c => c.Count);

            var recent = await Rows(Mine(tenantId.Value, userId.Value)).Take(5).ToListAsync();

            return Ok(new
            {
                stats = new
                {
                    available,
                    inProgress = CountOf("Under Review", "Design In Progress"),
                    withCustomer = CountOf("Design Submitted", "Customer Review", "Revision Requested"),
                    completed = CountOf("Approved", "Completed")
                },
                recent
            });
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            var tenantId = CurrentTenantId();
            if (tenantId == null) return Unauthorized();
            return Ok(await Rows(Available(tenantId.Value)).ToListAsync());
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var tenantId = CurrentTenantId();
            var userId = CurrentUserId();
            if (tenantId == null || userId == null) return Unauthorized();
            return Ok(await Rows(Mine(tenantId.Value, userId.Value)).ToListAsync());
        }

        // ---------- One project (read-only) ----------

        [HttpGet("projects/{projectId:long}")]
        public async Task<IActionResult> GetOne(long projectId)
        {
            var tenantId = CurrentTenantId();
            var userId = CurrentUserId();
            if (tenantId == null || userId == null) return Unauthorized();

            var project = await Visible(tenantId.Value, userId.Value)
                .Include(p => p.Properties.Where(x => x.IsActive))
                    .ThenInclude(x => x.Rooms.Where(r => r.IsActive))
                .Include(p => p.DesignRequirements.Where(d => d.IsActive))
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null)
                return NotFound(new { message = "Project not found." });

            var customerName = await _db.Customers
                .Where(c => c.CustomerId == project.CustomerId)
                .Select(c => c.FirstName + " " + c.LastName)
                .FirstOrDefaultAsync();

            var property = project.Properties.OrderBy(x => x.PropertyId).FirstOrDefault();
            var requirement = project.DesignRequirements
                .OrderBy(d => d.DesignRequirementId)
                .FirstOrDefault(d => d.RoomId == null);

            return Ok(new
            {
                projectId = project.ProjectId,
                projectCode = project.ProjectCode,
                status = project.Status,
                isMine = project.DesignerUserId == userId,
                customerName,
                projectName = project.ProjectName,
                description = project.Description,
                budgetMin = project.BudgetMin,
                budgetMax = project.BudgetMax,
                expectedStartDate = project.ExpectedStartDate,
                propertyType = property?.PropertyType,
                propertyName = property?.PropertyName,
                address = property?.Address,
                totalArea = property?.TotalArea,
                numberOfFloors = property?.NumberOfFloors,
                rooms = (property?.Rooms ?? new List<Room>())
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.RoomId)
                    .Select(r => new
                    {
                        roomId = r.RoomId,
                        roomType = r.RoomType,
                        roomName = r.RoomName,
                        length = r.Length,
                        width = r.Width,
                        height = r.Height
                    }),
                requirements = new
                {
                    preferredStyle = requirement?.PreferredStyle,
                    preferredColors = requirement?.PreferredColors,
                    furnitureRequirements = requirement?.FurnitureRequirements,
                    storageRequirements = requirement?.StorageRequirements,
                    lightingRequirements = requirement?.LightingRequirements,
                    specialRequirements = requirement?.SpecialRequirements,
                    additionalNotes = requirement?.AdditionalNotes
                }
            });
        }

        // ---------- Take a project ----------

        // First designer to press the button gets the project. The update only succeeds
        // while the project is still Submitted and unassigned, so two designers
        // pressing at the same moment cannot both win.
        [HttpPost("projects/{projectId:long}/take")]
        public async Task<IActionResult> Take(long projectId)
        {
            var tenantId = CurrentTenantId();
            var userId = CurrentUserId();
            if (tenantId == null || userId == null) return Unauthorized();

            var changed = await _db.Projects
                .Where(p => p.ProjectId == projectId && p.IsActive && p.TenantId == tenantId &&
                            p.Status == "Submitted" && p.DesignerUserId == null)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.DesignerUserId, userId)
                    .SetProperty(p => p.Status, "Under Review")
                    .SetProperty(p => p.ModifiedDate, DateTime.Now)
                    .SetProperty(p => p.ModifiedBy, userId));

            if (changed == 0)
                return Conflict(new { message = "This project is no longer available." });

            _db.Set<ProjectStatusHistory>().Add(new ProjectStatusHistory
            {
                ProjectId = projectId,
                Status = "Under Review",
                Remarks = "Taken by designer",
                ChangedBy = userId.Value,
                CreatedBy = userId
            });
            await _db.SaveChangesAsync();

            return Ok(new { projectId, status = "Under Review" });
        }

        // ---------- Customer's files (read-only) ----------

        [HttpGet("projects/{projectId:long}/attachments")]
        public async Task<IActionResult> ListAttachments(long projectId)
        {
            var tenantId = CurrentTenantId();
            var userId = CurrentUserId();
            if (tenantId == null || userId == null) return Unauthorized();

            if (!await Visible(tenantId.Value, userId.Value).AnyAsync(p => p.ProjectId == projectId))
                return NotFound(new { message = "Project not found." });

            var items = await _db.ProjectAttachments
                .Where(a => a.ProjectId == projectId && a.IsActive)
                .OrderBy(a => a.CreatedDate)
                .Select(a => new
                {
                    attachmentId = a.ProjectAttachmentId,
                    fileName = a.FileName,
                    fileType = a.FileType,
                    fileSize = a.FileSize,
                    description = a.Description,
                    uploaded = a.CreatedDate
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("projects/{projectId:long}/attachments/{attachmentId:long}/file")]
        public async Task<IActionResult> DownloadAttachment(long projectId, long attachmentId)
        {
            var tenantId = CurrentTenantId();
            var userId = CurrentUserId();
            if (tenantId == null || userId == null) return Unauthorized();

            if (!await Visible(tenantId.Value, userId.Value).AnyAsync(p => p.ProjectId == projectId))
                return NotFound(new { message = "Project not found." });

            var attachment = await _db.ProjectAttachments.FirstOrDefaultAsync(a =>
                a.ProjectAttachmentId == attachmentId && a.ProjectId == projectId && a.IsActive);
            if (attachment == null)
                return NotFound(new { message = "File not found." });

            var uploadRoot = _config["Uploads:Root"] is { Length: > 0 } custom
                ? custom
                : Path.Combine(_env.ContentRootPath, "Uploads");

            var root = Path.GetFullPath(uploadRoot);
            var fullPath = Path.GetFullPath(Path.Combine(root, attachment.FilePath));

            // Never read anything outside the uploads folder
            if (!fullPath.StartsWith(root + Path.DirectorySeparatorChar) || !System.IO.File.Exists(fullPath))
                return NotFound(new { message = "File not found." });

            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return PhysicalFile(fullPath, attachment.FileType ?? "application/octet-stream", attachment.FileName);
        }

        public record DesignerRow(
            long ProjectId,
            string ProjectCode,
            string ProjectName,
            string Status,
            decimal? BudgetMin,
            decimal? BudgetMax,
            string? CustomerName,
            string? PropertyType,
            DateTime LastUpdated,
            long? DesignerUserId);
    }
}