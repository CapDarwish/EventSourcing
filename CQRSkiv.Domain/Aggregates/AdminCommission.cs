using CQRSkiv.Domain.Events;

namespace CQRSkiv.Domain.Aggregates;

public class AdminCommission
{
  public Guid Id { get; private set; }
  public string Name { get; private set; }
  public Guid ResponsibleOrganizationId { get; private set; }
  private readonly List<object> _uncommittedEvents = new();

  // Parameterless constructor for event sourcing rehydration
  public AdminCommission()
  {
  }

  public AdminCommission(Guid id)
  {
    Id = id;
  }

  public IEnumerable<object> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();
  public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

  public void Create(Guid id, string name, Guid responsibleOrganizationId)
  {
    if (Id != Guid.Empty && Id != id)
    {
      throw new InvalidOperationException($"Cannot change Id from {Id} to {id}.");
    }

    var @event = new AdminCommissionCreated(id, name, responsibleOrganizationId);
    Apply(@event);
    _uncommittedEvents.Add(@event);
  }

  public void Update(string name, Guid responsibleOrganizationId)
  {
    var @event = new AdminCommissionUpdated(Id, name, responsibleOrganizationId);
    Apply(@event);
    _uncommittedEvents.Add(@event);
  }

  public void Delete()
  {
    var @event = new AdminCommissionDeleted(Id);
    Apply(@event);
    _uncommittedEvents.Add(@event);
  }

  public void Apply(AdminCommissionCreated @event)
  {
    Id = @event.Id;
    Name = @event.Name;
    ResponsibleOrganizationId = @event.ResponsibleOrganizationId;
  }

  public void Apply(AdminCommissionUpdated @event)
  {
    Name = @event.Name;
    ResponsibleOrganizationId = @event.ResponsibleOrganizationId;
  }

  public void Apply(AdminCommissionDeleted @event)
  {
    // Mark as deleted (if needed)
  }
}