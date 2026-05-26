using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;
using MeetingRoomBooking.Infrastructure.MeetingRoomContext;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

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
        public async Task AddAsync(Room room, CancellationToken ct)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync(ct);
        }
    }
}
