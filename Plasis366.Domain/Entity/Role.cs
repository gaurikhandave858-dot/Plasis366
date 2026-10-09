using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class Role : AuditEntity
    {
        public long RoleId { get; set; }

        [Range(1, long.MaxValue)]
        public long TenantId { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public string RoleName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public Tenant? Tenant { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
