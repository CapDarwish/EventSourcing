namespace CQRSkiv.Domain.Events;

public record OrganizationUnitUpdated(Guid Id, string Name, Guid? ParentId);