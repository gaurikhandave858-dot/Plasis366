using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Infrastructure
{
    public interface IUserRoleRepository
    {
        Task<IEnumerable<UserRole>> GetAllAsync();

        Task<UserRole?> GetByIdAsync(long userRoleId);

        Task<UserRole> CreateAsync(UserRole userRole);

        Task<UserRole> UpdateAsync(UserRole userRole);

        Task<bool> DeleteAsync(long userRoleId);
    }
}
