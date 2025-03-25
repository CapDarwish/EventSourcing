using CQRSkiv.Application.Commands;
using System.Threading.Tasks;
namespace CQRSkiv.Core.Interfaces;
public interface IEventStoreQueryService
{
  Task<IReadOnlyList<object>> FetchEventsAsync(FetchEventsCommand command);
}

