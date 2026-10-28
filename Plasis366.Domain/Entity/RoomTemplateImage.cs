using System;
using System.ComponentModel.DataAnnotations;
using Plasis366.Domain;

namespace Plasis366.Domain
{
    // One picture of a room template (a template has 1 to 5)
    public class RoomTemplateImage : AuditEntity
    {
        public long RoomTemplateImageId { get; set; }

        [Range(1, long.MaxValue)]
        public long RoomTemplateId { get; set; }

        [Required, StringLength(200)]
        public string FileName { get; set; } = string.Empty;

        // Relative path under the Uploads folder, for example templates/4/9f3c1a.jpg
        [Required, StringLength(1000)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FileType { get; set; }

        [Range(0, long.MaxValue)]
        public long FileSize { get; set; }

        // 1 is the cover picture
        [Range(1, 100)]
        public int SortOrder { get; set; } = 1;

        public RoomTemplate? RoomTemplate { get; set; }
    }
}