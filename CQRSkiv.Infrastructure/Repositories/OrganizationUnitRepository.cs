using System.Threading.Tasks;
using CQRSkiv.Core.Interfaces;
using CQRSkiv.Domain.Aggregates;
using CQRSkiv.Domain.Events;
using Marten;
using Microsoft.Extensions.Logging;

namespace CQRSkiv.Infrastructure.Repositories;

public class OrganizationUnitRepository : IRepository<OrganizationUnit>
{
    private readonly IDocumentStore _store;
    private readonly ILogger<OrganizationUnitRepository> _logger;

    public OrganizationUnitRepository(
        IDocumentStore session,
        ILogger<OrganizationUnitRepository> logger
    )
    {
        _store = session;
        _logger = logger;
    }

    public async Task<OrganizationUnit> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty.", nameof(id));

        var session = _store.LightweightSession();

        var events = await session.Events.FetchStreamAsync(id);
        if (events == null || events.Count == 0)
            return null;

        var unit = new OrganizationUnit();
        foreach (var @event in events)
        {
            switch (@event.Data)
            {
                case OrganizationUnitCreated created:
                    unit.Apply(created);
                    break;
                case OrganizationUnitUpdated updated:
                    unit.Apply(updated);
                    break;
                case OrganizationUnitDeleted deleted:
                    unit.Apply(deleted);
                    break;
            }
        }

        if (unit.Id == Guid.Empty)
            throw new InvalidOperationException(
                $"OrganizationUnit ID is empty after rehydration for ID {id}."
            );

        return unit;
    }

    public async Task SaveAsync(OrganizationUnit aggregate)
    {
        if (aggregate.Id == Guid.Empty)
            throw new InvalidOperationException("Cannot save OrganizationUnit with empty ID.");

        var events = aggregate.GetUncommittedEvents().ToArray();
        if (events.Length == 0)
            return;

        var session = _store.LightweightSession();
        session.Events.Append(aggregate.Id, events);
        await session.SaveChangesAsync();
        aggregate.ClearUncommittedEvents();
    }

    public async Task DeleteSnapshotAsync(Guid id)
    {
        _logger.LogInformation("Deleting snapshot for OrganizationUnit with ID {Id}", id);
        var session = _store.LightweightSession();
        session.Delete<OrganizationUnit>(id);
        await session.SaveChangesAsync();
    }
}
