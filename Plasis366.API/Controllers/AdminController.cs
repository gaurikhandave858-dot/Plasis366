using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plasis366.API;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.API.Controllers
{
    // For the studio's Admin and Manager. Everything is limited to their own tenant.
    // Managers can look; only an Admin can add or switch off designers.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
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

        // ---------- Dashboard ----------

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var tenantId = CurrentTenantId();
            if (tenantId == null) return Unauthorized();

            var projects = _db.Projects.Where(p => p.IsActive && p.TenantId == tenantId);

            var byStatus = await projects
                .GroupBy(p => p.Status)
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToListAsync();

            var waiting = await projects.CountAsync(p => p.Status == "Submitted" && p.DesignerUserId == null);

            var designers = await ActiveUsersInRole(tenantId.Value, "Designer").CountAsync();
            var customers = await ActiveUsersInRole(tenantId.Value, "Customer").CountAsync();

            var recent = await projects
                .OrderByDescending(p => p.ModifiedDate ?? p.CreatedDate)
                .Take(10)
                .Select(p => new
                {
                    projectId = p.ProjectId,
                    projectCode = p.ProjectCode,
                    projectName = p.ProjectName,
                    status = p.Status,
                    designerUserId = p.DesignerUserId,
                    customerName = _db.Customers
                        .Where(c => c.CustomerId == p.CustomerId)
                        .Select(c => c.FirstName + " " + c.LastName)
                        .FirstOrDefault(),
                    designerName = _db.Users
                        .Where(u => u.UserId == p.DesignerUserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault(),
                    lastUpdated = p.ModifiedDate ?? p.CreatedDate
                })
                .ToListAsync();

            return Ok(new
            {
                totalProjects = byStatus.Sum(x => x.count),
                waiting,
                designers,
                customers,
                byStatus,
                recent
            });
        }

        // ---------- Project list (used when a card or status is clicked) ----------

        // status: only that status. waiting=true: submitted projects with no designer yet.
        // With neither, every project in the studio (newest first, up to 200).
        [HttpGet("projects")]
        public async Task<IActionResult> Projects([FromQuery] string? status, [FromQuery] bool waiting = false)
        {
            var tenantId = CurrentTenantId();
            if (tenantId == null) return Unauthorized();

            var query = _db.Projects.Where(p => p.IsActive && p.TenantId == tenantId);

            if (waiting)
                query = query.Where(p => p.Status == "Submitted" && p.DesignerUserId == null);
            else if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(p => p.Status == status);

            var rows = await query
                .OrderByDescending(p => p.ModifiedDate ?? p.CreatedDate)
                .Take(200)
                .Select(p => new
                {
                    projectId = p.ProjectId,
                    projectCode = p.ProjectCode,
                    projectName = p.ProjectName,
                    status = p.Status,
                    designerUserId = p.DesignerUserId,
                    customerName = _db.Customers
                        .Where(c => c.CustomerId == p.CustomerId)
                        .Select(c => c.FirstName + " " + c.LastName)
                        .FirstOrDefault(),
                    designerName = _db.Users
                        .Where(u => u.UserId == p.DesignerUserId)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault(),
                    lastUpdated = p.ModifiedDate ?? p.CreatedDate
                })
                .ToListAsync();

            return Ok(rows);
        }

        private IQueryable<User> ActiveUsersInRole(long tenantId, string roleName) =>
            _db.Users.Where(u => u.IsActive && u.TenantId == tenantId &&
                u.UserRoles.Any(ur => ur.IsActive && ur.Role != null && ur.Role.RoleName == roleName));

        // ---------- Users ----------

        [HttpGet("users")]
        public async Task<IActionResult> Users()
        {
            var tenantId = CurrentTenantId();
            if (tenantId == null) return Unauthorized();

            var users = await _db.Users
                .Where(u => u.TenantId == tenantId)
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Select(u => new
                {
                    userId = u.UserId,
                    name = u.FirstName + " " + u.LastName,
                    email = u.Email,
                    phone = u.PhoneNumber,
                    joined = u.CreatedDate,
                    isActive = u.IsActive,
                    roles = u.UserRoles
                        .Where(ur => ur.IsActive && ur.Role != null)
                        .Select(ur => ur.Role!.RoleName)
                        .ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        public class NewDesignerRequest
        {
            [Required(ErrorMessage = "First name is required.")]
            [StringLength(100, ErrorMessage = "First name can be at most 100 characters.")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Last name is required.")]
            [StringLength(100, ErrorMessage = "Last name can be at most 100 characters.")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
            [StringLength(100, ErrorMessage = "Email can be at most 100 characters.")]
            public string Email { get; set; } = string.Empty;

            [RegularExpression(@"^\+?[0-9\s\-]{7,15}$", ErrorMessage = "Phone number must be 7 to 15 digits.")]
            public string? PhoneNumber { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(72, MinimumLength = 8, ErrorMessage = "Password must be 8 to 72 characters.")]
            [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "Password must contain at least one letter and one number.")]
            public string Password { get; set; } = string.Empty;
        }

        // The studio adds its own designers. They are staff, not customers who signed up.
        [HttpPost("designers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddDesigner(NewDesignerRequest request)
        {
            var tenantId = CurrentTenantId();
            var me = CurrentUserId();
            if (tenantId == null || me == null) return Unauthorized();

            var email = request.Email.Trim().ToLowerInvariant();

            var taken = await _db.Users.AnyAsync(u => u.TenantId == tenantId && u.Email == email);
            if (taken)
                return Conflict(new { message = "This email is already registered." });

            var designerRole = await _db.Roles
                .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.RoleName == "Designer");
            if (designerRole == null)
                return BadRequest(new { message = "The Designer role was not found." });

            var user = new User
            {
                TenantId = tenantId.Value,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                PhoneNumber = request.PhoneNumber,
                UserName = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedBy = me
            };
            user.UserRoles.Add(new UserRole { RoleId = designerRole.RoleId, CreatedBy = me });

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { userId = user.UserId, name = $"{user.FirstName} {user.LastName}", email = user.Email });
        }

        // ---------- Customers ----------

        // One customer with all of their projects (read-only for the admin)
        [HttpGet("customers/{userId:long}")]
        public async Task<IActionResult> CustomerDetail(long userId)
        {
            var tenantId = CurrentTenantId();
            if (tenantId == null) return Unauthorized();

            var user = await _db.Users
                .Where(u => u.UserId == userId && u.TenantId == tenantId &&
                            u.UserRoles.Any(ur => ur.Role != null && ur.Role.RoleName == "Customer"))
                .Select(u => new
                {
                    userId = u.UserId,
                    name = u.FirstName + " " + u.LastName,
                    email = u.Email,
                    phone = u.PhoneNumber,
                    joined = u.CreatedDate,
                    isActive = u.IsActive
                })
                .FirstOrDefaultAsync();
            if (user == null) return NotFound(new { message = "Customer not found." });

            var address = await _db.Customers
                .Where(c => c.UserId == userId)
                .Select(c => c.Address)
                .FirstOrDefaultAsync();

            var projects = await _db.Projects
                .Where(p => p.IsActive && p.TenantId == tenantId &&
                            _db.Customers.Any(c => c.CustomerId == p.CustomerId && c.UserId == userId))
                .OrderByDescending(p => p.ModifiedDate ?? p.CreatedDate)
                .Select(p => new
                {
                    projectId = p.ProjectId,
                    projectCode = p.ProjectCode,
                    projectName = p.ProjectName,
                    status = p.Status,
                    designerName = _db.Users
                        .Where(d => d.UserId == p.DesignerUserId)
                        .Select(d => d.FirstName + " " + d.LastName)
                        .FirstOrDefault(),
                    lastUpdated = p.ModifiedDate ?? p.CreatedDate
                })
                .ToListAsync();

            return Ok(new { user, address, projects });
        }

        // ---------- Switch a login off or on (designers and customers) ----------

        [HttpPost("users/{userId:long}/deactivate")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> Deactivate(long userId) => SetActive(userId, false);

        [HttpPost("users/{userId:long}/activate")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> Activate(long userId) => SetActive(userId, true);

        private async Task<IActionResult> SetActive(long userId, bool active)
        {
            var tenantId = CurrentTenantId();
            var me = CurrentUserId();
            if (tenantId == null || me == null) return Unauthorized();

            var user = await _db.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.TenantId == tenantId);
            if (user == null) return NotFound(new { message = "User not found." });

            var roles = user.UserRoles
                .Where(ur => ur.IsActive && ur.Role != null)
                .Select(ur => ur.Role!.RoleName)
                .ToList();

            // Admin and Manager accounts are never switched off from here
            if (roles.Contains("Admin") || roles.Contains("Manager"))
                return BadRequest(new { message = "Admin and Manager accounts cannot be changed here." });

            if (!active && roles.Contains("Designer"))
            {
                var busy = await _db.Projects.AnyAsync(p => p.IsActive && p.DesignerUserId == userId &&
                                                            p.Status != "Completed");
                if (busy)
                    return BadRequest(new { message = "This designer still has unfinished projects. Reassign them first." });
            }

            user.IsActive = active;
            user.ModifiedDate = DateTime.Now;
            user.ModifiedBy = me;
            await _db.SaveChangesAsync();

            return Ok(new { userId, isActive = active });
        }

        // ---------- Reassign a project to another designer ----------

        // Active designers, with how many unfinished projects each one has
        [HttpGet("designers")]
        public async Task<IActionResult> Designers()
        {
            var tenantId = CurrentTenantId();
            if (tenantId == null) return Unauthorized();

            var designers = await ActiveUsersInRole(tenantId.Value, "Designer")
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Select(u => new
                {
                    userId = u.UserId,
                    name = u.FirstName + " " + u.LastName,
                    openProjects = _db.Projects.Count(p => p.IsActive && p.DesignerUserId == u.UserId &&
                                                           p.Status != "Completed")
                })
                .ToListAsync();

            return Ok(designers);
        }

        public record ReassignRequest(long DesignerUserId);

        private static readonly string[] Reassignable =
            { "Under Review", "Design In Progress", "Design Submitted", "Revision Requested", "Approved" };

        [HttpPost("projects/{projectId:long}/reassign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reassign(long projectId, [FromBody] ReassignRequest request)
        {
            var tenantId = CurrentTenantId();
            var me = CurrentUserId();
            if (tenantId == null || me == null) return Unauthorized();

            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.IsActive && p.TenantId == tenantId);
            if (project == null) return NotFound(new { message = "Project not found." });

            if (project.DesignerUserId == null || !Reassignable.Contains(project.Status))
                return BadRequest(new { message = "Only a project that a designer is working on can be reassigned." });

            if (project.DesignerUserId == request.DesignerUserId)
                return BadRequest(new { message = "This designer already has the project." });

            var newDesigner = await ActiveUsersInRole(tenantId.Value, "Designer")
                .FirstOrDefaultAsync(u => u.UserId == request.DesignerUserId);
            if (newDesigner == null)
                return BadRequest(new { message = "Please choose an active designer." });

            var oldId = project.DesignerUserId.Value;
            var newName = $"{newDesigner.FirstName} {newDesigner.LastName}";

            project.DesignerUserId = newDesigner.UserId;
            project.ModifiedDate = DateTime.Now;
            project.ModifiedBy = me;

            _db.Set<ProjectStatusHistory>().Add(new ProjectStatusHistory
            {
                ProjectId = projectId,
                Status = project.Status,
                Remarks = $"Reassigned to {newName} by the admin",
                ChangedBy = me.Value,
                CreatedBy = me
            });

            NotificationHelper.Add(_db, newDesigner.UserId, projectId,
                "A project was assigned to you",
                $"{project.ProjectCode} ({project.ProjectName}) is now yours. Current status: {project.Status}.", me);

            NotificationHelper.Add(_db, oldId, projectId,
                "A project was reassigned",
                $"{project.ProjectCode} ({project.ProjectName}) was given to another designer.", me);

            var customerUserId = await _db.Customers
                .Where(c => c.CustomerId == project.CustomerId)
                .Select(c => (long?)c.UserId)
                .FirstOrDefaultAsync();
            if (customerUserId != null)
            {
                NotificationHelper.Add(_db, customerUserId.Value, projectId,
                    "Your designer has changed",
                    $"{newName} is now working on {project.ProjectCode} ({project.ProjectName}).", me);
            }

            await _db.SaveChangesAsync();
            return Ok(new { projectId, designerUserId = newDesigner.UserId, designerName = newName });
        }
    }
}