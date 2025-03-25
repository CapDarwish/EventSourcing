using System;

namespace CQRSkiv.Infrastructure.Entities;

public class EmploymentEntity
{
  public Guid PersonId { get; set; }
  public Guid OrganizationUnitId { get; set; }
  public string? Role { get; set; } 

  // Navigation properties
  public PersonEntity? Person { get; set; }
  public OrganizationUnitEntity? OrganizationUnit { get; set; }
}