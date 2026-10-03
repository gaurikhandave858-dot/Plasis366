using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class UserRole :AuditEntity
    {

        public long UserRoleId { get; set; }

        public long UserId { get; set; }

        public long RoleId { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;

        public Role Role { get; set; } = null!;
    }
}
