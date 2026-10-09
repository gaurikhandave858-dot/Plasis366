using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class RolePermission : AuditEntity
    {
        public long RolePermissionId { get; set; }

        [Range(1, long.MaxValue)]
        public long RoleId { get; set; }

        [Range(1, long.MaxValue)]
        public long PermissionId { get; set; }

        public Role? Role { get; set; }

        public Permission? Permission { get; set; }
    }
}
