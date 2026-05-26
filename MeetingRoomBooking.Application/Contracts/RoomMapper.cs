using System;
using System.Collections.Generic;
using System.Text;
using MeetingRoomBooking.Domain.Entities;

namespace MeetingRoomBooking.Application.Contracts
{
    public static class RoomMapper
    {
        public static RoomDto ToDto(this Room room)
        {
            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity
            };
        }
    }
}
