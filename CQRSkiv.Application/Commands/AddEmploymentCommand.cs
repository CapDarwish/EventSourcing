namespace CQRSkiv.Application.Commands;

public record AddEmploymentCommand(Guid PersonId, Guid OrganizationUnitId, string Role);