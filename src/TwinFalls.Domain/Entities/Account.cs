using System;
using TwinFalls.Domain.ValueObjects;

namespace TwinFalls.Domain.Entities
{
    public class Account : BaseEntity
    {
        public string Name { get; private set; }
        public Money Balance { get; private set; }

        // Sync metadata
        public DateTime UpdatedAtUtc { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeviceId { get; set; }

        public Account(string name, Money initialBalance)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Balance = initialBalance ?? throw new ArgumentNullException(nameof(initialBalance));
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void ApplyCredit(Money amount)
        {
            if (amount == null) throw new ArgumentNullException(nameof(amount));
            if (!string.Equals(amount.Currency, Balance.Currency, System.StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Currency mismatch");

            Balance = new Money(Balance.Amount + amount.Amount, Balance.Currency);
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
