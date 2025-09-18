using Projects;

var builder = DistributedApplication.CreateBuilder(args);


var tests = builder.AddNpmApp("webapp-e2e-tests", "../WebApp.Tests");

//postgres user is postgres
var dbPassword = builder.AddParameter("DatabasePassword");
var user = builder.AddParameter("DatabaseUser");
// Configure PostgreSQL and ensure it returns an IResourceBuilder
var postgres = builder.AddPostgres("postgres", user, dbPassword)
    .WithPgWeb().WithDataVolume("data");
var postgresdb = postgres.AddDatabase("postgresdb");

var key = builder.AddParameter("OpenAIApiKey"); // Store securely!


// Add a project and set up dependencies on the database
var app = builder.AddProject<WebApp>("webapp")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithHttpHealthCheck("/health")
    .WithEnvironment("OpenApi_Key_Dev", key);

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") tests.WaitFor(app);

builder.Build().Run();