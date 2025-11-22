using System;

namespace TwinFalls.Infrastructure.Local.Entities
{
    public class ProcessedFile
    {
        public string FileName { get; set; } = null!; // PK
        public DateTime ImportedAtUtc { get; set; }
    }
}
