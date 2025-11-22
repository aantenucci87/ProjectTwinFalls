using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TwinFalls.Infrastructure.Local;
using TwinFalls.Sync.Interfaces;
using TwinFalls.Sync.Models;

namespace TwinFalls.Sync.Services
{
    /// <summary>
    /// Exports unsent Outbox items into a changeset file and marks them Sent=true.
    /// </summary>
    public class SyncExporter
    {
        private readonly TwinFallsDbContext _db;
        private readonly ISyncFolderProvider _folderProvider;
        private readonly IDeviceIdProvider _deviceIdProvider;

        public SyncExporter(TwinFallsDbContext db, ISyncFolderProvider folderProvider, IDeviceIdProvider deviceIdProvider)
        {
            _db = db;
            _folderProvider = folderProvider;
            _deviceIdProvider = deviceIdProvider;
        }

        public async Task<string> ExportAsync()
        {
            var outbox = await _db.Outbox.Where(o => !o.Sent).ToListAsync();
            if (!outbox.Any()) return string.Empty;

            var changeSet = new ChangeSet
            {
                DeviceId = _deviceIdProvider.GetDeviceId(),
                CreatedAtUtc = DateTime.UtcNow,
                Changes = outbox.Select(o => new ChangeItem
                {
                    EntityType = o.EntityType,
                    EntityId = o.EntityId,
                    Operation = o.Operation == TwinFalls.Infrastructure.Local.Entities.OutboxOperation.Delete ? Sync.Models.SyncOperation.Delete : Sync.Models.SyncOperation.Upsert,
                    UpdatedAtUtc = o.OccurredAtUtc,
                    Payload = JsonSerializer.Deserialize<JsonElement>(o.PayloadJson)
                }).ToList()
            };

            var folder = _folderProvider.GetFolderPath();
            Directory.CreateDirectory(folder);

            var fileName = $"changes_{changeSet.DeviceId}_{DateTime.UtcNow:yyyyMMddHHmmssfff}.json";
            var fullPath = Path.Combine(folder, fileName);
            var json = JsonSerializer.Serialize(changeSet, new JsonSerializerOptions { WriteIndented = false });
            await File.WriteAllTextAsync(fullPath, json);

            // mark outbox as sent
            foreach (var o in outbox)
            {
                o.Sent = true;
            }

            await _db.SaveChangesAsync();
            return fullPath;
        }
    }
}
