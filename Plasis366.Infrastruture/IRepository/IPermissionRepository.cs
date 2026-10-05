using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Infrastructure
{
    public interface IPermissionRepository
    {

        Task<IEnumerable<Permission>> GetAllAsync();

        Task<Permission?> GetByIdAsync(long permissionId);

        Task<Permission> CreateAsync(Permission permission);

        Task<Permission> UpdateAsync(Permission permission);

        Task<bool> DeleteAsync(long permissionId);
    }
}
