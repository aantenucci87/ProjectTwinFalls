using System;

namespace TwinFalls.Infrastructure.Local.Entities
{
    public enum OutboxOperation
    {
        Upsert = 0,
        Delete = 1
    }

    public class OutboxItem
    {
        public Guid Id { get; set; }
        public string EntityType { get; set; } = null!;
        public Guid EntityId { get; set; }
        public OutboxOperation Operation { get; set; }
        public string PayloadJson { get; set; } = null!;
        public DateTime OccurredAtUtc { get; set; }
        public bool Sent { get; set; }
    }
}
