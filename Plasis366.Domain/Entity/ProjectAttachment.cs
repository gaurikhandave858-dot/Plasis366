using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class ProjectAttachment :AuditEntity
    {
        public long ProjectAttachmentId { get; set; }

        public long ProjectId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string? FileType { get; set; }

        public long? FileSize { get; set; }

        public string? Description { get; set; }

        // Navigation Property
        public Project Project { get; set; } = null!;
    }
}
