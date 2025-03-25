using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var api = builder.AddProject<CQRSkiv_PublicApi>("api");

var frontend = builder
    .AddNpmApp("frontend", "../frontend", scriptName: "dev")
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();
