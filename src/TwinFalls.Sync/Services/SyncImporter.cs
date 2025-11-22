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
    /// Imports all changes_*.json files from folder and applies them using last-write-wins on UpdatedAtUtc.
    /// Uses ProcessedFiles table to avoid duplicates.
    /// </summary>
    public class SyncImporter
    {
        private readonly TwinFallsDbContext _db;
        private readonly ISyncFolderProvider _folderProvider;

        public SyncImporter(TwinFallsDbContext db, ISyncFolderProvider folderProvider)
        {
            _db = db;
            _folderProvider = folderProvider;
        }

        public async Task<int> ImportAllAsync()
        {
            var folder = _folderProvider.GetFolderPath();
            if (!Directory.Exists(folder)) return 0;

            var files = Directory.GetFiles(folder, "changes_*.json");
            int applied = 0;

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var already = await _db.ProcessedFiles.FindAsync(fileName);
                if (already != null) continue;

                var json = await File.ReadAllTextAsync(file);
                var cs = JsonSerializer.Deserialize<ChangeSet>(json);
                if (cs == null) continue;

                foreach (var item in cs.Changes)
                {
                    // Simple LWW: compare UpdatedAtUtc in payload vs local UpdatedAtUtc
                    // We'll deserialize into a JsonElement and extract UpdatedAtUtc if present
                    try
                    {
                        var payload = item.Payload;
                        var entityType = item.EntityType;
                        var entityId = item.EntityId;

                        if (entityType == nameof(TwinFalls.Domain.Entities.Transaction))
                        {
                            var local = await _db.Transactions.FindAsync(entityId);
                            var incomingUpdated = item.UpdatedAtUtc;
                            if (local == null)
                            {
                                // insert
                                var tx = JsonSerializer.Deserialize<TwinFalls.Domain.Entities.Transaction>(payload.GetRawText());
                                if (tx != null)
                                {
                                    _db.Transactions.Add(tx);
                                    applied++;
                                }
                            }
                            else
                            {
                                if (incomingUpdated > local.UpdatedAtUtc)
                                {
                                    var updated = JsonSerializer.Deserialize<TwinFalls.Domain.Entities.Transaction>(payload.GetRawText());
                                    if (updated != null)
                                    {
                                        // apply fields (simple replace)
                                        _db.Entry(local).CurrentValues.SetValues(updated);
                                        applied++;
                                    }
                                }
                            }
                        }
                        else if (entityType == nameof(TwinFalls.Domain.Entities.Account))
                        {
                            var local = await _db.Accounts.FindAsync(entityId);
                            var incomingUpdated = item.UpdatedAtUtc;
                            if (local == null)
                            {
                                var acc = JsonSerializer.Deserialize<TwinFalls.Domain.Entities.Account>(payload.GetRawText());
                                if (acc != null)
                                {
                                    _db.Accounts.Add(acc);
                                    applied++;
                                }
                            }
                            else
                            {
                                if (incomingUpdated > local.UpdatedAtUtc)
                                {
                                    var updated = JsonSerializer.Deserialize<TwinFalls.Domain.Entities.Account>(payload.GetRawText());
                                    if (updated != null)
                                    {
                                        _db.Entry(local).CurrentValues.SetValues(updated);
                                        applied++;
                                    }
                                }
                            }
                        }
                        else if (entityType == nameof(TwinFalls.Domain.Entities.Category))
                        {
                            var local = await _db.Categories.FindAsync(entityId);
                            var incomingUpdated = item.UpdatedAtUtc;
                            if (local == null)
                            {
                                var cat = JsonSerializer.Deserialize<TwinFalls.Domain.Entities.Category>(payload.GetRawText());
                                if (cat != null)
                                {
                                    _db.Categories.Add(cat);
                                    applied++;
                                }
                            }
                            else
                            {
                                if (incomingUpdated > local.UpdatedAtUtc)
                                {
                                    var updated = JsonSerializer.Deserialize<TwinFalls.Domain.Entities.Category>(payload.GetRawText());
                                    if (updated != null)
                                    {
                                        _db.Entry(local).CurrentValues.SetValues(updated);
                                        applied++;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // swallow to avoid stopping the whole import; logs could be added here
                        continue;
                    }
                }

                // mark file processed
                _db.ProcessedFiles.Add(new TwinFalls.Infrastructure.Local.Entities.ProcessedFile
                {
                    FileName = fileName,
                    ImportedAtUtc = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
            }

            return applied;
        }
    }
}
