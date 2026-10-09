using System;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class ProjectStatusHistory : AuditEntity
    {
        public long ProjectStatusHistoryId { get; set; }

        [Range(1, long.MaxValue)]
        public long ProjectId { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        public string Status { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Remarks { get; set; }

        public DateTime StatusDate { get; set; } = DateTime.Now;

        [Range(1, long.MaxValue)]
        public long ChangedBy { get; set; }

        public Project? Project { get; set; }

        public User? ChangedByUser { get; set; }
    }
}
