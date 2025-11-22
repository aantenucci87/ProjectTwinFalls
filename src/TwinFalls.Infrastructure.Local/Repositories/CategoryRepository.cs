using System;
using System.Threading.Tasks;
using TwinFalls.Application.Interfaces;
using TwinFalls.Domain.Entities;

namespace TwinFalls.Infrastructure.Local.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TwinFallsDbContext _db;

        public CategoryRepository(TwinFallsDbContext db) => _db = db;

        public async Task AddAsync(Category category)
        {
            await _db.Categories.AddAsync(category);
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _db.Categories.FindAsync(id);
        }

        public Task UpdateAsync(Category category)
        {
            _db.Categories.Update(category);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Category category)
        {
            _db.Categories.Remove(category);
            return Task.CompletedTask;
        }
    }
}
