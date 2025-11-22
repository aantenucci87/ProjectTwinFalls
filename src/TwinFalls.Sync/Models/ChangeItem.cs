using System;
using System.Text.Json;

namespace TwinFalls.Sync.Models
{
    public enum SyncOperation
    {
        Upsert = 0,
        Delete = 1
    }

    public class ChangeItem
    {
        public string EntityType { get; set; } = null!;
        public Guid EntityId { get; set; }
        public SyncOperation Operation { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public JsonElement Payload { get; set; }
    }
}
