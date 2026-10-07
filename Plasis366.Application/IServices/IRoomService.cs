using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Application
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllAsync();

        Task<Room?> GetByIdAsync(long roomId);

        Task<Room> CreateAsync(Room room);

        Task<Room> UpdateAsync(Room room);

        Task<bool> DeleteAsync(long roomId);
    }
}
