using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class RolePermission : AuditEntity
    {
        public long RolePermissionId { get; set; }

        public long RoleId { get; set; }

        public long PermissionId { get; set; }

        // Navigation Properties
        public Role? Role { get; set; }

        public Permission? Permission { get; set; }
    }
}
