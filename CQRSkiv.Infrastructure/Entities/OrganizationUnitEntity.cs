using System;
using System.Collections.Generic;

namespace CQRSkiv.Infrastructure.Entities;

public class OrganizationUnitEntity
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public Guid? ParentId { get; set; }

  // Navigation property for the parent organization unit
  public OrganizationUnitEntity? Parent { get; set; }

  // Navigation property for child organization units
  public List<OrganizationUnitEntity> Children { get; set; } = new List<OrganizationUnitEntity>();

  // Navigation property for admin commissions responsible for this unit
  public List<AdminCommissionEntity> AdminCommissions { get; set; } = new List<AdminCommissionEntity>();
}