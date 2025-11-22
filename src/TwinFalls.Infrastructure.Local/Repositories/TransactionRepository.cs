using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TwinFalls.Application.Interfaces;
using TwinFalls.Domain.Entities;

namespace TwinFalls.Infrastructure.Local.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TwinFallsDbContext _db;

        public TransactionRepository(TwinFallsDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Transaction tx)
        {
            await _db.Transactions.AddAsync(tx);
        }

        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            return await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetByPeriodAsync(DateTime fromUtc, DateTime toUtc)
        {
            return await _db.Transactions.Where(t => t.Date >= fromUtc && t.Date <= toUtc && !t.IsDeleted).ToListAsync();
        }

        public Task UpdateAsync(Transaction tx)
        {
            _db.Transactions.Update(tx);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Transaction tx)
        {
            _db.Transactions.Remove(tx);
            return Task.CompletedTask;
        }
    }
}
