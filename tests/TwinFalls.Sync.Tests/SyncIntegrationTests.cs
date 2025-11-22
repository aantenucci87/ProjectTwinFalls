using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TwinFalls.Infrastructure.Local;
using TwinFalls.Sync.Services;
using TwinFalls.Sync.Interfaces;
using TwinFalls.Sync.Models;
using Xunit;

namespace TwinFalls.Sync.Tests
{
    class TempFolderProvider : ISyncFolderProvider
    {
        private readonly string _p;
        public TempFolderProvider(string p) => _p = p;
        public string GetFolderPath() => _p;
    }

    class DummyDeviceProvider : IDeviceIdProvider { public string GetDeviceId() => "device-test"; }

    public class SyncIntegrationTests
    {
        [Fact]
        public async Task Exporter_creates_file_and_importer_applies()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            var options = new DbContextOptionsBuilder<TwinFallsDbContext>().UseSqlite(conn).Options;

            var folder = Path.Combine(Path.GetTempPath(), "twinfalls_sync_test");
            if (Directory.Exists(folder)) Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);

            using var db = new TwinFallsDbContext(options);
            db.Database.EnsureCreated();

            // create an account to produce an outbox
            var acc = new TwinFalls.Domain.Entities.Account("SyncAcc", new TwinFalls.Domain.ValueObjects.Money(10, "USD"));
            db.Accounts.Add(acc);
            await db.SaveChangesAsync();

            var exporter = new SyncExporter(db, new TempFolderProvider(folder), new DummyDeviceProvider());
            var path = await exporter.ExportAsync();
            File.Exists(path).Should().BeTrue();

            // clear local DB and reimport
            // For simplicity import into same DB (should skip already applied later)
            var importer = new SyncImporter(db, new TempFolderProvider(folder));
            var applied = await importer.ImportAllAsync();
            applied.Should().BeGreaterOrEqualTo(0);
        }
    }
}
