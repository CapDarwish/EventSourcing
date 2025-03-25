namespace CQRSkiv.Application.Commands;

public record UpdateOrganizationUnitCommand(Guid Id, string Name, Guid? ParentId);