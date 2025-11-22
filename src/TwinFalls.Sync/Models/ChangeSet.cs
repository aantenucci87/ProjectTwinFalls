using System;
using System.Collections.Generic;

namespace TwinFalls.Sync.Models
{
    public class ChangeSet
    {
        public string DeviceId { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
        public List<ChangeItem> Changes { get; set; } = new();
    }
}
