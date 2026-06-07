using System;
using System.Collections.Generic;
using System.Text;

namespace MeetingRoomBooking.Application.Contracts.BookingContacts
{
    public class BookingResponseDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
