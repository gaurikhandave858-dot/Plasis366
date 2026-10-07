using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class Notification : AuditEntity
    {
        public long NotificationId { get; set; }

        public long UserId { get; set; }

        public long? ProjectId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime? ReadDate { get; set; }

        // Navigation Properties
        public User? User { get; set; }

        public Project? Project { get; set; }
    }
}
