using CQRSkiv.Application.Commands;
using CQRSkiv.Application.Services;
using CQRSkiv.Core.Interfaces;
using CQRSkiv.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CQRSkiv.PublicApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PersonsController : ControllerBase
{
  private readonly IPersonService _personService;
  private readonly ReadDbContext _dbContext;

  public PersonsController(IPersonService personService, ReadDbContext dbContext)
  {
    _personService = personService;
    _dbContext = dbContext;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    try
    {
      var persons = await _dbContext.Persons.ToListAsync();
      return Ok(persons);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while fetching persons: {ex.Message}");
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> Get(Guid id)
  {
    var person = await _dbContext.Persons.FindAsync(id);
    if (person == null) return NotFound($"Person with Id {id} not found.");
    return Ok(person);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreatePersonCommand command)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    try
    {
      await _personService.CreatePersonAsync(command);
      return CreatedAtAction(nameof(Get), new { id = command.Id }, null);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while creating the person: {ex.Message}");
    }
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonCommand command)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    if (id != command.Id) return BadRequest("Id mismatch");

    try
    {
      await _personService.UpdatePersonAsync(command);
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      if (ex.Message.Contains("not found")) return NotFound(ex.Message);
      return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while updating the person: {ex.Message}");
    }
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var command = new DeletePersonCommand(id);

    try
    {
      await _personService.DeletePersonAsync(command);
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      if (ex.Message.Contains("not found")) return NotFound(ex.Message);
      return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while deleting the person: {ex.Message}");
    }
  }

  [HttpPost("{personId}/employment")]
  public async Task<IActionResult> AddEmployment(Guid personId, [FromBody] AddEmploymentCommand command)
  {
    if (personId != command.PersonId)
      return BadRequest("PersonId mismatch");

    try
    {
      await _personService.AddEmploymentAsync(command);
      return Ok();
    }
    catch (InvalidOperationException ex)
    {
      if (ex.Message.Contains("not found")) return NotFound(ex.Message);
      return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while adding employment: {ex.Message}");
    }
  }

  [HttpPut("{personId}/employment/{organizationUnitId}")]
  public async Task<IActionResult> UpdateEmployment(Guid personId, Guid organizationUnitId, [FromBody] UpdateEmploymentCommand command)
  {
    if (personId != command.PersonId || organizationUnitId != command.OrganizationUnitId)
      return BadRequest("PersonId or OrganizationUnitId mismatch");

    try
    {
      await _personService.UpdateEmploymentAsync(command);
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      if (ex.Message.Contains("not found")) return NotFound(ex.Message);
      return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while updating employment: {ex.Message}");
    }
  }

  [HttpDelete("{personId}/employment/{organizationUnitId}")]
  public async Task<IActionResult> DeleteEmployment(Guid personId, Guid organizationUnitId)
  {
    var command = new DeleteEmploymentCommand(personId, organizationUnitId);

    try
    {
      await _personService.DeleteEmploymentAsync(command);
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      if (ex.Message.Contains("not found")) return NotFound(ex.Message);
      return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while deleting employment: {ex.Message}");
    }
  }
}