using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;

namespace Plasis366.Domain
{
   public class Tenant :AuditEntity
    {
        public long TenantId { get; set; }

        public string TenantName { get; set; } = string.Empty;

        public string TenantCode { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        // Foreign Key
        public long? CountryId { get; set; }

        // Navigation Property
        public Country? Country { get; set; }
    }
}
