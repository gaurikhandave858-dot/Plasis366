using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class ProjectStatusHistory : AuditEntity
    {
        public long ProjectStatusHistoryId { get; set; }

        public long ProjectId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public DateTime StatusDate { get; set; } = DateTime.Now;

        public long ChangedBy { get; set; }

        // Navigation Properties
        public Project? Project { get; set; }
        public User? ChangedByUser { get; set; }
    }

}
    
