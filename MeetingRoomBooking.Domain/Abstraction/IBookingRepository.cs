using MeetingRoomBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeetingRoomBooking.Domain.Abstraction
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid roomId, CancellationToken ct = default);
        Task AddAsync(Booking booking, CancellationToken ct = default);
        Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<bool> HasConflictAsync(Guid roomId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken ct = default);
        
    }
}