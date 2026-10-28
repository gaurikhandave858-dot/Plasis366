using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.API.Controllers
{
    // The studio's library of room templates.
    // Admin: add, edit, hide and show templates with 1 to 5 pictures.
    // Everyone logged in (same studio): browse the visible templates and see their pictures.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TemplatesController : ControllerBase
    {
        private const int MaxImages = 5;
        private const long MaxFileSize = 10 * 1024 * 1024;   // 10 MB

        // Same names as the room types on the project form
        private static readonly string[] RoomTypes =
            { "Living room", "Bedroom", "Kitchen", "Bathroom", "Dining room", "Home office", "Kids room", "Balcony", "Other" };

        // Pictures only (no PDF) for templates
        private static readonly Dictionary<string, string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp"
        };

        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public TemplatesController(ApplicationDbContext db, IConfiguration config, IWebHostEnvironment env)
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
            ".webp" => h.Length >= 12 && h[0] == 0x52 && h[1] == 0x49 && h[2] == 0x46 && h[3] == 0x46
                       && h[8] == 0x57 && h[9] == 0x45 && h[10] == 0x42 && h[11] == 0x50,
            _ => false
        };

        // ---------- Read ----------

        // Visible templates for the customer to pick from (optionally for one room type)
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] string? roomType)
        {
            var tenantId = CurrentTenantId();
            if (tenantId == null) return Unauthorized();

            var query = _db.RoomTemplates.Where(t => t.IsActive && t.TenantId == tenantId);
            if (!string.IsNullOrWhiteSpace(roomType))
                query = query.Where(t => t.RoomType == roomType);

            return Ok(await Shape(query));
        }

        // Every template, including hidden ones (Admin and Manager)
        [HttpGet("all")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> All()
        {
            var tenantId = CurrentTenantId();
            if (tenantId == null) return Unauthorized();

            return Ok(await Shape(_db.RoomTemplates.Where(t => t.TenantId == tenantId)));
        }

        private async Task<List<object>> Shape(IQueryable<RoomTemplate> query)
        {
            var rows = await query
                .OrderBy(t => t.RoomType).ThenBy(t => t.Name)
                .Select(t => new
                {
                    id = t.RoomTemplateId,
                    name = t.Name,
                    roomType = t.RoomType,
                    style = t.Style,
                    description = t.Description,
                    isActive = t.IsActive,
                    images = t.Images
                        .Where(i => i.IsActive)
                        .OrderBy(i => i.SortOrder).ThenBy(i => i.RoomTemplateImageId)
                        .Select(i => new { id = i.RoomTemplateImageId, fileName = i.FileName })
                })
                .ToListAsync();

            return rows.Cast<object>().ToList();
        }

        // A picture of a template. Anyone in the same studio may see it, because the
        // designer must still see the template a customer picked, even if it is hidden later.
        [HttpGet("images/{imageId:long}")]
        public async Task<IActionResult> Image(long imageId)
        {
            var tenantId = CurrentTenantId();
            if (tenantId == null) return Unauthorized();

            var image = await _db.RoomTemplateImages
                .Include(i => i.RoomTemplate)
                .FirstOrDefaultAsync(i => i.RoomTemplateImageId == imageId && i.IsActive);
            if (image?.RoomTemplate == null || image.RoomTemplate.TenantId != tenantId)
                return NotFound();

            var root = Path.GetFullPath(UploadRoot());
            var full = Path.GetFullPath(Path.Combine(root, image.FilePath.Replace('/', Path.DirectorySeparatorChar)));
            if (!full.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                !System.IO.File.Exists(full))
                return NotFound();

            return PhysicalFile(full, image.FileType ?? "application/octet-stream");
        }

        // ---------- Admin: add, edit, hide ----------

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(60 * 1024 * 1024)]
        public async Task<IActionResult> Create(
            [FromForm] string? name, [FromForm] string? roomType,
            [FromForm] string? style, [FromForm] string? description,
            [FromForm] List<IFormFile>? images)
        {
            var tenantId = CurrentTenantId();
            var me = CurrentUserId();
            if (tenantId == null || me == null) return Unauthorized();

            images ??= new List<IFormFile>();
            var problem = ValidateText(ref name, ref roomType, ref style, ref description);
            if (problem != null) return BadRequest(new { message = problem });
            if (images.Count < 1) return BadRequest(new { message = "Please add at least one picture." });
            if (images.Count > MaxImages) return BadRequest(new { message = $"A template can have up to {MaxImages} pictures." });

            var checkedFiles = await CheckImages(images);
            if (checkedFiles.Error != null) return BadRequest(new { message = checkedFiles.Error });

            var template = new RoomTemplate
            {
                TenantId = tenantId.Value,
                Name = name!,
                RoomType = roomType!,
                Style = style,
                Description = description,
                CreatedBy = me
            };
            _db.RoomTemplates.Add(template);
            await _db.SaveChangesAsync();   // gets the id for the folder name

            var written = new List<string>();
            try
            {
                await SaveImages(template, checkedFiles.Valid, written, me.Value, 1);
                await _db.SaveChangesAsync();
            }
            catch
            {
                DeleteFiles(written);
                _db.RoomTemplates.Remove(template);
                await _db.SaveChangesAsync();
                throw;
            }

            return Ok(new { id = template.RoomTemplateId });
        }

        [HttpPut("{id:long}")]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(60 * 1024 * 1024)]
        public async Task<IActionResult> Update(long id,
            [FromForm] string? name, [FromForm] string? roomType,
            [FromForm] string? style, [FromForm] string? description,
            [FromForm] List<IFormFile>? newImages, [FromForm] List<long>? removeImageIds)
        {
            var tenantId = CurrentTenantId();
            var me = CurrentUserId();
            if (tenantId == null || me == null) return Unauthorized();

            newImages ??= new List<IFormFile>();
            removeImageIds ??= new List<long>();

            var template = await _db.RoomTemplates
                .Include(t => t.Images)
                .FirstOrDefaultAsync(t => t.RoomTemplateId == id && t.TenantId == tenantId);
            if (template == null) return NotFound(new { message = "Template not found." });

            var problem = ValidateText(ref name, ref roomType, ref style, ref description);
            if (problem != null) return BadRequest(new { message = problem });

            var active = template.Images.Where(i => i.IsActive).ToList();
            var removing = active.Where(i => removeImageIds.Contains(i.RoomTemplateImageId)).ToList();
            var total = active.Count - removing.Count + newImages.Count;
            if (total < 1) return BadRequest(new { message = "A template needs at least one picture." });
            if (total > MaxImages) return BadRequest(new { message = $"A template can have up to {MaxImages} pictures." });

            var checkedFiles = await CheckImages(newImages);
            if (checkedFiles.Error != null) return BadRequest(new { message = checkedFiles.Error });

            template.Name = name!;
            template.RoomType = roomType!;
            template.Style = style;
            template.Description = description;
            template.ModifiedDate = DateTime.Now;
            template.ModifiedBy = me;

            foreach (var image in removing)
            {
                image.IsActive = false;
                image.ModifiedDate = DateTime.Now;
                image.ModifiedBy = me;
            }

            var next = (active.Count == 0 ? 0 : active.Max(i => i.SortOrder)) + 1;
            var written = new List<string>();
            try
            {
                await SaveImages(template, checkedFiles.Valid, written, me.Value, next);
                await _db.SaveChangesAsync();
            }
            catch
            {
                DeleteFiles(written);
                throw;
            }

            // The removed pictures are no longer needed on disk
            DeleteFiles(removing.Select(i => Path.Combine(UploadRoot(), i.FilePath.Replace('/', Path.DirectorySeparatorChar))));

            return Ok(new { id = template.RoomTemplateId });
        }

        // Hidden templates stay on rooms that already use them, but cannot be picked again
        [HttpPost("{id:long}/hide")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> Hide(long id) => SetVisible(id, false);

        [HttpPost("{id:long}/show")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> Show(long id) => SetVisible(id, true);

        private async Task<IActionResult> SetVisible(long id, bool visible)
        {
            var tenantId = CurrentTenantId();
            var me = CurrentUserId();
            if (tenantId == null || me == null) return Unauthorized();

            var template = await _db.RoomTemplates
                .FirstOrDefaultAsync(t => t.RoomTemplateId == id && t.TenantId == tenantId);
            if (template == null) return NotFound(new { message = "Template not found." });

            template.IsActive = visible;
            template.ModifiedDate = DateTime.Now;
            template.ModifiedBy = me;
            await _db.SaveChangesAsync();

            return Ok(new { id, isActive = visible });
        }

        // ---------- Helpers ----------

        private static string? ValidateText(ref string? name, ref string? roomType, ref string? style, ref string? description)
        {
            name = name?.Trim();
            roomType = roomType?.Trim();
            style = string.IsNullOrWhiteSpace(style) ? null : style.Trim();
            description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            if (string.IsNullOrWhiteSpace(name) || name.Length < 2) return "Please give the template a name (at least 2 letters).";
            if (name.Length > 150) return "The name can be at most 150 characters.";
            if (string.IsNullOrWhiteSpace(roomType) || !RoomTypes.Contains(roomType)) return "Please choose a room type from the list.";
            if (style != null && style.Length > 100) return "The style can be at most 100 characters.";
            if (description != null && description.Length > 1000) return "The description can be at most 1000 characters.";
            return null;
        }

        private async Task<(List<(IFormFile File, string Ext)> Valid, string? Error)> CheckImages(List<IFormFile> files)
        {
            var valid = new List<(IFormFile, string)>();
            foreach (var file in files)
            {
                var displayName = Path.GetFileName(file.FileName);
                var ext = Path.GetExtension(displayName);

                if (file.Length == 0) return (valid, $"{displayName} is empty.");
                if (file.Length > MaxFileSize) return (valid, $"{displayName} is larger than 10 MB.");
                if (!AllowedTypes.ContainsKey(ext)) return (valid, $"{displayName}: only JPG, PNG and WEBP pictures are allowed.");

                var header = new byte[12];
                int read;
                await using (var s = file.OpenReadStream())
                {
                    read = await s.ReadAsync(header, 0, header.Length);
                }
                if (read < header.Length) Array.Resize(ref header, read);
                if (!SignatureMatches(ext, header)) return (valid, $"{displayName} does not look like a valid {ext.ToLowerInvariant()} picture.");

                valid.Add((file, ext));
            }
            return (valid, null);
        }

        private async Task SaveImages(RoomTemplate template, List<(IFormFile File, string Ext)> files,
                                      List<string> written, long userId, int firstOrder)
        {
            if (files.Count == 0) return;

            var folder = Path.Combine(UploadRoot(), "templates", template.RoomTemplateId.ToString());
            Directory.CreateDirectory(folder);

            var order = firstOrder;
            foreach (var (file, ext) in files)
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

                _db.RoomTemplateImages.Add(new RoomTemplateImage
                {
                    RoomTemplateId = template.RoomTemplateId,
                    FileName = displayName,
                    FilePath = $"templates/{template.RoomTemplateId}/{storedName}",
                    FileType = AllowedTypes[ext],
                    FileSize = file.Length,
                    SortOrder = order++,
                    CreatedBy = userId
                });
            }
        }

        private static void DeleteFiles(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                try { System.IO.File.Delete(path); } catch { /* a leftover file is harmless */ }
            }
        }
    }
}