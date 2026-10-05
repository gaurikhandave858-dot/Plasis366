using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _repository;

        public RolePermissionService(IRolePermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RolePermission>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<RolePermission?> GetByIdAsync(long rolePermissionId)
        {
            return await _repository.GetByIdAsync(rolePermissionId);
        }

        public async Task<RolePermission> CreateAsync(RolePermission rolePermission)
        {
            return await _repository.CreateAsync(rolePermission);
        }

        public async Task<RolePermission> UpdateAsync(RolePermission rolePermission)
        {
            return await _repository.UpdateAsync(rolePermission);
        }

        public async Task<bool> DeleteAsync(long rolePermissionId)
        {
            return await _repository.DeleteAsync(rolePermissionId);
        }
    }
}
