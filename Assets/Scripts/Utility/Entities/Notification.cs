using System;

namespace ConnectBoy.Core.Entities
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RecipientUserId { get; set; }
        public Guid? SenderUserId { get; set; }
        public string Type { get; set; } = default!;  // e.g. "Invite"
        public string Title { get; set; } = default!;
        public string Body { get; set; } = default!;
        public string? Data { get; set; }              // JSON payload
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User Recipient { get; set; } = default!;
        public User? Sender { get; set; }
    }
}
