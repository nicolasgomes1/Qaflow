var builder = DistributedApplication.CreateBuilder(args);


var tests = builder.AddNpmApp("webapptests",
    "../WebApp.Tests", "start");


#if SQLSERVER
var sqlPassword = builder.AddParameter("sql-password", secret: true);
// Configure SQL Server and ensure it returns an IResourceBuilder
var sql = builder.AddSqlServer("sql", password: sqlPassword, port: 1400) // Use the overload that takes the name and password
    .WithDataVolume("data")
    .WithLifetime(ContainerLifetime.Persistent);

// Add a database to the SQL Server instance
var db = sql.AddDatabase("qa");

// Add a project and set up dependencies on the database
builder.AddProject<Projects.WebApp>("webapp")
    .WithReference(sql)
    .WaitFor(sql)
    .WithReference(db)
    .WaitFor(db);
#elif POSTGRES
var dbPassword = builder.AddParameter("DatabasePassword", false);
// Configure PostgreSQL and ensure it returns an IResourceBuilder
var postgres = builder.AddPostgres("postgres", password: dbPassword)
    .WithPgWeb().WithDataVolume("data",isReadOnly: false);
var postgresdb = postgres.AddDatabase("postgresdb");


// Add a project and set up dependencies on the database
var app = builder.AddProject<Projects.WebApp>("webapp")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithHttpsHealthCheck("/health");

tests.WaitFor(app);


#endif

builder.Build().Run();
