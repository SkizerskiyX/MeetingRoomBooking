using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;
using MeetingRoomBooking.Infrastructure.MeetingRoomContext;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.Infrastructure.Repositories
{

    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Room>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Rooms.AsNoTracking().ToListAsync(ct);
        }
        public async Task<Room?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Rooms.FirstOrDefaultAsync(x => x.Id == id, ct);
            
        }
        public async Task AddAsync(Room room, CancellationToken ct)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Room room, CancellationToken ct)
        {
            var existingRoom = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == room.Id, ct);
            if (existingRoom == null)
            {
                throw new KeyNotFoundException($"Room with id {room.Id} not found");
            }
            existingRoom.Name = room.Name;
            existingRoom.Capacity = room.Capacity;
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var existingRoom = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (existingRoom == null)
            {
                throw new KeyNotFoundException($"Room with id {id} not found"); 
            }
            _context.Rooms.Remove(existingRoom);
            await _context.SaveChangesAsync(ct);
        }
        
    }
}
