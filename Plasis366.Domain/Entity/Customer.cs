using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class Customer : AuditEntity
    {
        public long CustomerId { get; set; }

        [Range(1, long.MaxValue)]
        public long TenantId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 1)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(30)]
        public string? PhoneNumber { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        public Tenant? Tenant { get; set; }

        public User? User { get; set; }

        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
