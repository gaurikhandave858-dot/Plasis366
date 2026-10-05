using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;


namespace Plasis366.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(long userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == user.UserId);

            if (existingUser == null)
                return user;

            existingUser.TenantId = user.TenantId;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.UserName = user.UserName;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.ProfileImage = user.ProfileImage;
            existingUser.IsActive = user.IsActive;

            await _context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<bool> DeleteAsync(long userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                return false;

            user.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
