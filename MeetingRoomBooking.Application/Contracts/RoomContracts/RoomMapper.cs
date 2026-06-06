using System;
using System.Collections.Generic;
using System.Text;
using MeetingRoomBooking.Domain.Entities;

namespace MeetingRoomBooking.Application.Contracts.RoomContracts
{
    public static class RoomMappingExtensions
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

        public static Room ToEntity(this RoomDto roomDto)
        {
            return new Room
            {
                Name = roomDto.Name,
                Capacity = roomDto.Capacity

            };
        }

    }
}
