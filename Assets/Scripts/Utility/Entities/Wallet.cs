using System;
using System.Collections.Generic;

namespace ConnectBoy.Core.Entities
{
    public class Wallet
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public decimal Balance { get; set; } = 0;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = default!;
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
