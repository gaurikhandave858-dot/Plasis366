using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await _permissionRepository.GetAllAsync();
        }

        public async Task<Permission?> GetByIdAsync(long permissionId)
        {
            return await _permissionRepository.GetByIdAsync(permissionId);
        }

        public async Task<Permission> CreateAsync(Permission permission)
        {
            return await _permissionRepository.CreateAsync(permission);
        }

        public async Task<Permission> UpdateAsync(Permission permission)
        {
            return await _permissionRepository.UpdateAsync(permission);
        }

        public async Task<bool> DeleteAsync(long permissionId)
        {
            return await _permissionRepository.DeleteAsync(permissionId);
        }
    }
}
