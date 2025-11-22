using System;
using TwinFalls.Domain.Enums;
using TwinFalls.Domain.ValueObjects;

namespace TwinFalls.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid AccountId { get; private set; }
        public Money Value { get; private set; }
        public DateTime Date { get; private set; }
        public string? Note { get; private set; }
        public Guid? CategoryId { get; private set; }
        public TransactionType Type { get; private set; }

        // Sync metadata
        public DateTime UpdatedAtUtc { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeviceId { get; set; }

        public Transaction(Guid accountId, Money value, DateTime date, TransactionType type, Guid? categoryId = null, string? note = null)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            AccountId = accountId;
            Value = value;
            Date = date;
            Type = type;
            CategoryId = categoryId;
            Note = note;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void UpdateValue(Money newValue)
        {
            if (newValue == null) throw new ArgumentNullException(nameof(newValue));
            if (!string.Equals(newValue.Currency, Value.Currency, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Currency mismatch");

            Value = newValue;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
