using CQRSkiv.Application.Commands;
using CQRSkiv.Core.Interfaces;
using Marten;

public class EventStoreQueryService : IEventStoreQueryService
{
  private readonly IDocumentSession _session;

  public EventStoreQueryService(IDocumentSession session)
  {
    _session = session;
  }

  public async Task<IReadOnlyList<object>> FetchEventsAsync(FetchEventsCommand command)
  {
    // Fetch events from the event store up to the specified version (or all if version is null)
    var events = await _session.Events
        .FetchStreamAsync(command.StreamId, command.Version ?? long.MaxValue);

    // Map the events to their data (the actual event payload)
    return events.Select(e => e.Data).ToList().AsReadOnly();
  }
}