using System.Threading.Tasks;
using TwinFalls.Application.Interfaces;

namespace TwinFalls.Infrastructure.Local.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TwinFallsDbContext _db;
        public UnitOfWork(TwinFallsDbContext db) => _db = db;

        public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
