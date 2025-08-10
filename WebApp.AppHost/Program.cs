using Projects;

var builder = DistributedApplication.CreateBuilder(args);


var tests = builder.AddNpmApp("webapptests", "../WebApp.Tests", "start");


var dbPassword = builder.AddParameter("DatabasePassword", false);
// Configure PostgreSQL and ensure it returns an IResourceBuilder
var postgres = builder.AddPostgres("postgres", password: dbPassword)
    .WithPgWeb().WithDataVolume("data", false);
var postgresdb = postgres.AddDatabase("postgresdb");


// Add a project and set up dependencies on the database
var app = builder.AddProject<WebApp>("webapp")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithHttpHealthCheck("/health");

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    tests.WaitFor(app);
}

builder.Build().Run();