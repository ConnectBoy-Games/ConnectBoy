using System;

namespace ConnectBoy.Core.Entities
{
    public class ExternalLogin
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Provider { get; set; } = default!;   // "Google" | "Apple"
        public string ProviderKey { get; set; } = default!; // subject from the ID token
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = default!;
    }
}
