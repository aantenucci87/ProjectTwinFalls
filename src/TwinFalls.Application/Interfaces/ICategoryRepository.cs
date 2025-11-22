using System;
using System.Threading.Tasks;
using TwinFalls.Domain.Entities;

namespace TwinFalls.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddAsync(Category category);
        Task<Category?> GetByIdAsync(Guid id);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
