using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class UserRoleService :IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRoleService(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        public async Task<IEnumerable<UserRole>> GetAllAsync()
        {
            return await _userRoleRepository.GetAllAsync();
        }

        public async Task<UserRole?> GetByIdAsync(long userRoleId)
        {
            return await _userRoleRepository.GetByIdAsync(userRoleId);
        }

        public async Task<UserRole> CreateAsync(UserRole userRole)
        {
            return await _userRoleRepository.CreateAsync(userRole);
        }

        public async Task<UserRole> UpdateAsync(UserRole userRole)
        {
            return await _userRoleRepository.UpdateAsync(userRole);
        }

        public async Task<bool> DeleteAsync(long userRoleId)
        {
            return await _userRoleRepository.DeleteAsync(userRoleId);
        }
    }
}
