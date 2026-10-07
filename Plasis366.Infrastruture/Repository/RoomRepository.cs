using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;


namespace Plasis366.Infrastructure
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(long roomId)
        {
            return await _context.Rooms
                .FirstOrDefaultAsync(x =>
                    x.RoomId == roomId &&
                    x.IsActive);
        }

        public async Task<Room> CreateAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);

            await _context.SaveChangesAsync();

            return room;
        }

        public async Task<Room> UpdateAsync(Room room)
        {
            var existingRoom = await _context.Rooms
                .FirstOrDefaultAsync(x =>
                    x.RoomId == room.RoomId);

            if (existingRoom == null)
            {
                return room;
            }

            existingRoom.PropertyId = room.PropertyId;
            existingRoom.RoomType = room.RoomType;
            existingRoom.RoomName = room.RoomName;

            existingRoom.Length = room.Length;
            existingRoom.Width = room.Width;
            existingRoom.Height = room.Height;

            existingRoom.DoorCount = room.DoorCount;
            existingRoom.WindowCount = room.WindowCount;

            existingRoom.Details = room.Details;

            existingRoom.IsActive = room.IsActive;

            await _context.SaveChangesAsync();

            return existingRoom;
        }

        public async Task<bool> DeleteAsync(long roomId)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(x =>
                    x.RoomId == roomId);

            if (room == null)
            {
                return false;
            }

            room.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
