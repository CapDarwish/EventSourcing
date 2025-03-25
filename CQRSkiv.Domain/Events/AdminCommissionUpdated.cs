namespace CQRSkiv.Domain.Events;

public record AdminCommissionUpdated(Guid Id, string Name, Guid ResponsibleOrganizationId);