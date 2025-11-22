using System;
using System.Threading.Tasks;
using TwinFalls.Domain.Entities;

namespace TwinFalls.Application.Interfaces
{
    public interface IAccountRepository
    {
        Task AddAsync(Account account);
        Task<Account?> GetByIdAsync(Guid id);
        Task UpdateAsync(Account account);
        Task DeleteAsync(Account account);
    }
}
