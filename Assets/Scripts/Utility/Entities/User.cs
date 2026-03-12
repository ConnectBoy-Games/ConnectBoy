using System;
using System.Collections.Generic;

namespace ConnectBoy.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = default!;
        public string? PasswordHash { get; set; }
        public string Username { get; set; } = default!;
        public string? DisplayName { get; set; }
        public bool IsBlocked { get; set; } = false;
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Wallet? Wallet { get; set; }
        public ICollection<ExternalLogin> ExternalLogins { get; set; } = new List<ExternalLogin>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<DeviceToken> DeviceTokens { get; set; } = new List<DeviceToken>();
        public ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();
        public ICollection<Notification> SentNotifications { get; set; } = new List<Notification>();
    }
}