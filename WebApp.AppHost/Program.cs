using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Add the following line to configure the Docker Compose environment
builder.AddDockerComposeEnvironment("env");

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
    .WithExternalHttpEndpoints()
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithHttpHealthCheck("/health")
    .WithEnvironment("OpenApi_Key_Dev", key);


if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    var tests = builder.AddJavaScriptApp(
        name: "webapp-e2e-tests",
        appDirectory: "../WebApp.Tests",
        runScriptName: "test" // or "e2e", "playwright", etc. (whatever exists in package.json scripts)
    );
    tests.WaitFor(postgres);
}

//if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") tests.WaitFor(app);

builder.Build().Run();