using CQRSkiv.Application.Commands;
using CQRSkiv.Application.Services;
using CQRSkiv.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CQRSkiv.PublicApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventStoreController : ControllerBase
{
  private readonly IEventStoreQueryService _eventStoreQueryService;

  public EventStoreController(IEventStoreQueryService eventStoreQueryService)
  {
    _eventStoreQueryService = eventStoreQueryService;
  }

  [HttpGet("events/{streamId}")]
  public async Task<IActionResult> GetEvents(Guid streamId, [FromQuery] long? version = null)
  {
    try
    {
      var command = new FetchEventsCommand(streamId, version);
      var events = await _eventStoreQueryService.FetchEventsAsync(command);

      if (events == null || events.Count == 0)
      {
        return NotFound($"No events found for stream {streamId}.");
      }

      return Ok(events);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An error occurred while fetching events: {ex.Message}");
    }
  }
}