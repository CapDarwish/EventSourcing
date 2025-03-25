using System.Threading.Tasks;

namespace CQRSkiv.Core.Interfaces;

public interface IRepository<T> where T : class
{
  Task<T> GetByIdAsync(Guid id);
  Task SaveAsync(T aggregate);
  Task DeleteSnapshotAsync(Guid id);
}