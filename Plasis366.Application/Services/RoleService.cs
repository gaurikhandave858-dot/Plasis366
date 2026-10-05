using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role?> GetByIdAsync(long roleId)
        {
            return await _roleRepository.GetByIdAsync(roleId);
        }

        public async Task<Role> CreateAsync(Role role)
        {
            return await _roleRepository.CreateAsync(role);
        }

        public async Task<Role> UpdateAsync(Role role)
        {
            return await _roleRepository.UpdateAsync(role);
        }

        public async Task<bool> DeleteAsync(long roleId)
        {
            return await _roleRepository.DeleteAsync(roleId);
        }
    }
}
