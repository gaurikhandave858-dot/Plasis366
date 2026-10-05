using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;
using Microsoft.EntityFrameworkCore;


namespace Plasis366.Infrastructure
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public RolePermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolePermission>> GetAllAsync()
        {
            return await _context.RolePermissions
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<RolePermission?> GetByIdAsync(long rolePermissionId)
        {
            return await _context.RolePermissions
                .FirstOrDefaultAsync(x =>
                    x.RolePermissionId == rolePermissionId &&
                    x.IsActive);
        }

        public async Task<RolePermission> CreateAsync(RolePermission rolePermission)
        {
            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            return rolePermission;
        }

        public async Task<RolePermission> UpdateAsync(RolePermission rolePermission)
        {
            var existingRolePermission =
                await _context.RolePermissions
                .FirstOrDefaultAsync(x =>
                    x.RolePermissionId == rolePermission.RolePermissionId);

            if (existingRolePermission == null)
                return rolePermission;

            existingRolePermission.RoleId = rolePermission.RoleId;
            existingRolePermission.PermissionId = rolePermission.PermissionId;
            existingRolePermission.IsActive = rolePermission.IsActive;

            await _context.SaveChangesAsync();

            return existingRolePermission;
        }

        public async Task<bool> DeleteAsync(long rolePermissionId)
        {
            var rolePermission =
                await _context.RolePermissions
                .FirstOrDefaultAsync(x =>
                    x.RolePermissionId == rolePermissionId);

            if (rolePermission == null)
                return false;

            rolePermission.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
