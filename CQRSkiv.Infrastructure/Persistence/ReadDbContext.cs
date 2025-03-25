using CQRSkiv.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CQRSkiv.Infrastructure.Persistence;

public class ReadDbContext : DbContext
{
  public DbSet<PersonEntity> Persons { get; set; }
  public DbSet<OrganizationUnitEntity> OrganizationUnits { get; set; }
  public DbSet<AdminCommissionEntity> AdminCommissions { get; set; }
  public DbSet<EmploymentEntity> Employments { get; set; }

  public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // Configure Employment -> Person relationship
    modelBuilder.Entity<EmploymentEntity>()
        .HasKey(e => new { e.PersonId, e.OrganizationUnitId });

    modelBuilder.Entity<EmploymentEntity>()
        .HasOne(e => e.Person)
        .WithMany()
        .HasForeignKey(e => e.PersonId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<EmploymentEntity>()
        .HasOne(e => e.OrganizationUnit)
        .WithMany()
        .HasForeignKey(e => e.OrganizationUnitId)
        .OnDelete(DeleteBehavior.Cascade);

    // Configure AdminCommission -> OrganizationUnit (ResponsibleOrganizationId) relationship
    modelBuilder.Entity<AdminCommissionEntity>()
        .HasOne(ac => ac.ResponsibleOrganization)
        .WithMany(ou => ou.AdminCommissions)
        .HasForeignKey(ac => ac.ResponsibleOrganizationId)
        .OnDelete(DeleteBehavior.Restrict);

    // Configure OrganizationUnit -> OrganizationUnit (ParentId) relationship
    modelBuilder.Entity<OrganizationUnitEntity>()
        .HasOne(ou => ou.Parent)
        .WithMany(ou => ou.Children)
        .HasForeignKey(ou => ou.ParentId)
        .OnDelete(DeleteBehavior.SetNull);
  }
}