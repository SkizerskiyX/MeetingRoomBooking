using System.ComponentModel.DataAnnotations;

namespace MeetingRoomBooking.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public List<Booking> Bookings { get; set; } = new();
    }
}
