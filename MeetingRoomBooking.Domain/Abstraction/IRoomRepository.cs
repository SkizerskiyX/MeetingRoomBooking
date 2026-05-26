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
    
    }
}
