using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
     public class Customer:AuditEntity
    {
        public long CustomerId { get; set; }

        public long TenantId { get; set; }

        public long UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        // Navigation Properties
        public Tenant? Tenant { get; set; }
        public User? User { get; set; }

        // Navigation Property
        public ICollection<Project> Projects { get; set; }
            = new List<Project>();
    }
}
