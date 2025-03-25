namespace CQRSkiv.Application.Commands;

public record CreateOrganizationUnitCommand(Guid Id, string Name, Guid? ParentId);