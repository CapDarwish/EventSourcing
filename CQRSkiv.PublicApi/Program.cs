using CQRSkiv.Application.Services;
using CQRSkiv.Core.Interfaces;
using CQRSkiv.Domain.Aggregates;
using CQRSkiv.Infrastructure.Persistence;
using CQRSkiv.Infrastructure.Projections;
using CQRSkiv.Infrastructure.Repositories;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Configure EF Core for PostgreSQL (Read Database)
builder.Services.AddDbContext<ReadDbContext>(options =>
    options.UseNpgsql("Host=postgres;Port=5432;Database=readdb;Username=postgres;Password=yourpassword"));

// Register application services and repositories
builder.Services.AddScoped<IRepository<Person>, PersonRepository>();
builder.Services.AddScoped<IRepository<OrganizationUnit>, OrganizationUnitRepository>();
builder.Services.AddScoped<IRepository<AdminCommission>, AdminCommissionRepository>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IOrganizationUnitService, OrganizationUnitService>();
builder.Services.AddScoped<IAdminCommissionService, AdminCommissionService>();
builder.Services.AddScoped<IEventStoreQueryService, EventStoreQueryService>();

// Configure Marten for PostgreSQL (Event Store Database) and enable the async daemon
builder.Services.AddMarten(options =>
{
  options.Connection("Host=postgres;Port=5432;Database=eventstore;Username=postgres;Password=yourpassword");
  options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;

  // Configure aggregates for event sourcing
  options.Schema.For<Person>().Identity(x => x.Id).UseOptimisticConcurrency(true);
  options.Schema.For<OrganizationUnit>().Identity(x => x.Id).UseOptimisticConcurrency(true);
  options.Schema.For<AdminCommission>().Identity(x => x.Id).UseOptimisticConcurrency(true);

  // Register the custom projection
  options.Projections.Add(new ReadModelProjection(), ProjectionLifecycle.Async);
})
.UseLightweightSessions()
.AddAsyncDaemon(DaemonMode.Solo);

var app = builder.Build();

// Set the static service provider for ReadModelProjection
ReadModelProjection.ServiceProvider = app.Services;

// Ensure the read database is created
using (var scope = app.Services.CreateScope())
{
  var dbContext = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
  dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
app.MapControllers();

app.Run();