namespace CQRSkiv.Infrastructure.Entities;

public class AdminCommissionEntity
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public Guid ResponsibleOrganizationId { get; set; }

  // Navigation property for the foreign key
  public OrganizationUnitEntity? ResponsibleOrganization { get; set; }
}