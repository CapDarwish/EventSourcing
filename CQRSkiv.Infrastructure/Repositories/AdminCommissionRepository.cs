using CQRSkiv.Core.Interfaces;
using CQRSkiv.Domain.Aggregates;
using CQRSkiv.Domain.Events;
using Marten;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CQRSkiv.Infrastructure.Repositories;

public class AdminCommissionRepository : IRepository<AdminCommission>
{
  private readonly IDocumentSession _session;
  private readonly ILogger<AdminCommissionRepository> _logger;

  public AdminCommissionRepository(IDocumentSession session, ILogger<AdminCommissionRepository> logger)
  {
    _session = session;
    _logger = logger;
  }

  public async Task<AdminCommission> GetByIdAsync(Guid id)
  {
    if (id == Guid.Empty)
      throw new ArgumentException("ID cannot be empty.", nameof(id));

    var events = await _session.Events.FetchStreamAsync(id);
    if (events == null || events.Count == 0)
      return null;

    var commission = new AdminCommission();
    foreach (var @event in events)
    {
      switch (@event.Data)
      {
        case AdminCommissionCreated created:
          commission.Apply(created);
          break;
        case AdminCommissionUpdated updated:
          commission.Apply(updated);
          break;
        case AdminCommissionDeleted deleted:
          commission.Apply(deleted);
          break;
      }
    }

    if (commission.Id == Guid.Empty)
      throw new InvalidOperationException($"AdminCommission ID is empty after rehydration for ID {id}.");

    return commission;
  }

  public async Task SaveAsync(AdminCommission aggregate)
  {
    if (aggregate.Id == Guid.Empty)
      throw new InvalidOperationException("Cannot save AdminCommission with empty ID.");

    var events = aggregate.GetUncommittedEvents().ToArray();
    if (events.Length == 0)
      return;

    _session.Events.Append(aggregate.Id, events);
    await _session.SaveChangesAsync();
    aggregate.ClearUncommittedEvents();
  }

  public async Task DeleteSnapshotAsync(Guid id)
  {
    _logger.LogInformation("Deleting snapshot for AdminCommission with ID {Id}", id);
    _session.Delete<AdminCommission>(id);
    await _session.SaveChangesAsync();
  }
}