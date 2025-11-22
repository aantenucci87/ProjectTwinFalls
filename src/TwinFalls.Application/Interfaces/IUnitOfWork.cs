using System.Threading.Tasks;

namespace TwinFalls.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
