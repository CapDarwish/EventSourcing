namespace CQRSkiv.Domain.Events;

public record EmploymentCreated(Guid PersonId, Guid OrganizationUnitId, string Role);