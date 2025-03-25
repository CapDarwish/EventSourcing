using CQRSkiv.Application.Commands;
using CQRSkiv.Core.Interfaces;
using CQRSkiv.Domain.Aggregates;
using CQRSkiv.Infrastructure.Persistence;
using Marten;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CQRSkiv.Application.Services;

public class OrganizationUnitService : IOrganizationUnitService
{
  private readonly IRepository<OrganizationUnit> _repository;
  private readonly ReadDbContext _dbContext;
  private readonly IDocumentSession _session;

  public OrganizationUnitService(IRepository<OrganizationUnit> repository, ReadDbContext dbContext, IDocumentSession session)
  {
    _repository = repository;
    _dbContext = dbContext;
    _session = session;
  }

  public async Task CreateOrganizationUnitAsync(CreateOrganizationUnitCommand command)
  {
    if (command.Id == Guid.Empty)
      throw new ArgumentException("ID cannot be empty.", nameof(command.Id));

    var existingStream = await _session.Events.FetchStreamStateAsync(command.Id);
    if (existingStream != null)
      throw new InvalidOperationException($"An organization unit with Id {command.Id} already exists.");

    if (command.ParentId.HasValue && command.ParentId != Guid.Empty)
    {
      if (await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_dbContext.OrganizationUnits, u => u.Id == command.ParentId) == false)
        throw new InvalidOperationException($"ParentId {command.ParentId} does not exist.");
    }

    var unit = new OrganizationUnit();
    unit.Create(command.Id, command.Name, command.ParentId);

    await _repository.SaveAsync(unit);
  }

  public async Task UpdateOrganizationUnitAsync(UpdateOrganizationUnitCommand command)
  {
    var unit = await _repository.GetByIdAsync(command.Id)
        ?? throw new InvalidOperationException("Organization unit not found.");

    if (command.ParentId.HasValue && command.ParentId != Guid.Empty)
    {
      if (await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_dbContext.OrganizationUnits, u => u.Id == command.ParentId) == false)
        throw new InvalidOperationException($"ParentId {command.ParentId} does not exist.");
    }

    unit.Update(command.Name, command.ParentId);

    await _repository.SaveAsync(unit);
  }

  public async Task DeleteOrganizationUnitAsync(DeleteOrganizationUnitCommand command)
  {
    var unit = await _repository.GetByIdAsync(command.Id)
        ?? throw new InvalidOperationException("Organization unit not found.");

    // Check for dependent AdminCommissions
    if (await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_dbContext.AdminCommissions, ac => ac.ResponsibleOrganizationId == command.Id))
    {
      throw new InvalidOperationException($"Cannot delete OrganizationUnit with Id {command.Id} because it is referenced by AdminCommissions.");
    }

    unit.Delete();

    await _repository.SaveAsync(unit);
  }
}