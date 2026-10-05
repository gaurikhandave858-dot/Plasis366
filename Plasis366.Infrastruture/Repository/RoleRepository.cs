using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(long roleId)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(x =>
                    x.RoleId == roleId &&
                    x.IsActive);
        }

        public async Task<Role> CreateAsync(Role role)
        {
            await _context.Roles.AddAsync(role);

            await _context.SaveChangesAsync();

            return role;
        }

        public async Task<Role> UpdateAsync(Role role)
        {
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(x => x.RoleId == role.RoleId);

            if (existingRole == null)
                return role;

            existingRole.TenantId = role.TenantId;
            existingRole.RoleName = role.RoleName;
            existingRole.Description = role.Description;
            existingRole.IsActive = role.IsActive;

            await _context.SaveChangesAsync();

            return existingRole;
        }

        public async Task<bool> DeleteAsync(long roleId)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(x => x.RoleId == roleId);

            if (role == null)
                return false;

            role.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
