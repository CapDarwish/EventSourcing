namespace CQRSkiv.Domain.Events;

public record AdminCommissionCreated(Guid Id, string Name, Guid ResponsibleOrganizationId);