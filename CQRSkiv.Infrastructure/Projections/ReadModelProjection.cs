using CQRSkiv.Domain.Aggregates;
using CQRSkiv.Domain.Events;
using CQRSkiv.Infrastructure.Entities;
using CQRSkiv.Infrastructure.Persistence;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CQRSkiv.Infrastructure.Projections;

public class ReadModelProjection : IProjection
{
  public static IServiceProvider ServiceProvider { get; set; }

  public ReadModelProjection()
  {
  }

  public void Apply(IDocumentOperations operations, IReadOnlyList<StreamAction> streams)
  {
    throw new NotSupportedException("Synchronous projection not supported. Use ApplyAsync instead.");
  }

  public async Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<StreamAction> streams, CancellationToken cancellation)
  {
    if (ServiceProvider == null)
    {
      throw new InvalidOperationException("ServiceProvider is not set.");
    }

    using var scope = ServiceProvider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<ReadModelProjection>>();

    logger.LogInformation("Starting ApplyAsync for {StreamCount} streams", streams.Count);

    foreach (var stream in streams)
    {
      logger.LogInformation("Processing stream {StreamId} with {EventCount} events", stream.Id, stream.Events.Count);
      foreach (var @event in stream.Events)
      {
        logger.LogInformation("Processing event {EventType} for stream {StreamId}", @event.EventTypeName, stream.Id);
        try
        {
          await HandleEventAsync(dbContext, @event, logger);
        }
        catch (Exception ex)
        {
          logger.LogError(ex, "Error processing event {EventType} for stream {StreamId}", @event.EventTypeName, stream.Id);
          throw;
        }
      }
    }

    try
    {
      logger.LogInformation("Saving changes to readdb");
      await dbContext.SaveChangesAsync(cancellation);
      logger.LogInformation("Successfully saved changes to readdb");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error saving changes to readdb");
      throw;
    }
  }

  private async Task HandleEventAsync(ReadDbContext dbContext, IEvent @event, ILogger<ReadModelProjection> logger)
  {
    switch (@event.Data)
    {
      // Person Events
      case PersonCreated created:
        logger.LogInformation("Processing PersonCreated event for Person with ID {Id}, Name: {Name}", created.Id, created.Name);
        if (await dbContext.Persons.FindAsync(created.Id) == null)
        {
          dbContext.Persons.Add(new PersonEntity
          {
            Id = created.Id,
            Name = created.Name
          });
          logger.LogInformation("Added Person with ID {Id} to readdb", created.Id);
        }
        else
        {
          logger.LogWarning("Person with ID {Id} already exists in readdb", created.Id);
        }
        break;

      case PersonUpdated updated:
        logger.LogInformation("Processing PersonUpdated event for Person with ID {Id}, Name: {Name}", updated.Id, updated.Name);
        var person = await dbContext.Persons.FindAsync(@event.StreamId);
        if (person != null)
        {
          person.Name = updated.Name;
          logger.LogInformation("Updated Person with ID {Id} in readdb", updated.Id);
        }
        else
        {
          logger.LogWarning("Person with ID {Id} not found in readdb for update", updated.Id);
        }
        break;

      case PersonDeleted:
        logger.LogInformation("Processing PersonDeleted event for Person with ID {Id}", @event.StreamId);
        var personToDelete = await dbContext.Persons.FindAsync(@event.StreamId);
        if (personToDelete != null)
        {
          dbContext.Persons.Remove(personToDelete);
          logger.LogInformation("Deleted Person with ID {Id} from readdb", @event.StreamId);
        }
        else
        {
          logger.LogWarning("Person with ID {Id} not found in readdb for deletion", @event.StreamId);
        }
        break;

      case EmploymentCreated employment:
        logger.LogInformation("Processing EmploymentCreated event for Person {PersonId} and OrganizationUnit {OrganizationUnitId}, Role: {Role}", employment.PersonId, employment.OrganizationUnitId, employment.Role);
        if (await dbContext.Employments.FindAsync(employment.PersonId, employment.OrganizationUnitId) == null)
        {
          dbContext.Employments.Add(new EmploymentEntity
          {
            PersonId = employment.PersonId,
            OrganizationUnitId = employment.OrganizationUnitId,
            Role = employment.Role
          });
          logger.LogInformation("Added Employment for Person {PersonId} and OrganizationUnit {OrganizationUnitId} to readdb", employment.PersonId, employment.OrganizationUnitId);
        }
        else
        {
          logger.LogWarning("Employment for Person {PersonId} and OrganizationUnit {OrganizationUnitId} already exists in readdb", employment.PersonId, employment.OrganizationUnitId);
        }
        break;

      case EmploymentUpdated employment:
        logger.LogInformation("Processing EmploymentUpdated event for Person {PersonId} and OrganizationUnit {OrganizationUnitId}, Role: {Role}", employment.PersonId, employment.OrganizationUnitId, employment.Role);
        var employmentToUpdate = await dbContext.Employments.FindAsync(employment.PersonId, employment.OrganizationUnitId);
        if (employmentToUpdate != null)
        {
          employmentToUpdate.Role = employment.Role;
          logger.LogInformation("Updated Employment for Person {PersonId} and OrganizationUnit {OrganizationUnitId} with Role {Role} in readdb", employment.PersonId, employment.OrganizationUnitId, employment.Role);
        }
        else
        {
          logger.LogWarning("Employment for Person {PersonId} and OrganizationUnit {OrganizationUnitId} not found in readdb for update", employment.PersonId, employment.OrganizationUnitId);
        }
        break;

      case EmploymentDeleted employment:
        logger.LogInformation("Processing EmploymentDeleted event for Person {PersonId} and OrganizationUnit {OrganizationUnitId}", employment.PersonId, employment.OrganizationUnitId);
        var employmentToDelete = await dbContext.Employments.FindAsync(employment.PersonId, employment.OrganizationUnitId);
        if (employmentToDelete != null)
        {
          dbContext.Employments.Remove(employmentToDelete);
          logger.LogInformation("Deleted Employment for Person {PersonId} and OrganizationUnit {OrganizationUnitId} from readdb", employment.PersonId, employment.OrganizationUnitId);
        }
        else
        {
          logger.LogWarning("Employment for Person {PersonId} and OrganizationUnit {OrganizationUnitId} not found in readdb for deletion", employment.PersonId, employment.OrganizationUnitId);
        }
        break;

      // OrganizationUnit Events
      case OrganizationUnitCreated created:
        logger.LogInformation("Processing OrganizationUnitCreated event for OrganizationUnit with ID {Id}, Name: {Name}, ParentId: {ParentId}", created.Id, created.Name, created.ParentId);
        if (await dbContext.OrganizationUnits.FindAsync(created.Id) == null)
        {
          var newUnit = new OrganizationUnitEntity
          {
            Id = created.Id,
            Name = created.Name,
            ParentId = created.ParentId
          };
          dbContext.OrganizationUnits.Add(newUnit);
          logger.LogInformation("Added OrganizationUnit with ID {Id} to readdb", created.Id);
        }
        else
        {
          logger.LogWarning("OrganizationUnit with ID {Id} already exists in readdb", created.Id);
        }
        break;

      case OrganizationUnitUpdated updated:
        logger.LogInformation("Processing OrganizationUnitUpdated event for OrganizationUnit with ID {Id}, Name: {Name}, ParentId: {ParentId}", updated.Id, updated.Name, updated.ParentId);
        var unit = await dbContext.OrganizationUnits.FindAsync(@event.StreamId);
        if (unit != null)
        {
          unit.Name = updated.Name;
          unit.ParentId = updated.ParentId;
          logger.LogInformation("Updated OrganizationUnit with ID {Id} in readdb", updated.Id);
        }
        else
        {
          logger.LogWarning("OrganizationUnit with ID {Id} not found in readdb for update", updated.Id);
        }
        break;

      case OrganizationUnitDeleted:
        logger.LogInformation("Processing OrganizationUnitDeleted event for OrganizationUnit with ID {Id}", @event.StreamId);
        var unitToDelete = await dbContext.OrganizationUnits.FindAsync(@event.StreamId);
        if (unitToDelete != null)
        {
          dbContext.OrganizationUnits.Remove(unitToDelete);
          logger.LogInformation("Deleted OrganizationUnit with ID {Id} from readdb", @event.StreamId);
        }
        else
        {
          logger.LogWarning("OrganizationUnit with ID {Id} not found in readdb for deletion", @event.StreamId);
        }
        break;

      // AdminCommission Events
      case AdminCommissionCreated created:
        logger.LogInformation("Processing AdminCommissionCreated event for AdminCommission with ID {Id}, Name: {Name}, ResponsibleOrganizationId: {ResponsibleOrganizationId}", created.Id, created.Name, created.ResponsibleOrganizationId);
        if (await dbContext.AdminCommissions.FindAsync(created.Id) == null)
        {
          dbContext.AdminCommissions.Add(new AdminCommissionEntity
          {
            Id = created.Id,
            Name = created.Name,
            ResponsibleOrganizationId = created.ResponsibleOrganizationId
          });
          logger.LogInformation("Added AdminCommission with ID {Id} to readdb", created.Id);
        }
        else
        {
          logger.LogWarning("AdminCommission with ID {Id} already exists in readdb", created.Id);
        }
        break;

      case AdminCommissionUpdated updated:
        logger.LogInformation("Processing AdminCommissionUpdated event for AdminCommission with ID {Id}, Name: {Name}, ResponsibleOrganizationId: {ResponsibleOrganizationId}", updated.Id, updated.Name, updated.ResponsibleOrganizationId);
        var commission = await dbContext.AdminCommissions.FindAsync(@event.StreamId);
        if (commission != null)
        {
          commission.Name = updated.Name;
          commission.ResponsibleOrganizationId = updated.ResponsibleOrganizationId;
          logger.LogInformation("Updated AdminCommission with ID {Id} in readdb", updated.Id);
        }
        else
        {
          logger.LogWarning("AdminCommission with ID {Id} not found in readdb for update", updated.Id);
        }
        break;

      case AdminCommissionDeleted:
        logger.LogInformation("Processing AdminCommissionDeleted event for AdminCommission with ID {Id}", @event.StreamId);
        var commissionToDelete = await dbContext.AdminCommissions.FindAsync(@event.StreamId);
        if (commissionToDelete != null)
        {
          dbContext.AdminCommissions.Remove(commissionToDelete);
          logger.LogInformation("Deleted AdminCommission with ID {Id} from readdb", @event.StreamId);
        }
        else
        {
          logger.LogWarning("AdminCommission with ID {Id} not found in readdb for deletion", @event.StreamId);
        }
        break;

      default:
        logger.LogWarning("Unhandled event type {EventType} for stream {StreamId}", @event.EventTypeName, @event.StreamId);
        break;
    }
  }
}