using CQRSkiv.Core.Interfaces;
using CQRSkiv.Domain.Aggregates;
using CQRSkiv.Domain.Events;
using Marten;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CQRSkiv.Infrastructure.Repositories;

public class PersonRepository : IRepository<Person>
{
  private readonly IDocumentSession _session;
  private readonly ILogger<PersonRepository> _logger;

  public PersonRepository(IDocumentSession session, ILogger<PersonRepository> logger)
  {
    _session = session;
    _logger = logger;
  }

  public async Task<Person> GetByIdAsync(Guid id)
  {
    if (id == Guid.Empty)
      throw new ArgumentException("ID cannot be empty.", nameof(id));

    var events = await _session.Events.FetchStreamAsync(id);
    if (events == null || events.Count == 0)
      return null;

    var person = new Person();
    foreach (var @event in events)
    {
      switch (@event.Data)
      {
        case PersonCreated created:
          person.Apply(created);
          break;
        case PersonUpdated updated:
          person.Apply(updated);
          break;
        case PersonDeleted deleted:
          person.Apply(deleted);
          break;
      }
    }

    if (person.Id == Guid.Empty)
      throw new InvalidOperationException($"Person ID is empty after rehydration for ID {id}.");

    return person;
  }

  public async Task SaveAsync(Person aggregate)
  {
    if (aggregate.Id == Guid.Empty)
      throw new InvalidOperationException("Cannot save Person with empty ID.");

    var events = aggregate.GetUncommittedEvents().ToArray();
    if (events.Length == 0)
      return;

    _session.Events.Append(aggregate.Id, events);
    await _session.SaveChangesAsync();
    aggregate.ClearUncommittedEvents();
  }

  public async Task DeleteSnapshotAsync(Guid id)
  {
    _logger.LogInformation("Deleting snapshot for Person with ID {Id}", id);
    _session.Delete<Person>(id);
    await _session.SaveChangesAsync();
  }
}