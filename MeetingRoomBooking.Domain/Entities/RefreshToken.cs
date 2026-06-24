using System.ComponentModel.DataAnnotations;

namespace MeetingRoomBooking.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        [Required]
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
