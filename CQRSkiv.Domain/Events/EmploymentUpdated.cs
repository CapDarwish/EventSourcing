namespace CQRSkiv.Domain.Events;

public record EmploymentUpdated(Guid PersonId, Guid OrganizationUnitId, string Role);