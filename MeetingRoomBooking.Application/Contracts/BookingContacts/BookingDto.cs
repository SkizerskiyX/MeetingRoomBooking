using System;
using System.Collections.Generic;
using System.Text;

namespace MeetingRoomBooking.Application.Contracts.BookingContacts
{
    public class BookingDto
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
