namespace CQRSkiv.Domain.Events;

public record OrganizationUnitCreated(Guid Id, string Name, Guid? ParentId);