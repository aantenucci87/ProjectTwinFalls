using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TwinFalls.Infrastructure.Local;
using TwinFalls.Domain.ValueObjects;
using TwinFalls.Domain.Entities;
using Xunit;

namespace TwinFalls.Infrastructure.Local.Tests
{
    public class OutboxIntegrationTests
    {
        [Fact]
        public async Task Insert_generates_outbox_item()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            var options = new DbContextOptionsBuilder<TwinFallsDbContext>().UseSqlite(conn).Options;

            using var db = new TwinFallsDbContext(options);
            db.Database.EnsureCreated();

            var acc = new Account("Test", new Money(100, "USD"));
            await db.Accounts.AddAsync(acc);
            await db.SaveChangesAsync();

            var outbox = await db.Outbox.FirstOrDefaultAsync(o => o.EntityId == acc.Id);
            outbox.Should().NotBeNull();
            outbox!.Operation.Should().Be(TwinFalls.Infrastructure.Local.Entities.OutboxOperation.Upsert);
        }
    }
}
