using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plasis366.API;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.API.Controllers
{
    // Design proposals: the designer starts and submits them, the customer approves
    // or asks for a revision. Every action checks that the caller belongs to the project.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProposalsController : ControllerBase
    {
        private const int MaxFilesPerProposal = 10;
        private const long MaxFileSize = 10 * 1024 * 1024;   // 10 MB

        private static readonly Dictionary<string, string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp",
            [".pdf"] = "application/pdf"
        };

        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public ProposalsController(ApplicationDbContext db, IConfiguration config, IWebHostEnvironment env)
        {
            _db = db;
            _config = config;
            _env = env;
        }

        private long? CurrentUserId()
        {
            var value = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(value, out var id) ? id : null;
        }

        private long? CurrentTenantId() =>
            long.TryParse(User.FindFirstValue("tenantId"), out var id) ? id : null;

        private string UploadRoot() =>
            _config["Uploads:Root"] is { Length: > 0 } custom
                ? custom
                : Path.Combine(_env.ContentRootPath, "Uploads");

        private static bool SignatureMatches(string ext, byte[] h) => ext.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => h.Length >= 3 && h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF,
            ".png" => h.Length >= 4 && h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47,
            ".pdf" => h.Length >= 4 && h[0] == 0x25 && h[1] == 0x50 && h[2] == 0x44 && h[3] == 0x46,
            ".webp" => h.Length >= 12 && h[0] == 0x52 && h[1] == 0x49 && h[2] == 0x46 && h[3] == 0x46
                       && h[8] == 0x57 && h[9] == 0x45 && h[10] == 0x42 && h[11] == 0x50,
            _ => false
        };

        // The project, if this user is its assigned designer
        private Task<Project?> MyDesignerProject(long projectId, long userId, long tenantId) =>
            _db.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId && p.IsActive &&
                p.TenantId == tenantId && p.DesignerUserId == userId);

        // The project, if this user is the customer who owns it
        private Task<Project?> MyCustomerProject(long projectId, long userId) =>
            _db.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId && p.IsActive &&
                _db.Customers.Any(c => c.CustomerId == p.CustomerId && c.UserId == userId && c.IsActive));

        private async Task<(Project? Project, string Role)> FindForViewer(long projectId, long userId, long tenantId)
        {
            var asDesigner = await MyDesignerProject(projectId, userId, tenantId);
            if (asDesigner != null) return (asDesigner, "Designer");
            var asCustomer = await MyCustomerProject(projectId, userId);
            return (asCustomer, "Customer");
        }

        private void History(long projectId, string status, string remarks, long userId)
        {
            _db.Set<ProjectStatusHistory>().Add(new ProjectStatusHistory
            {
                ProjectId = projectId,
                Status = status,
                Remarks = remarks,
                ChangedBy = userId,
                CreatedBy = userId
            });
        }

        private Task<long?> CustomerUserIdOf(Project p) =>
            _db.Customers.Where(c => c.CustomerId == p.CustomerId)
                .Select(c => (long?)c.UserId).FirstOrDefaultAsync();

        // ---------- Read ----------

        // All proposals of a project. The designer sees drafts too (there are none today);
        // the customer sees only proposals that were actually submitted.
        [HttpGet("projects/{projectId:long}")]
        public async Task<IActionResult> List(long projectId)
        {
            var userId = CurrentUserId();
            var tenantId = CurrentTenantId();
            if (userId == null || tenantId == null) return Unauthorized();

            var (project, role) = await FindForViewer(projectId, userId.Value, tenantId.Value);
            if (project == null) return NotFound(new { message = "Project not found." });

            var items = await _db.DesignProposals
                .Where(x => x.ProjectId == projectId && x.IsActive && x.Status != "Draft")
                .OrderByDescending(x => x.ProposalVersion)
                .Select(x => new
                {
                    id = x.DesignProposalId,
                    version = x.ProposalVersion,
                    title = x.ProposalTitle,
                    description = x.Description,
                    estimatedCost = x.EstimatedCost,
                    estimatedDays = x.EstimatedDays,
                    status = x.Status,
                    submittedDate = x.SubmittedDate,
                    customerFeedback = x.CustomerFeedback,
                    reviewedDate = x.ReviewedDate,
                    files = x.Files.Where(f => f.IsActive).OrderBy(f => f.DesignProposalFileId).Select(f => new
                    {
                        id = f.DesignProposalFileId,
                        fileName = f.FileName,
                        fileType = f.FileType,
                        fileSize = f.FileSize
                    })
                })
                .ToListAsync();

            return Ok(new { role, projectStatus = project.Status, items });
        }

        [HttpGet("files/{fileId:long}")]
        public async Task<IActionResult> Download(long fileId)
        {
            var userId = CurrentUserId();
            var tenantId = CurrentTenantId();
            if (userId == null || tenantId == null) return Unauthorized();

            var file = await _db.Set<DesignProposalFile>()
                .Include(f => f.DesignProposal)
                .FirstOrDefaultAsync(f => f.DesignProposalFileId == fileId && f.IsActive);
            if (file?.DesignProposal == null) return NotFound();

            var (project, _) = await FindForViewer(file.DesignProposal.ProjectId, userId.Value, tenantId.Value);
            if (project == null || file.DesignProposal.Status == "Draft") return NotFound();

            // FilePath is relative; make sure it cannot escape the upload root
            var root = Path.GetFullPath(UploadRoot());
            var full = Path.GetFullPath(Path.Combine(root, file.FilePath.Replace('/', Path.DirectorySeparatorChar)));
            if (!full.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                !System.IO.File.Exists(full))
                return NotFound();

            return PhysicalFile(full, file.FileType ?? "application/octet-stream", file.FileName);
        }

        // ---------- Designer ----------

        // Under Review (or Revision Requested) -> Design In Progress
        [HttpPost("projects/{projectId:long}/start")]
        [Authorize(Roles = "Designer")]
        public async Task<IActionResult> Start(long projectId)
        {
            var userId = CurrentUserId();
            var tenantId = CurrentTenantId();
            if (userId == null || tenantId == null) return Unauthorized();

            var changed = await _db.Projects
                .Where(p => p.ProjectId == projectId && p.IsActive && p.TenantId == tenantId &&
                            p.DesignerUserId == userId &&
                            (p.Status == "Under Review" || p.Status == "Revision Requested"))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Status, "Design In Progress")
                    .SetProperty(p => p.ModifiedDate, DateTime.Now)
                    .SetProperty(p => p.ModifiedBy, userId));

            if (changed == 0)
                return Conflict(new { message = "You can only start designing a project that is under review or needs a revision." });

            History(projectId, "Design In Progress", "Designer started the design", userId.Value);
            await _db.SaveChangesAsync();
            return Ok(new { projectId, status = "Design In Progress" });
        }

        // Create and submit a proposal in one action (multipart/form-data)
        [HttpPost("projects/{projectId:long}")]
        [Authorize(Roles = "Designer")]
        [RequestSizeLimit(120 * 1024 * 1024)]
        public async Task<IActionResult> Submit(long projectId,
            [FromForm] string? title, [FromForm] string? description,
            [FromForm] decimal? estimatedCost, [FromForm] int? estimatedDays,
            [FromForm] List<IFormFile>? files)
        {
            var userId = CurrentUserId();
            var tenantId = CurrentTenantId();
            if (userId == null || tenantId == null) return Unauthorized();

            title = title?.Trim();
            description = description?.Trim();
            files ??= new List<IFormFile>();

            if (string.IsNullOrWhiteSpace(title)) return BadRequest(new { message = "Please give the proposal a title." });
            if (title.Length > 200) return BadRequest(new { message = "The title can be at most 200 characters." });
            if (string.IsNullOrWhiteSpace(description)) return BadRequest(new { message = "Please add a note for the customer." });
            if (description.Length > 4000) return BadRequest(new { message = "The note can be at most 4000 characters." });
            if (estimatedCost == null || estimatedCost < 0 || estimatedCost > 100000000000m)
                return BadRequest(new { message = "Please enter a valid estimated cost." });
            if (estimatedDays == null || estimatedDays < 1 || estimatedDays > 3650)
                return BadRequest(new { message = "Estimated days must be between 1 and 3650." });
            if (files.Count == 0) return BadRequest(new { message = "Please attach at least one file." });
            if (files.Count > MaxFilesPerProposal)
                return BadRequest(new { message = $"You can attach up to {MaxFilesPerProposal} files." });

            var project = await MyDesignerProject(projectId, userId.Value, tenantId.Value);
            if (project == null) return NotFound(new { message = "Project not found." });
            if (project.Status != "Design In Progress")
                return BadRequest(new { message = "Start the design first, then submit your proposal." });

            // Check every file before touching the disk
            var valid = new List<(IFormFile File, string Ext)>();
            foreach (var file in files)
            {
                var displayName = Path.GetFileName(file.FileName);
                var ext = Path.GetExtension(displayName);
                if (file.Length == 0) return BadRequest(new { message = $"{displayName} is empty." });
                if (file.Length > MaxFileSize) return BadRequest(new { message = $"{displayName} is larger than 10 MB." });
                if (!AllowedTypes.ContainsKey(ext))
                    return BadRequest(new { message = $"{displayName}: only JPG, PNG, WEBP and PDF files are allowed." });

                var header = new byte[12];
                int read;
                await using (var s = file.OpenReadStream())
                {
                    read = await s.ReadAsync(header, 0, header.Length);
                }
                if (read < header.Length) Array.Resize(ref header, read);
                if (!SignatureMatches(ext, header))
                    return BadRequest(new { message = $"{displayName} does not look like a valid {ext.ToLowerInvariant()} file." });

                valid.Add((file, ext));
            }

            var lastVersion = await _db.DesignProposals
                .Where(x => x.ProjectId == projectId)
                .MaxAsync(x => (int?)x.ProposalVersion) ?? 0;

            var proposal = new DesignProposal
            {
                ProjectId = projectId,
                SubmittedBy = userId.Value,
                ProposalVersion = lastVersion + 1,
                ProposalTitle = title,
                Description = description,
                EstimatedCost = estimatedCost,
                EstimatedDays = estimatedDays,
                Status = "Submitted",
                SubmittedDate = DateTime.Now,
                CreatedBy = userId
            };
            _db.DesignProposals.Add(proposal);

            await using var tx = await _db.Database.BeginTransactionAsync();
            var written = new List<string>();
            try
            {
                // Only moves forward if the project is still "Design In Progress"
                var moved = await _db.Projects
                    .Where(p => p.ProjectId == projectId && p.Status == "Design In Progress" &&
                                p.DesignerUserId == userId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.Status, "Design Submitted")
                        .SetProperty(p => p.ModifiedDate, DateTime.Now)
                        .SetProperty(p => p.ModifiedBy, userId));
                if (moved == 0)
                {
                    _db.Entry(proposal).State = EntityState.Detached;
                    return Conflict(new { message = "This project is no longer in progress." });
                }

                await _db.SaveChangesAsync();   // gets the proposal id for the folder name

                var folder = Path.Combine(UploadRoot(), "proposals", projectId.ToString(), proposal.DesignProposalId.ToString());
                Directory.CreateDirectory(folder);

                foreach (var (file, ext) in valid)
                {
                    var storedName = $"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
                    var fullPath = Path.Combine(folder, storedName);
                    await using (var fs = new FileStream(fullPath, FileMode.CreateNew))
                    {
                        await file.CopyToAsync(fs);
                    }
                    written.Add(fullPath);

                    var displayName = Path.GetFileName(file.FileName);
                    if (displayName.Length > 200) displayName = displayName[^200..];

                    _db.Set<DesignProposalFile>().Add(new DesignProposalFile
                    {
                        DesignProposalId = proposal.DesignProposalId,
                        FileName = displayName,
                        FilePath = $"proposals/{projectId}/{proposal.DesignProposalId}/{storedName}",
                        FileType = AllowedTypes[ext],
                        FileSize = file.Length,
                        CreatedBy = userId
                    });
                }

                History(projectId, "Design Submitted", $"Proposal v{proposal.ProposalVersion} submitted", userId.Value);

                var customerUserId = await CustomerUserIdOf(project);
                if (customerUserId != null)
                {
                    NotificationHelper.Add(_db, customerUserId.Value, projectId,
                        "A design proposal is ready",
                        $"Your designer submitted proposal v{proposal.ProposalVersion} for {project.ProjectCode}. Please review it.",
                        userId);
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                foreach (var path in written)
                {
                    try { System.IO.File.Delete(path); } catch { }
                }
                throw;
            }

            return Ok(new { projectId, proposalId = proposal.DesignProposalId, version = proposal.ProposalVersion, status = "Design Submitted" });
        }

        // Approved -> Completed
        [HttpPost("projects/{projectId:long}/complete")]
        [Authorize(Roles = "Designer")]
        public async Task<IActionResult> Complete(long projectId)
        {
            var userId = CurrentUserId();
            var tenantId = CurrentTenantId();
            if (userId == null || tenantId == null) return Unauthorized();

            var changed = await _db.Projects
                .Where(p => p.ProjectId == projectId && p.IsActive && p.TenantId == tenantId &&
                            p.DesignerUserId == userId && p.Status == "Approved")
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Status, "Completed")
                    .SetProperty(p => p.ModifiedDate, DateTime.Now)
                    .SetProperty(p => p.ModifiedBy, userId));

            if (changed == 0)
                return Conflict(new { message = "Only an approved project can be marked as completed." });

            History(projectId, "Completed", "Marked as completed by the designer", userId.Value);

            var project = await _db.Projects.FirstAsync(p => p.ProjectId == projectId);
            var customerUserId = await CustomerUserIdOf(project);
            if (customerUserId != null)
            {
                NotificationHelper.Add(_db, customerUserId.Value, projectId,
                    "Your project is completed",
                    $"{project.ProjectCode} ({project.ProjectName}) has been marked as completed.", userId);
            }

            await _db.SaveChangesAsync();
            return Ok(new { projectId, status = "Completed" });
        }

        // ---------- Customer ----------

        public record RevisionRequest(string? Comment);

        [HttpPost("{proposalId:long}/approve")]
        [Authorize(Roles = "Customer")]
        public Task<IActionResult> Approve(long proposalId) => Review(proposalId, true, null);

        [HttpPost("{proposalId:long}/revision")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RequestRevision(long proposalId, [FromBody] RevisionRequest body)
        {
            var comment = body?.Comment?.Trim();
            if (string.IsNullOrWhiteSpace(comment))
                return BadRequest(new { message = "Please tell the designer what you would like changed." });
            if (comment.Length > 2000)
                return BadRequest(new { message = "The comment can be at most 2000 characters." });
            return await Review(proposalId, false, comment);
        }

        private async Task<IActionResult> Review(long proposalId, bool approve, string? comment)
        {
            var userId = CurrentUserId();
            if (userId == null) return Unauthorized();

            var proposal = await _db.DesignProposals
                .FirstOrDefaultAsync(x => x.DesignProposalId == proposalId && x.IsActive);
            if (proposal == null) return NotFound(new { message = "Proposal not found." });

            var project = await MyCustomerProject(proposal.ProjectId, userId.Value);
            if (project == null) return NotFound(new { message = "Proposal not found." });

            var newStatus = approve ? "Approved" : "Revision Requested";

            await using var tx = await _db.Database.BeginTransactionAsync();

            // Only the project that is waiting for the customer can be answered, once
            var moved = await _db.Projects
                .Where(p => p.ProjectId == project.ProjectId && p.Status == "Design Submitted")
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Status, newStatus)
                    .SetProperty(p => p.ModifiedDate, DateTime.Now)
                    .SetProperty(p => p.ModifiedBy, userId));
            if (moved == 0)
                return Conflict(new { message = "This proposal has already been answered." });

            var answered = await _db.DesignProposals
                .Where(x => x.DesignProposalId == proposalId && x.Status == "Submitted")
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Status, newStatus)
                    .SetProperty(x => x.CustomerFeedback, comment)
                    .SetProperty(x => x.ReviewedDate, DateTime.Now)
                    .SetProperty(x => x.ModifiedDate, DateTime.Now)
                    .SetProperty(x => x.ModifiedBy, userId));
            if (answered == 0)
            {
                await tx.RollbackAsync();
                return Conflict(new { message = "This proposal has already been answered." });
            }

            History(project.ProjectId, newStatus,
                approve ? $"Customer approved proposal v{proposal.ProposalVersion}"
                        : $"Customer requested a revision of proposal v{proposal.ProposalVersion}: {comment}",
                userId.Value);

            if (project.DesignerUserId != null)
            {
                NotificationHelper.Add(_db, project.DesignerUserId.Value, project.ProjectId,
                    approve ? "Your proposal was approved" : "Revision requested",
                    approve ? $"The customer approved proposal v{proposal.ProposalVersion} for {project.ProjectCode}."
                            : $"The customer asked for changes on {project.ProjectCode}: {comment}",
                    userId);
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return Ok(new { projectId = project.ProjectId, status = newStatus });
        }
    }
}