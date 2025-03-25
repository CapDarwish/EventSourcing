using CQRSkiv.Application.Commands;
using CQRSkiv.Application.Services;
using CQRSkiv.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CQRSkiv.PublicApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrganizationUnitsController : ControllerBase
{
  private readonly IOrganizationUnitService _organizationUnitService;
  private readonly ReadDbContext _dbContext;

  public OrganizationUnitsController(IOrganizationUnitService organizationUnitService, ReadDbContext dbContext)
  {
    _organizationUnitService = organizationUnitService;
    _dbContext = dbContext;
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> Get(Guid id)
  {
    var unit = await _dbContext.OrganizationUnits.FindAsync(id);
    if (unit == null) return NotFound($"OrganizationUnit with Id {id} not found.");
    return Ok(unit);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateOrganizationUnitCommand command)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    try
    {
      await _organizationUnitService.CreateOrganizationUnitAsync(command);
      return CreatedAtAction(nameof(Get), new { id = command.Id }, null);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while creating the organization unit: {ex.Message}");
    }
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganizationUnitCommand command)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    if (id != command.Id) return BadRequest("Id mismatch");

    try
    {
      await _organizationUnitService.UpdateOrganizationUnitAsync(command);
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      if (ex.Message.Contains("not found")) return NotFound(ex.Message);
      return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while updating the organization unit: {ex.Message}");
    }
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var command = new DeleteOrganizationUnitCommand(id);

    try
    {
      await _organizationUnitService.DeleteOrganizationUnitAsync(command);
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      if (ex.Message.Contains("not found")) return NotFound(ex.Message);
      return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while deleting the organization unit: {ex.Message}");
    }
  }
}