using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class Permission : AuditEntity
    {
        public long PermissionId { get; set; }

        [Required, StringLength(150, MinimumLength = 2)]
        public string PermissionName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
