using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class ProjectAttachment : AuditEntity
    {
        public long ProjectAttachmentId { get; set; }

        [Range(1, long.MaxValue)]
        public long ProjectId { get; set; }

        [Required, StringLength(255, MinimumLength = 1)]
        public string FileName { get; set; } = string.Empty;

        [Required, StringLength(1000, MinimumLength = 1)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FileType { get; set; }

        [Range(1, long.MaxValue)]
        public long? FileSize { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public Project? Project { get; set; }
    }
}
