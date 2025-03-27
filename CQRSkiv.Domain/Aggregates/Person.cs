using CQRSkiv.Domain.Events;

namespace CQRSkiv.Domain.Aggregates;

public class Person
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    private readonly List<object> _uncommittedEvents = new();

    public Person() { }

    public Person(Guid id)
    {
        Id = id;
    }

    public IEnumerable<object> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();

    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

    public void Create(Guid id, string name)
    {
        if (Id != Guid.Empty && Id != id)
            throw new InvalidOperationException($"Cannot change Id from {Id} to {id}.");

        var @event = new PersonCreated(id, name);
        Apply(@event);
        _uncommittedEvents.Add(@event);
    }

    public void Update(string name)
    {
        var @event = new PersonUpdated(Id, name);
        Apply(@event);
        _uncommittedEvents.Add(@event);
    }

    public void Delete()
    {
        var @event = new PersonDeleted(Id);
        Apply(@event);
        _uncommittedEvents.Add(@event);
    }

    public void AddEmployment(Guid organizationUnitId, string role)
    {
        var @event = new EmploymentCreated(Id, organizationUnitId, role);
        Apply(@event);
        _uncommittedEvents.Add(@event);
    }

    public void DeleteEmployment(Guid organizationUnitId)
    {
        var @event = new EmploymentDeleted(Id, organizationUnitId);
        Apply(@event);
        _uncommittedEvents.Add(@event);
    }

    public void UpdateEmployment(Guid organizationUnitId, string role)
    {
        var @event = new EmploymentUpdated(Id, organizationUnitId, role);
        Apply(@event);
        _uncommittedEvents.Add(@event);
    }

    public void Apply(PersonCreated @event)
    {
        Id = @event.Id;
        Name = @event.Name;
    }

    public void Apply(PersonUpdated @event)
    {
        Name = @event.Name;
    }

    public void Apply(PersonDeleted @event) { }

    public void Apply(EmploymentCreated @event) { }

    public void Apply(EmploymentDeleted @event) { }

    public void Apply(EmploymentUpdated @event) { }
}
