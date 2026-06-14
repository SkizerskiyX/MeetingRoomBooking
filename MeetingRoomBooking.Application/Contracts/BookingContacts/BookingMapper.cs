using MeetingRoomBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeetingRoomBooking.Application.Contracts.BookingContacts
{
    public static class BookingMappingExtensions
    {
        public static BookingResponseDto ToDto(this Booking booking)
        {
            return new BookingResponseDto
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime
            };
        }
        

        public static Booking ToEntity(this BookingDto bookingDto, Guid roomId, string userId)
        {
            return new Booking
            {
                Id = Guid.NewGuid(),
                RoomId = roomId,
                UserId = Guid.Parse(userId),
                StartTime = bookingDto.StartTime,
                EndTime = bookingDto.EndTime
            }; 
        }
    }
}
