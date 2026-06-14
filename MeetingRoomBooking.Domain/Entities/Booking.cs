using System.ComponentModel.DataAnnotations;

namespace MeetingRoomBooking.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = new Room();
        public Guid UserId { get; set; }
        [Required]
        public User User { get; set; } = new User();
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
