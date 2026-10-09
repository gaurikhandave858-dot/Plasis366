using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class User : AuditEntity
    {
        public long UserId { get; set; }

        [Range(1, long.MaxValue)]
        public long TenantId { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 1)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(30)]
        public string? PhoneNumber { get; set; }

        [Required, StringLength(100, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required, StringLength(500, MinimumLength = 1)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ProfileImage { get; set; }

        public Tenant? Tenant { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
