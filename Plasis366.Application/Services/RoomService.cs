using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class RoomService :IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _roomRepository.GetAllAsync();
        }

        public async Task<Room?> GetByIdAsync(long roomId)
        {
            return await _roomRepository.GetByIdAsync(roomId);
        }

        public async Task<Room> CreateAsync(Room room)
        {
            return await _roomRepository.CreateAsync(room);
        }

        public async Task<Room> UpdateAsync(Room room)
        {
            return await _roomRepository.UpdateAsync(room);
        }

        public async Task<bool> DeleteAsync(long roomId)
        {
            return await _roomRepository.DeleteAsync(roomId);
        }
    }
}
