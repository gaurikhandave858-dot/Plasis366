using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Application
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllAsync();

        Task<Role?> GetByIdAsync(long roleId);

        Task<Role> CreateAsync(Role role);

        Task<Role> UpdateAsync(Role role);

        Task<bool> DeleteAsync(long roleId);
    }
}
