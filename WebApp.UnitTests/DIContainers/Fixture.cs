using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;
using WebApp.Services.TestData;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.Models;
using RequirementsSpecificationModel = WebApp.Models.RequirementsSpecificationModel;

namespace WebApp.UnitTests.DIContainers;

public class TestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; private set; }

    public TestFixture()
    {
        var serviceCollection = new ServiceCollection();

        // Set up in-memory database
        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Add Data Protection services
        serviceCollection.AddDataProtection();

        // Register your services here
        serviceCollection.AddScoped<ProjectModel>();
        serviceCollection.AddScoped<AuthenticationStateProvider, TestAuthenticationStateProvider>();
        serviceCollection.AddScoped<UserService>();
        serviceCollection.AddScoped<ProjectState>();
        serviceCollection.AddScoped<RequirementsModel>();
        serviceCollection.AddScoped<RequirementsFilesModel>();
        serviceCollection.AddScoped<RoleSeeder>();
        serviceCollection.AddScoped<IntegrationDataSeeder>();
        serviceCollection.AddScoped<ProjectDataSeeder>();
        serviceCollection.AddScoped<BugsModel>();
        serviceCollection.AddScoped<BugsFilesModel>();
        serviceCollection.AddScoped<RequirementsSpecificationModel>();

        // Register IDbContextFactory
        serviceCollection.AddDbContextFactory<ApplicationDbContext>();

        // Configure Identity
        serviceCollection.AddIdentityCore<ApplicationUser>(options => { })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        // Build the service provider
        ServiceProvider = serviceCollection.BuildServiceProvider();

        // NOW create the scope and resolve the seeders
        using var scope = ServiceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        // Ensure the DB is created
        var db = scopedServices.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();


        try
        {
            var roleSeeder = scopedServices.GetRequiredService<RoleSeeder>();
            roleSeeder.StartAsync(CancellationToken.None).GetAwaiter().GetResult();

            var integrationSeeder = scopedServices.GetRequiredService<IntegrationDataSeeder>();
            integrationSeeder.StartAsync(CancellationToken.None).GetAwaiter().GetResult();

            var projectSeeder = scopedServices.GetRequiredService<ProjectDataSeeder>();
            projectSeeder.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    public void Dispose()
    {
        // Clean up any resources if needed (optional)
    }
}