namespace CQRSkiv.Application.Commands;

public record CreateAdminCommissionCommand(Guid Id, string Name, Guid ResponsibleOrganizationId);