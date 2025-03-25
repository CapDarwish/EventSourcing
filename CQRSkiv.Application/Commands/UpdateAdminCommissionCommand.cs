namespace CQRSkiv.Application.Commands;

public record UpdateAdminCommissionCommand(Guid Id, string Name, Guid ResponsibleOrganizationId);