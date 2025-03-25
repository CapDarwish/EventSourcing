namespace CQRSkiv.Domain.Events;

public record EmploymentDeleted(Guid PersonId, Guid OrganizationUnitId);