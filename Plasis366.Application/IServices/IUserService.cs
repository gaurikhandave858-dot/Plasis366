using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Application
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User?> GetByIdAsync(long userId);

        Task<User> CreateAsync(User user);

        Task<User> UpdateAsync(User user);

        Task<bool> DeleteAsync(long userId);
    }
}
