using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;
using MeetingRoomBooking.Infrastructure.MeetingRoomContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;
        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Booking booking, CancellationToken ct = default)
        {
            await _context.Bookings.AddAsync(booking, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var existingBooking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (existingBooking == null)
            {
                throw new KeyNotFoundException($"Room with id {id} not found");
            }
            _context.Bookings.Remove(existingBooking);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
           return await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id, ct);
        }
        

        public async Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid roomId, CancellationToken ct = default)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(x => x.RoomId == roomId)
                .ToListAsync(ct);
        }

        public async Task<bool> HasConflictAsync(Guid roomId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken ct = default)
        {
            return await _context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                b.StartTime < endTime &&
                b.EndTime > startTime,
                ct);
        }
    }
}
