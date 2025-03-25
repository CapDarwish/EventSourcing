namespace CQRSkiv.Application.Commands;

public record DeleteEmploymentCommand(Guid PersonId, Guid OrganizationUnitId);