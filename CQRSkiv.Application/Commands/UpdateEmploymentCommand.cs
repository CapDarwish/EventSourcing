namespace CQRSkiv.Application.Commands;

public record UpdateEmploymentCommand(Guid PersonId, Guid OrganizationUnitId, string Role);