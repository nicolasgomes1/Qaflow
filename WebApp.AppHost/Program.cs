var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sql-password", secret: true);


#if SQLSERVER
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
// Configure PostgreSQL and ensure it returns an IResourceBuilder
var postgres = builder.AddPostgres("postgres").WithPgWeb().WithDataVolume("data",isReadOnly: false);
var postgresdb = postgres.AddDatabase("postgresdb");


// Add a project and set up dependencies on the database
builder.AddProject<Projects.WebApp>("webapp")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

#endif



builder.Build().Run();
