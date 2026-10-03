using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class Permission:AuditEntity
    {
        public long PermissionId { get; set; }

        public string PermissionName { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Navigation Property
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
