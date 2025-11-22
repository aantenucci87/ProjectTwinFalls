using System;

namespace TwinFalls.Domain.ValueObjects
{
    public sealed class Money : IEquatable<Money>
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be > 0", nameof(amount));
            if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency must be set", nameof(currency));
            Amount = amount;
            Currency = currency;
        }

        public Money Add(Money other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (!string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot add money with different currencies");

            return new Money(Amount + other.Amount, Currency);
        }

        public static Money operator +(Money a, Money b) => a.Add(b);

        public bool Equals(Money? other)
        {
            if (other is null) return false;
            return Amount == other.Amount && string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj) => obj is Money m && Equals(m);
        public override int GetHashCode() => HashCode.Combine(Amount, Currency?.ToUpperInvariant());
        public override string ToString() => $"{Amount} {Currency}";
    }
}
