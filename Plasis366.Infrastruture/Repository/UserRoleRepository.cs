using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserRole>> GetAllAsync()
        {
            return await _context.UserRoles
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<UserRole?> GetByIdAsync(long userRoleId)
        {
            return await _context.UserRoles
                .FirstOrDefaultAsync(x =>
                    x.UserRoleId == userRoleId &&
                    x.IsActive);
        }

        public async Task<UserRole> CreateAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);

            await _context.SaveChangesAsync();

            return userRole;
        }

        public async Task<UserRole> UpdateAsync(UserRole userRole)
        {
            var existingUserRole = await _context.UserRoles
                .FirstOrDefaultAsync(x =>
                    x.UserRoleId == userRole.UserRoleId);

            if (existingUserRole == null)
                return userRole;

            existingUserRole.UserId = userRole.UserId;
            existingUserRole.RoleId = userRole.RoleId;
            existingUserRole.IsActive = userRole.IsActive;

            await _context.SaveChangesAsync();

            return existingUserRole;
        }

        public async Task<bool> DeleteAsync(long userRoleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(x =>
                    x.UserRoleId == userRoleId);

            if (userRole == null)
                return false;

            userRole.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
