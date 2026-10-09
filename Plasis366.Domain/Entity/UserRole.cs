using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class UserRole : AuditEntity
    {
        public long UserRoleId { get; set; }

        [Range(1, long.MaxValue)]
        public long UserId { get; set; }

        [Range(1, long.MaxValue)]
        public long RoleId { get; set; }

        public User? User { get; set; }

        public Role? Role { get; set; }
    }
}
