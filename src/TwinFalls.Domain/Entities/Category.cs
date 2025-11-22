using System;

namespace TwinFalls.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; private set; }

        // Sync metadata
        public DateTime UpdatedAtUtc { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeviceId { get; set; }

        public Category(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
