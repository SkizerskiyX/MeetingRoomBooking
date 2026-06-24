using System.ComponentModel.DataAnnotations;

namespace MeetingRoomBooking.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
        public Room? Room { get; set; }
        public User? User { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
