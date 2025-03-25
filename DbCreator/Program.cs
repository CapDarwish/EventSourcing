using DbCreator;

var builder = Host.CreateApplicationBuilder(args);

var connstring = builder
    .Configuration.GetConnectionString("eventstore")
    .Replace("Database=eventstore", string.Empty);
Console.WriteLine(connstring);
builder.AddKeyedNpgsqlDataSource("es", c => c.ConnectionString = connstring);
builder.AddKeyedNpgsqlDataSource("rd", c => c.ConnectionString = connstring);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
