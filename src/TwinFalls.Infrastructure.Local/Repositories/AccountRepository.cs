using System;
using System.Threading.Tasks;
using TwinFalls.Application.Interfaces;
using TwinFalls.Domain.Entities;

namespace TwinFalls.Infrastructure.Local.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly TwinFallsDbContext _db;

        public AccountRepository(TwinFallsDbContext db) => _db = db;

        public async Task AddAsync(Account account)
        {
            await _db.Accounts.AddAsync(account);
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _db.Accounts.FindAsync(id);
        }

        public Task UpdateAsync(Account account)
        {
            _db.Accounts.Update(account);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Account account)
        {
            _db.Accounts.Remove(account);
            return Task.CompletedTask;
        }
    }
}
