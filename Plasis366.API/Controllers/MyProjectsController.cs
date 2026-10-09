using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MyProjectsController : ControllerBase
    {
        private const int MaxFilesPerUpload = 10;
        private const int MaxFilesPerProject = 30;
        private const long MaxFileSize = 10 * 1024 * 1024;   // 10 MB

        // Allowed extension -> the content type we store and serve
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

        public MyProjectsController(ApplicationDbContext db, IConfiguration config, IWebHostEnvironment env)
        {
            _db = db;
            _config = config;
            _env = env;
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

        // Only projects that belong to the logged-in customer
        private IQueryable<Project> OwnedProjects()
        {
            var userId = CurrentUserId();
            return _db.Projects.Where(p => p.IsActive &&
                _db.Customers.Any(c => c.CustomerId == p.CustomerId && c.UserId == userId && c.IsActive));
        }

        private Task<Project?> OwnedProject(long projectId) =>
            OwnedProjects().FirstOrDefaultAsync(p => p.ProjectId == projectId);

        // The project with its property, rooms and requirements, for viewing and editing
        private Task<Project?> LoadOwnedProjectGraph(long projectId) =>
            OwnedProjects()
                .Include(p => p.Properties.Where(x => x.IsActive))
                    .ThenInclude(x => x.Rooms.Where(r => r.IsActive))
                .Include(p => p.DesignRequirements.Where(d => d.IsActive))
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        // Files live outside wwwroot, so they are never served as public static files
        private string UploadRoot() =>
            _config["Uploads:Root"] is { Length: > 0 } custom
                ? custom
                : Path.Combine(_env.ContentRootPath, "Uploads");

        // Checks the first bytes of the file, so a renamed .exe cannot pass as a .jpg
        private static bool SignatureMatches(string ext, byte[] h) => ext.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => h.Length >= 3 && h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF,
            ".png" => h.Length >= 4 && h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47,
            ".pdf" => h.Length >= 4 && h[0] == 0x25 && h[1] == 0x50 && h[2] == 0x44 && h[3] == 0x46,
            ".webp" => h.Length >= 12 && h[0] == 0x52 && h[1] == 0x49 && h[2] == 0x46 && h[3] == 0x46
                       && h[8] == 0x57 && h[9] == 0x45 && h[10] == 0x42 && h[11] == 0x50,
            _ => false
        };

        private static bool HasAny(RequirementInput? r) =>
            r != null && (
                !string.IsNullOrWhiteSpace(r.PreferredStyle) ||
                !string.IsNullOrWhiteSpace(r.PreferredColors) ||
                !string.IsNullOrWhiteSpace(r.FurnitureRequirements) ||
                !string.IsNullOrWhiteSpace(r.StorageRequirements) ||
                !string.IsNullOrWhiteSpace(r.LightingRequirements) ||
                !string.IsNullOrWhiteSpace(r.SpecialRequirements) ||
                !string.IsNullOrWhiteSpace(r.AdditionalNotes));

        // Rules that compare fields. The per-field rules live on the request classes below.
        private IActionResult? Validate(CreateProjectRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ProjectName))
                return BadRequest(new { message = "Project name is required." });
            if (string.IsNullOrWhiteSpace(request.PropertyType))
                return BadRequest(new { message = "Property type is required." });
            if (request.BudgetMin != null && request.BudgetMax != null && request.BudgetMax < request.BudgetMin)
                return BadRequest(new { message = "Maximum budget cannot be less than minimum budget." });
            return null;
        }

        // ---------- Dashboard and project list ----------

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

        // ---------- One project: view, create, edit, delete, submit ----------

        // Everything the edit page needs, in the same shape the form sends back
        [HttpGet("{projectId:long}")]
        public async Task<IActionResult> GetOne(long projectId)
        {
            var project = await LoadOwnedProjectGraph(projectId);
            if (project == null)
                return NotFound(new { message = "Project not found." });

            var property = project.Properties.OrderBy(x => x.PropertyId).FirstOrDefault();
            var requirement = project.DesignRequirements
                .OrderBy(d => d.DesignRequirementId)
                .FirstOrDefault(d => d.RoomId == null);

            return Ok(new
            {
                projectId = project.ProjectId,
                projectCode = project.ProjectCode,
                status = project.Status,
                projectName = project.ProjectName,
                description = project.Description,
                budgetMin = project.BudgetMin,
                budgetMax = project.BudgetMax,
                expectedStartDate = project.ExpectedStartDate,
                propertyType = property?.PropertyType,
                propertyName = property?.PropertyName,
                address = property?.Address,
                regionId = property?.RegionId,
                countryId = property?.CountryId,
                stateId = property?.StateId,
                districtId = property?.DistrictId,
                talukaId = property?.TalukaId,
                cityId = property?.CityId,
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

        // Creates a project (as a Draft) together with its property, rooms and requirements
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectRequest request)
        {
            var userId = CurrentUserId();
            var customer = userId == null ? null : await _db.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);
            if (customer == null)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only customers can create projects." });

            var invalid = Validate(request);
            if (invalid != null) return invalid;

            await using var tx = await _db.Database.BeginTransactionAsync();

            var project = new Project
            {
                TenantId = customer.TenantId,
                CustomerId = customer.CustomerId,
                ProjectName = request.ProjectName.Trim(),
                ProjectCode = "TMP" + Guid.NewGuid().ToString("N")[..8],   // replaced below
                Description = request.Description,
                Status = "Draft",
                BudgetMin = request.BudgetMin,
                BudgetMax = request.BudgetMax,
                ExpectedStartDate = request.ExpectedStartDate,
                CreatedBy = userId
            };

            var rooms = request.Rooms.Where(r => !string.IsNullOrWhiteSpace(r.RoomType)).ToList();

            var property = new Property
            {
                PropertyType = request.PropertyType.Trim(),
                PropertyName = request.PropertyName,
                Address = request.Address,
                RegionId = request.RegionId,
                CountryId = request.CountryId,
                StateId = request.StateId,
                DistrictId = request.DistrictId,
                TalukaId = request.TalukaId,
                CityId = request.CityId,
                TotalArea = request.TotalArea,
                NumberOfFloors = request.NumberOfFloors,
                NumberOfRooms = rooms.Count,
                CreatedBy = userId
            };
            foreach (var r in rooms)
            {
                property.Rooms.Add(new Room
                {
                    RoomType = r.RoomType.Trim(),
                    RoomName = r.RoomName,
                    Length = r.Length,
                    Width = r.Width,
                    Height = r.Height,
                    CreatedBy = userId
                });
            }
            project.Properties.Add(property);

            var req = request.Requirements;
            if (HasAny(req))
            {
                project.DesignRequirements.Add(new DesignRequirement
                {
                    PreferredStyle = req!.PreferredStyle,
                    PreferredColors = req.PreferredColors,
                    FurnitureRequirements = req.FurnitureRequirements,
                    StorageRequirements = req.StorageRequirements,
                    LightingRequirements = req.LightingRequirements,
                    SpecialRequirements = req.SpecialRequirements,
                    AdditionalNotes = req.AdditionalNotes,
                    CreatedBy = userId
                });
            }

            _db.Projects.Add(project);
            _db.Set<ProjectStatusHistory>().Add(new ProjectStatusHistory
            {
                Project = project,
                Status = "Draft",
                Remarks = "Project created",
                ChangedBy = userId!.Value,
                CreatedBy = userId
            });
            await _db.SaveChangesAsync();

            // Now that the id exists, give the project its real code
            project.ProjectCode = $"PRJ-{project.ProjectId:D5}";
            project.ModifiedDate = DateTime.Now;
            await _db.SaveChangesAsync();

            await tx.CommitAsync();
            return Ok(new { projectId = project.ProjectId, projectCode = project.ProjectCode });
        }

        // Saves changes to a draft project
        [HttpPut("{projectId:long}")]
        public async Task<IActionResult> Update(long projectId, CreateProjectRequest request)
        {
            var userId = CurrentUserId();
            var project = await LoadOwnedProjectGraph(projectId);
            if (project == null)
                return NotFound(new { message = "Project not found." });
            if (project.Status != "Draft")
                return BadRequest(new { message = "Only draft projects can be edited." });

            var invalid = Validate(request);
            if (invalid != null) return invalid;

            var now = DateTime.Now;

            project.ProjectName = request.ProjectName.Trim();
            project.Description = request.Description;
            project.BudgetMin = request.BudgetMin;
            project.BudgetMax = request.BudgetMax;
            project.ExpectedStartDate = request.ExpectedStartDate;
            project.ModifiedDate = now;
            project.ModifiedBy = userId;

            // Property
            var property = project.Properties.OrderBy(x => x.PropertyId).FirstOrDefault();
            if (property == null)
            {
                property = new Property { CreatedBy = userId };
                project.Properties.Add(property);
            }
            property.PropertyType = request.PropertyType.Trim();
            property.PropertyName = request.PropertyName;
            property.Address = request.Address;
            property.RegionId = request.RegionId;
            property.CountryId = request.CountryId;
            property.StateId = request.StateId;
            property.DistrictId = request.DistrictId;
            property.TalukaId = request.TalukaId;
            property.CityId = request.CityId;
            property.TotalArea = request.TotalArea;
            property.NumberOfFloors = request.NumberOfFloors;
            property.ModifiedDate = now;
            property.ModifiedBy = userId;

            // Rooms: update the ones that came back with an id, add new ones, hide the ones that were removed
            var incoming = request.Rooms.Where(r => !string.IsNullOrWhiteSpace(r.RoomType)).ToList();
            var keep = incoming.Where(r => r.RoomId != null).Select(r => r.RoomId!.Value).ToHashSet();

            foreach (var gone in property.Rooms.Where(r => r.IsActive && !keep.Contains(r.RoomId)).ToList())
            {
                gone.IsActive = false;
                gone.ModifiedDate = now;
                gone.ModifiedBy = userId;
            }

            foreach (var r in incoming)
            {
                var room = r.RoomId == null
                    ? null
                    : property.Rooms.FirstOrDefault(x => x.RoomId == r.RoomId && x.IsActive);

                if (room == null)
                {
                    room = new Room { CreatedBy = userId };
                    property.Rooms.Add(room);
                }
                else
                {
                    room.ModifiedDate = now;
                    room.ModifiedBy = userId;
                }

                room.RoomType = r.RoomType.Trim();
                room.RoomName = r.RoomName;
                room.Length = r.Length;
                room.Width = r.Width;
                room.Height = r.Height;
            }
            property.NumberOfRooms = incoming.Count;

            // Requirements: one general row for the whole project
            var req = request.Requirements;
            var requirement = project.DesignRequirements
                .OrderBy(d => d.DesignRequirementId)
                .FirstOrDefault(d => d.RoomId == null);

            if (requirement == null && HasAny(req))
            {
                requirement = new DesignRequirement { CreatedBy = userId };
                project.DesignRequirements.Add(requirement);
            }
            if (requirement != null)
            {
                requirement.PreferredStyle = req?.PreferredStyle;
                requirement.PreferredColors = req?.PreferredColors;
                requirement.FurnitureRequirements = req?.FurnitureRequirements;
                requirement.StorageRequirements = req?.StorageRequirements;
                requirement.LightingRequirements = req?.LightingRequirements;
                requirement.SpecialRequirements = req?.SpecialRequirements;
                requirement.AdditionalNotes = req?.AdditionalNotes;
                requirement.ModifiedDate = now;
                requirement.ModifiedBy = userId;
            }

            await _db.SaveChangesAsync();
            return Ok(new { projectId = project.ProjectId });
        }

        // Deletes a draft project. It is hidden, not wiped, so it can still be recovered from the database.
        [HttpDelete("{projectId:long}")]
        public async Task<IActionResult> Delete(long projectId)
        {
            var userId = CurrentUserId();
            var project = await OwnedProject(projectId);
            if (project == null)
                return NotFound(new { message = "Project not found." });
            if (project.Status != "Draft")
                return BadRequest(new { message = "Only draft projects can be deleted." });

            var now = DateTime.Now;
            project.IsActive = false;
            project.ModifiedDate = now;
            project.ModifiedBy = userId;

            var attachments = await _db.ProjectAttachments
                .Where(a => a.ProjectId == projectId && a.IsActive)
                .ToListAsync();
            foreach (var a in attachments)
            {
                a.IsActive = false;
                a.ModifiedDate = now;
                a.ModifiedBy = userId;
            }

            _db.Set<ProjectStatusHistory>().Add(new ProjectStatusHistory
            {
                ProjectId = projectId,
                Status = "Deleted",
                Remarks = "Deleted by customer",
                ChangedBy = userId!.Value,
                CreatedBy = userId
            });
            await _db.SaveChangesAsync();

            // Remove the uploaded files from disk
            var root = Path.GetFullPath(UploadRoot());
            foreach (var a in attachments)
            {
                var fullPath = Path.GetFullPath(Path.Combine(root, a.FilePath));
                if (fullPath.StartsWith(root + Path.DirectorySeparatorChar))
                {
                    try { System.IO.File.Delete(fullPath); } catch { }
                }
            }

            return Ok(new { message = "Project deleted." });
        }

        // Sends a draft project to the studio's designers
        [HttpPost("{projectId:long}/submit")]
        public async Task<IActionResult> Submit(long projectId)
        {
            var userId = CurrentUserId();
            var project = await LoadOwnedProjectGraph(projectId);
            if (project == null)
                return NotFound(new { message = "Project not found." });
            if (project.Status != "Draft")
                return BadRequest(new { message = "Only draft projects can be submitted." });

            // A designer needs at least the basics to start working
            var property = project.Properties.OrderBy(x => x.PropertyId).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(project.ProjectName) ||
                property == null ||
                string.IsNullOrWhiteSpace(property.PropertyType))
            {
                return BadRequest(new { message = "Please add the project name and property details before submitting." });
            }
            if (!property.Rooms.Any(r => r.IsActive))
                return BadRequest(new { message = "Please add at least one room before submitting." });

            project.Status = "Submitted";
            project.ModifiedDate = DateTime.Now;
            project.ModifiedBy = userId;

            _db.Set<ProjectStatusHistory>().Add(new ProjectStatusHistory
            {
                ProjectId = projectId,
                Status = "Submitted",
                Remarks = "Submitted by customer",
                ChangedBy = userId!.Value,
                CreatedBy = userId
            });

            await _db.SaveChangesAsync();
            return Ok(new { projectId, status = project.Status });
        }

        // ---------- Attachments ----------

        [HttpPost("{projectId:long}/attachments")]
        [RequestSizeLimit(110_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 110_000_000)]
        public async Task<IActionResult> UploadAttachments(long projectId, [FromForm] List<IFormFile> files, [FromForm] string? description)
        {
            var project = await OwnedProject(projectId);
            if (project == null)
                return NotFound(new { message = "Project not found." });
            if (project.Status != "Draft")
                return BadRequest(new { message = "Files can only be added while the project is a draft." });

            if (files.Count == 0)
                return BadRequest(new { message = "Please choose at least one file." });
            if (files.Count > MaxFilesPerUpload)
                return BadRequest(new { message = $"You can upload up to {MaxFilesPerUpload} files at a time." });

            var existing = await _db.ProjectAttachments.CountAsync(a => a.ProjectId == projectId && a.IsActive);
            if (existing + files.Count > MaxFilesPerProject)
                return BadRequest(new { message = $"A project can have up to {MaxFilesPerProject} files." });

            // Check every file before writing anything to disk
            var valid = new List<(IFormFile File, string Ext)>();
            foreach (var file in files)
            {
                var displayName = Path.GetFileName(file.FileName);
                var ext = Path.GetExtension(displayName);

                if (file.Length == 0)
                    return BadRequest(new { message = $"{displayName} is empty." });
                if (file.Length > MaxFileSize)
                    return BadRequest(new { message = $"{displayName} is larger than 10 MB." });
                if (!AllowedTypes.ContainsKey(ext))
                    return BadRequest(new { message = $"{displayName}: only JPG, PNG, WEBP and PDF files are allowed." });

                var header = new byte[12];
                int read;
                await using (var stream = file.OpenReadStream())
                {
                    read = await stream.ReadAsync(header, 0, header.Length);
                }
                if (read < header.Length) Array.Resize(ref header, read);

                if (!SignatureMatches(ext, header))
                    return BadRequest(new { message = $"{displayName} does not look like a valid {ext.ToLowerInvariant()} file." });

                valid.Add((file, ext));
            }

            var userId = CurrentUserId();
            var folder = Path.Combine(UploadRoot(), "projects", projectId.ToString());
            Directory.CreateDirectory(folder);

            var written = new List<string>();
            foreach (var (file, ext) in valid)
            {
                // The stored name is random. The customer's own file name is only kept for display.
                var storedName = $"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
                var fullPath = Path.Combine(folder, storedName);

                await using (var fs = new FileStream(fullPath, FileMode.CreateNew))
                {
                    await file.CopyToAsync(fs);
                }
                written.Add(fullPath);

                var displayName = Path.GetFileName(file.FileName);
                if (displayName.Length > 200) displayName = displayName[^200..];

                _db.ProjectAttachments.Add(new ProjectAttachment
                {
                    ProjectId = projectId,
                    FileName = displayName,
                    FilePath = $"projects/{projectId}/{storedName}",
                    FileType = AllowedTypes[ext],
                    FileSize = file.Length,
                    Description = description,
                    CreatedBy = userId
                });
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch
            {
                // Do not leave orphan files behind if the database save fails
                foreach (var path in written)
                {
                    try { System.IO.File.Delete(path); } catch { }
                }
                throw;
            }

            return Ok(new { uploaded = valid.Count });
        }

        [HttpGet("{projectId:long}/attachments")]
        public async Task<IActionResult> ListAttachments(long projectId)
        {
            if (await OwnedProject(projectId) == null)
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

        [HttpGet("{projectId:long}/attachments/{attachmentId:long}/file")]
        public async Task<IActionResult> DownloadAttachment(long projectId, long attachmentId)
        {
            if (await OwnedProject(projectId) == null)
                return NotFound(new { message = "Project not found." });

            var attachment = await _db.ProjectAttachments.FirstOrDefaultAsync(a =>
                a.ProjectAttachmentId == attachmentId && a.ProjectId == projectId && a.IsActive);
            if (attachment == null)
                return NotFound(new { message = "File not found." });

            var root = Path.GetFullPath(UploadRoot());
            var fullPath = Path.GetFullPath(Path.Combine(root, attachment.FilePath));

            // Never read anything outside the uploads folder
            if (!fullPath.StartsWith(root + Path.DirectorySeparatorChar) || !System.IO.File.Exists(fullPath))
                return NotFound(new { message = "File not found." });

            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return PhysicalFile(fullPath, attachment.FileType ?? "application/octet-stream", attachment.FileName);
        }

        [HttpDelete("{projectId:long}/attachments/{attachmentId:long}")]
        public async Task<IActionResult> RemoveAttachment(long projectId, long attachmentId)
        {
            var project = await OwnedProject(projectId);
            if (project == null)
                return NotFound(new { message = "Project not found." });
            if (project.Status != "Draft")
                return BadRequest(new { message = "Files can only be removed while the project is a draft." });

            var attachment = await _db.ProjectAttachments.FirstOrDefaultAsync(a =>
                a.ProjectAttachmentId == attachmentId && a.ProjectId == projectId && a.IsActive);
            if (attachment == null)
                return NotFound(new { message = "File not found." });

            attachment.IsActive = false;
            attachment.ModifiedDate = DateTime.Now;
            attachment.ModifiedBy = CurrentUserId();
            await _db.SaveChangesAsync();

            var root = Path.GetFullPath(UploadRoot());
            var fullPath = Path.GetFullPath(Path.Combine(root, attachment.FilePath));
            if (fullPath.StartsWith(root + Path.DirectorySeparatorChar))
            {
                try { System.IO.File.Delete(fullPath); } catch { }
            }

            return Ok(new { message = "File removed." });
        }

        // ---------- Helpers and request shapes ----------

        private static IQueryable<ProjectRow> Rows(IQueryable<Project> query)
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

        public class CreateProjectRequest
        {
            [Required(ErrorMessage = "Project name is required.")]
            [StringLength(200, MinimumLength = 3, ErrorMessage = "Project name must be 3 to 200 characters.")]
            public string ProjectName { get; set; } = string.Empty;

            [StringLength(2000, ErrorMessage = "Description can be at most 2000 characters.")]
            public string? Description { get; set; }

            [Range(0.0, 100000000000.0, ErrorMessage = "Minimum budget must be between 0 and 100,000,000,000.")]
            public decimal? BudgetMin { get; set; }

            [Range(0.0, 100000000000.0, ErrorMessage = "Maximum budget must be between 0 and 100,000,000,000.")]
            public decimal? BudgetMax { get; set; }

            public DateTime? ExpectedStartDate { get; set; }

            [Required(ErrorMessage = "Property type is required.")]
            [StringLength(50, ErrorMessage = "Property type can be at most 50 characters.")]
            public string PropertyType { get; set; } = string.Empty;

            [StringLength(150, ErrorMessage = "Property name can be at most 150 characters.")]
            public string? PropertyName { get; set; }

            [StringLength(500, ErrorMessage = "Address can be at most 500 characters.")]
            public string? Address { get; set; }

            public long? RegionId { get; set; }
            public long? CountryId { get; set; }
            public long? StateId { get; set; }
            public long? DistrictId { get; set; }
            public long? TalukaId { get; set; }
            public long? CityId { get; set; }

            [Range(1.0, 1000000.0, ErrorMessage = "Total area must be between 1 and 1,000,000 sq ft.")]
            public decimal? TotalArea { get; set; }

            [Range(0, 200, ErrorMessage = "Number of floors must be between 0 and 200.")]
            public int? NumberOfFloors { get; set; }

            [MaxLength(50, ErrorMessage = "A project can have at most 50 rooms.")]
            public List<RoomInput> Rooms { get; set; } = new();

            public RequirementInput? Requirements { get; set; }
        }

        public class RoomInput
        {
            public long? RoomId { get; set; }   // set when editing an existing room

            [Required(ErrorMessage = "Room type is required.")]
            [StringLength(50, ErrorMessage = "Room type can be at most 50 characters.")]
            public string RoomType { get; set; } = string.Empty;

            [StringLength(100, ErrorMessage = "Room name can be at most 100 characters.")]
            public string? RoomName { get; set; }

            [Range(0.01, 1000.0, ErrorMessage = "Room length must be between 0.01 and 1000 ft.")]
            public decimal? Length { get; set; }

            [Range(0.01, 1000.0, ErrorMessage = "Room width must be between 0.01 and 1000 ft.")]
            public decimal? Width { get; set; }

            [Range(0.01, 100.0, ErrorMessage = "Room height must be between 0.01 and 100 ft.")]
            public decimal? Height { get; set; }
        }

        public class RequirementInput
        {
            [StringLength(100, ErrorMessage = "Preferred style can be at most 100 characters.")]
            public string? PreferredStyle { get; set; }

            [StringLength(200, ErrorMessage = "Preferred colors can be at most 200 characters.")]
            public string? PreferredColors { get; set; }

            [StringLength(1000, ErrorMessage = "Furniture requirements can be at most 1000 characters.")]
            public string? FurnitureRequirements { get; set; }

            [StringLength(1000, ErrorMessage = "Storage requirements can be at most 1000 characters.")]
            public string? StorageRequirements { get; set; }

            [StringLength(1000, ErrorMessage = "Lighting requirements can be at most 1000 characters.")]
            public string? LightingRequirements { get; set; }

            [StringLength(1000, ErrorMessage = "Special requirements can be at most 1000 characters.")]
            public string? SpecialRequirements { get; set; }

            [StringLength(2000, ErrorMessage = "Additional notes can be at most 2000 characters.")]
            public string? AdditionalNotes { get; set; }
        }
    }
}