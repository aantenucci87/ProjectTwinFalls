using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwinFalls.Domain.Entities;

namespace TwinFalls.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction tx);
        Task<Transaction?> GetByIdAsync(Guid id);
        Task<IEnumerable<Transaction>> GetByPeriodAsync(DateTime fromUtc, DateTime toUtc);
        Task UpdateAsync(Transaction tx);
        Task DeleteAsync(Transaction tx);
    }
}
