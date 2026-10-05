using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Application
{
    public interface IRolePermissionService
    {
        Task<IEnumerable<RolePermission>> GetAllAsync();

        Task<RolePermission?> GetByIdAsync(long rolePermissionId);

        Task<RolePermission> CreateAsync(RolePermission rolePermission);

        Task<RolePermission> UpdateAsync(RolePermission rolePermission);

        Task<bool> DeleteAsync(long rolePermissionId);
    }
}
