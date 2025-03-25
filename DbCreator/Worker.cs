using Npgsql;

namespace DbCreator;

public class Worker : BackgroundService
{
    private readonly NpgsqlDataSource eventstore;
    private readonly NpgsqlDataSource readdb;
    private readonly IHostApplicationLifetime lifetime;

    public Worker(
        [FromKeyedServices("es")] NpgsqlDataSource eventstore,
        [FromKeyedServices("rd")] NpgsqlDataSource readdb,
        IHostApplicationLifetime lifetime
    )
    {
        this.eventstore = eventstore;
        this.readdb = readdb;
        this.lifetime = lifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            Console.WriteLine(eventstore.ConnectionString);
            using var eventstoreConnection = eventstore.CreateConnection();
            eventstoreConnection.Open();
            using var command = eventstoreConnection.CreateCommand();
            command.CommandText = $"CREATE DATABASE eventstore;";
            command.ExecuteNonQuery();
            eventstoreConnection.Close();
        }
        catch (PostgresException p) when (p.SqlState == "42P04")
        {
            Console.WriteLine("Eventstore Database already exists.");
            // Ignore the error if the database already exists when Aspire tries to create it automatically.
            // If the database was created by the user, then this exception would be thrown.
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to create database Eventstore");
            Console.WriteLine(e);
        }

        try
        {
            Console.WriteLine(readdb.ConnectionString);
            using var readdbConnection = readdb.CreateConnection();
            readdbConnection.Open();
            using var command2 = readdbConnection.CreateCommand();
            command2.CommandText = $"CREATE DATABASE readdb;";
            command2.ExecuteNonQuery();
            readdbConnection.Close();
        }
        catch (PostgresException p) when (p.SqlState == "42P04")
        {
            Console.WriteLine("Readdb Database already exists.");
            // Ignore the error if the database already exists when Aspire tries to create it automatically.
            // If the database was created by the user, then this exception would be thrown.
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to create databases Readdb");
            Console.WriteLine(e);
        }
        lifetime.StopApplication();
    }
}
