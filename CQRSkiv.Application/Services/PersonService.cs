using CQRSkiv.Application.Commands;
using CQRSkiv.Core.Interfaces;
using CQRSkiv.Domain.Aggregates;
using CQRSkiv.Infrastructure.Persistence;
using Marten;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CQRSkiv.Application.Services;

public class PersonService : IPersonService
{
  private readonly IRepository<Person> _repository;
  private readonly ReadDbContext _dbContext;
  private readonly IDocumentSession _session;

  public PersonService(IRepository<Person> repository, ReadDbContext dbContext, IDocumentSession session)
  {
    _repository = repository;
    _dbContext = dbContext;
    _session = session;
  }

  public async Task CreatePersonAsync(CreatePersonCommand command)
  {
    if (command.Id == Guid.Empty)
      throw new ArgumentException("ID cannot be empty.", nameof(command.Id));

    var existingStream = await _session.Events.FetchStreamStateAsync(command.Id);
    if (existingStream != null)
      throw new InvalidOperationException($"A person with Id {command.Id} already exists.");

    var person = new Person();
    person.Create(command.Id, command.Name);

    await _repository.SaveAsync(person);
  }

  public async Task UpdatePersonAsync(UpdatePersonCommand command)
  {
    var person = await _repository.GetByIdAsync(command.Id)
        ?? throw new InvalidOperationException("Person not found.");

    person.Update(command.Name);

    await _repository.SaveAsync(person);
  }

  public async Task DeletePersonAsync(DeletePersonCommand command)
  {
    var person = await _repository.GetByIdAsync(command.Id)
        ?? throw new InvalidOperationException("Person not found.");

    person.Delete();

    await _repository.SaveAsync(person);
  }

  public async Task AddEmploymentAsync(AddEmploymentCommand command)
  {
    var person = await _repository.GetByIdAsync(command.PersonId)
        ?? throw new InvalidOperationException($"Person with Id {command.PersonId} not found.");

    if (await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_dbContext.OrganizationUnits, u => u.Id == command.OrganizationUnitId) == false)
      throw new InvalidOperationException($"OrganizationUnit with Id {command.OrganizationUnitId} does not exist.");

    person.AddEmployment(command.OrganizationUnitId, command.Role);

    await _repository.SaveAsync(person);
  }

  public async Task DeleteEmploymentAsync(DeleteEmploymentCommand command)
  {
    var person = await _repository.GetByIdAsync(command.PersonId)
        ?? throw new InvalidOperationException($"Person with Id {command.PersonId} not found.");

    if (await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_dbContext.OrganizationUnits, u => u.Id == command.OrganizationUnitId) == false)
      throw new InvalidOperationException($"OrganizationUnit with Id {command.OrganizationUnitId} does not exist.");

    person.DeleteEmployment(command.OrganizationUnitId);

    await _repository.SaveAsync(person);
  }

  public async Task UpdateEmploymentAsync(UpdateEmploymentCommand command)
  {
    var person = await _repository.GetByIdAsync(command.PersonId)
        ?? throw new InvalidOperationException($"Person with Id {command.PersonId} not found.");

    if (await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_dbContext.OrganizationUnits, u => u.Id == command.OrganizationUnitId) == false)
      throw new InvalidOperationException($"OrganizationUnit with Id {command.OrganizationUnitId} does not exist.");

    if (await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_dbContext.Employments, e => e.PersonId == command.PersonId && e.OrganizationUnitId == command.OrganizationUnitId) == false)
      throw new InvalidOperationException($"Employment for Person {command.PersonId} and OrganizationUnit {command.OrganizationUnitId} does not exist.");

    person.UpdateEmployment(command.OrganizationUnitId, command.Role);

    await _repository.SaveAsync(person);
  }
}