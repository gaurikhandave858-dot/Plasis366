using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class Role : AuditEntity
    {

        public long RoleId { get; set; }

        public long TenantId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Navigation Property
        public Tenant? Tenant { get; set; }

        // Navigation Properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
