using System;

namespace ConnectBoy.Core.Entities
{
    public class DeviceToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string FcmToken { get; set; } = default!;
        public string Platform { get; set; } = default!; // "iOS" | "Android"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = default!;
    }
}
