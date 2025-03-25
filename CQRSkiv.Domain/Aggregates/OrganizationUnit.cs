using CQRSkiv.Domain.Events;

namespace CQRSkiv.Domain.Aggregates;

public class OrganizationUnit
{
  public Guid Id { get; private set; }
  public string Name { get; private set; }
  public Guid? ParentId { get; private set; }
  private readonly List<object> _uncommittedEvents = new();

  // Parameterless constructor for event sourcing rehydration
  public OrganizationUnit()
  {
  }

  public OrganizationUnit(Guid id)
  {
    Id = id;
  }

  public IEnumerable<object> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();
  public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

  public void Create(Guid id, string name, Guid? parentId)
  {
    if (Id != Guid.Empty && Id != id)
    {
      throw new InvalidOperationException($"Cannot change Id from {Id} to {id}.");
    }

    var @event = new OrganizationUnitCreated(id, name, parentId);
    Apply(@event);
    _uncommittedEvents.Add(@event);
  }

  public void Update(string name, Guid? parentId)
  {
    var @event = new OrganizationUnitUpdated(Id, name, parentId);
    Apply(@event);
    _uncommittedEvents.Add(@event);
  }

  public void Delete()
  {
    var @event = new OrganizationUnitDeleted(Id);
    Apply(@event);
    _uncommittedEvents.Add(@event);
  }

  public void Apply(OrganizationUnitCreated @event)
  {
    Id = @event.Id;
    Name = @event.Name;
    ParentId = @event.ParentId;
  }

  public void Apply(OrganizationUnitUpdated @event)
  {
    Name = @event.Name;
    ParentId = @event.ParentId;
  }

  public void Apply(OrganizationUnitDeleted @event)
  {
    // Mark as deleted (if needed)
  }
}