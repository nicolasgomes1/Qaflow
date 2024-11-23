var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sql-password", secret: true);

// Configure SQL Server and ensure it returns an IResourceBuilder
var sql = builder.AddSqlServer("sql", password: sqlPassword) // Use the overload that takes the name and password
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

builder.Build().Run();
