using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using TwinFalls.Domain.Entities;
using TwinFalls.Infrastructure.Local.Entities;
using System;

namespace TwinFalls.Infrastructure.Local
{
    public class TwinFallsDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;

        // Outbox and processed files
        public DbSet<OutboxItem> Outbox { get; set; } = null!;
        public DbSet<ProcessedFile> ProcessedFiles { get; set; } = null!;

        public TwinFallsDbContext(DbContextOptions<TwinFallsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired();
                b.OwnsOne(typeof(TwinFalls.Domain.ValueObjects.Money), "Balance");
            });

            modelBuilder.Entity<Category>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired();
            });

            modelBuilder.Entity<Transaction>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Note);
            });

            modelBuilder.Entity<OutboxItem>(b =>
            {
                b.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ProcessedFile>(b =>
            {
                b.HasKey(x => x.FileName);
            });
        }

        public override int SaveChanges()
        {
            CreateOutboxEntries().GetAwaiter().GetResult();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return SaveChangesAsync(true, cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            await CreateOutboxEntries();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private async Task CreateOutboxEntries()
        {
            var now = DateTime.UtcNow;
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Account || e.Entity is Category || e.Entity is Transaction)
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .ToList();

            foreach (var e in entries)
            {
                object entity = e.Entity;
                Guid entityId = (Guid)entity.GetType().GetProperty("Id")!.GetValue(entity)!;
                string entityType = entity.GetType().Name;

                // If deleted -> convert to tombstone: set IsDeleted = true and make sure we capture state
                if (e.State == EntityState.Deleted)
                {
                    // Mark as soft-delete
                    var isDeletedProp = entity.GetType().GetProperty("IsDeleted");
                    if (isDeletedProp != null)
                    {
                        isDeletedProp.SetValue(entity, true);
                        var updatedAtProp = entity.GetType().GetProperty("UpdatedAtUtc");
                        if (updatedAtProp != null) updatedAtProp.SetValue(entity, now);
                        // change state to modified so entity remains in DB as tombstone
                        e.State = EntityState.Modified;
                    }
                }

                // snapshot JSON
                var json = JsonSerializer.Serialize(entity, new JsonSerializerOptions { WriteIndented = false });

                var operation = e.State == EntityState.Deleted ? OutboxOperation.Delete : OutboxOperation.Upsert;

                var outbox = new OutboxItem
                {
                    Id = Guid.NewGuid(),
                    EntityId = entityId,
                    EntityType = entityType,
                    Operation = operation,
                    PayloadJson = json,
                    OccurredAtUtc = now,
                    Sent = false
                };

                // Attach outbox item directly to context
                Outbox.Add(outbox);
            }

            await Task.CompletedTask;
        }
    }
}
