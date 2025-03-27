using System.Threading.Tasks;
using CQRSkiv.Application.Commands;
using CQRSkiv.Core.Interfaces;
using CQRSkiv.Domain.Aggregates;
using CQRSkiv.Domain.Events;
using CQRSkiv.Infrastructure.Persistence;
using Marten;
using Microsoft.EntityFrameworkCore;

namespace CQRSkiv.Application.Services;

public interface ITestRepository
{
    Task<PersonProjection> GetByIdAsync(Guid id, long version);
}

public class TestRepository : ITestRepository
{
    private readonly IDocumentStore _store;

    public TestRepository(IDocumentStore store)
    {
        _store = store;
    }

    public async Task<PersonProjection> GetByIdAsync(Guid id, long version)
    {
        var session = _store.LightweightSession();
        PersonProjection? person = await session.Events.AggregateStreamAsync<PersonProjection>(
            id,
            version
        );

        return person is not null ? person : throw new NotFoundException();
    }
}

[Serializable]
public class NotFoundException : Exception
{
    public NotFoundException() { }

    public NotFoundException(string? message)
        : base(message) { }

    public NotFoundException(string? message, Exception? innerException)
        : base(message, innerException) { }
}

public sealed record PersonProjection(Guid Id, string Name, ICollection<Guid> Units)
{
    public static PersonProjection Create(PersonCreated personCommand)
    {
        return new PersonProjection(personCommand.Id, personCommand.Name, []);
    }

    public static PersonProjection Apply(PersonUpdated update, PersonProjection projection) =>
        projection with
        {
            Name = update.Name,
        };

    public static PersonProjection Apply(EmploymentCreated update, PersonProjection projection) =>
        projection with
        {
            Units = [.. projection.Units.Union([update.OrganizationUnitId])],
        };

    public static PersonProjection Apply(EmploymentDeleted update, PersonProjection projection) =>
        projection with
        {
            Units = [.. projection.Units.Where(x => x != update.OrganizationUnitId)],
        };
}
