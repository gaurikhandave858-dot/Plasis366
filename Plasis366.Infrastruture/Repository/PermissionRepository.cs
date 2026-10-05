using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class PermissionRepository :IPermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public PermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await _context.Permissions
                .Where(x => x.IsActive)
                .ToListAsync();          //LINQ
        }

        public async Task<Permission?> GetByIdAsync(long permissionId)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(x =>
                    x.PermissionId == permissionId &&
                    x.IsActive);
        }

        public async Task<Permission> CreateAsync(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);

            await _context.SaveChangesAsync();

            return permission;
        }

        public async Task<Permission> UpdateAsync(Permission permission)
        {
            var existingPermission = await _context.Permissions
                .FirstOrDefaultAsync(x =>
                    x.PermissionId == permission.PermissionId);

            if (existingPermission == null)
                return permission;

            existingPermission.PermissionName = permission.PermissionName;
            existingPermission.Description = permission.Description;
            existingPermission.IsActive = permission.IsActive;

            await _context.SaveChangesAsync();

            return existingPermission;
        }

        public async Task<bool> DeleteAsync(long permissionId)
        {
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(x =>
                    x.PermissionId == permissionId);

            if (permission == null)
                return false;

            permission.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
