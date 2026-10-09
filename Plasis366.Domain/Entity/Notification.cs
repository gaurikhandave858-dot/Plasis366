using System;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class Notification : AuditEntity
    {
        public long NotificationId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }

        [Range(1, long.MaxValue)]
        public long? ProjectId { get; set; }

        [Required, StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000, MinimumLength = 1)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime? ReadDate { get; set; }

        public User? User { get; set; }

        public Project? Project { get; set; }
    }
}
