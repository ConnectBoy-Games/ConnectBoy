using System;

namespace ConnectBoy.Core.Entities
{
    public enum TransactionStatus
    {
        Pending,
        Completed,
        Failed,
        Expired
    }

    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = default!; // "Deposit", "Withdrawal", "Payment"
        public string Description { get; set; } = default!;
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

        // The unique token generated for external service initiation
        public string ReferenceToken { get; set; } = Guid.NewGuid().ToString("N");

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public Wallet Wallet { get; set; } = default!;
    }
}