using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));
var eventstore = postgres.AddDatabase("eventstore");
var readdb = postgres.AddDatabase("readdb");

var worker = builder
    .AddProject<DbCreator>("dbcreator")
    .WithReference(eventstore)
    .WithReference(readdb)
    .WaitFor(postgres);

var api = builder
    .AddProject<CQRSkiv_PublicApi>("api")
    .WithReference(eventstore)
    .WithReference(readdb)
    .WaitFor(worker, WaitBehavior.StopOnResourceUnavailable);

var frontend = builder
    .AddNpmApp("frontend", "../frontend", scriptName: "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();
