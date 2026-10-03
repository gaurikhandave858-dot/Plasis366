using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Domain
{
    public class User : AuditEntity
    {
        public long UserId { get; set; }

        public long TenantId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string? ProfileImage { get; set; }

        // Navigation Property
        public Tenant Tenant { get; set; } = null!;

        // Navigation Property
        public ICollection<UserRole> UserRoles { get; set; }
            = new List<UserRole>();
    }
}
