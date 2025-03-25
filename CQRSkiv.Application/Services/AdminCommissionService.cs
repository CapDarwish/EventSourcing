using CQRSkiv.Application.Commands;
using CQRSkiv.Core.Interfaces;
using CQRSkiv.Domain.Aggregates;
using CQRSkiv.Infrastructure.Persistence;
using Marten;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CQRSkiv.Application.Services;

public class AdminCommissionService : IAdminCommissionService
{
  private readonly IRepository<AdminCommission> _repository;
  private readonly ReadDbContext _dbContext;
  private readonly IDocumentSession _session;

  public AdminCommissionService(IRepository<AdminCommission> repository, ReadDbContext dbContext, IDocumentSession session)
  {
    _repository = repository;
    _dbContext = dbContext;
    _session = session;
  }

  public async Task CreateAdminCommissionAsync(CreateAdminCommissionCommand command)
  {
    if (command.Id == Guid.Empty)
      throw new ArgumentException("ID cannot be empty.", nameof(command.Id));

    // Check if a stream with this ID already exists
    var existingStream = await _session.Events.FetchStreamStateAsync(command.Id);
    if (existingStream != null)
      throw new InvalidOperationException($"An admin commission with Id {command.Id} already exists.");

    // Validate ResponsibleOrganizationId
    if (command.ResponsibleOrganizationId == Guid.Empty)
      throw new InvalidOperationException("ResponsibleOrganizationId cannot be empty.");
    if (await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_dbContext.OrganizationUnits, u => u.Id == command.ResponsibleOrganizationId) == false)
      throw new InvalidOperationException($"ResponsibleOrganizationId {command.ResponsibleOrganizationId} does not exist.");

    var commission = new AdminCommission();
    commission.Create(command.Id, command.Name, command.ResponsibleOrganizationId);

    await _repository.SaveAsync(commission);
  }

  public async Task UpdateAdminCommissionAsync(UpdateAdminCommissionCommand command)
  {
    var commission = await _repository.GetByIdAsync(command.Id)
        ?? throw new InvalidOperationException("Admin commission not found.");

    // Validate ResponsibleOrganizationId
    if (command.ResponsibleOrganizationId == Guid.Empty)
      throw new InvalidOperationException("ResponsibleOrganizationId cannot be empty.");
    if (await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_dbContext.OrganizationUnits, u => u.Id == command.ResponsibleOrganizationId) == false)
      throw new InvalidOperationException($"ResponsibleOrganizationId {command.ResponsibleOrganizationId} does not exist.");

    commission.Update(command.Name, command.ResponsibleOrganizationId);

    await _repository.SaveAsync(commission);
  }

  public async Task DeleteAdminCommissionAsync(DeleteAdminCommissionCommand command)
  {
    var commission = await _repository.GetByIdAsync(command.Id)
        ?? throw new InvalidOperationException("Admin commission not found.");

    commission.Delete();

    await _repository.SaveAsync(commission);
  }
}