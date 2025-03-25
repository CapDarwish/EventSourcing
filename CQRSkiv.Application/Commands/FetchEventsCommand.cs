namespace CQRSkiv.Application.Commands;

public record FetchEventsCommand(Guid StreamId, long? Version = null);