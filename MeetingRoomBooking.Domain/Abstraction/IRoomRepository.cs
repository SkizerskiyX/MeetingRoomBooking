using MeetingRoomBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeetingRoomBooking.Domain.Abstraction
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(Room room, CancellationToken ct = default);
        Task<Room?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task UpdateAsync(Room room, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);

    }
}
