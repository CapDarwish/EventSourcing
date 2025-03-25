using CQRSkiv.Application.Commands;
using CQRSkiv.Application.Services;
using CQRSkiv.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CQRSkiv.PublicApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminCommissionsController : ControllerBase
{
  private readonly IAdminCommissionService _adminCommissionService;
  private readonly ReadDbContext _dbContext;

  public AdminCommissionsController(IAdminCommissionService adminCommissionService, ReadDbContext dbContext)
  {
    _adminCommissionService = adminCommissionService;
    _dbContext = dbContext;
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> Get(Guid id)
  {
    var commission = await _dbContext.AdminCommissions.FindAsync(id);
    if (commission == null) return NotFound($"AdminCommission with Id {id} not found.");
    return Ok(commission);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateAdminCommissionCommand command)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    try
    {
      await _adminCommissionService.CreateAdminCommissionAsync(command);
      return CreatedAtAction(nameof(Get), new { id = command.Id }, null);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while creating the admin commission: {ex.Message}");
    }
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdminCommissionCommand command)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    if (id != command.Id) return BadRequest("Id mismatch");

    try
    {
      await _adminCommissionService.UpdateAdminCommissionAsync(command);
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      if (ex.Message.Contains("not found")) return NotFound(ex.Message);
      return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while updating the admin commission: {ex.Message}");
    }
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var command = new DeleteAdminCommissionCommand(id);

    try
    {
      await _adminCommissionService.DeleteAdminCommissionAsync(command);
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      if (ex.Message.Contains("not found")) return NotFound(ex.Message);
      return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while deleting the admin commission: {ex.Message}");
    }
  }
}