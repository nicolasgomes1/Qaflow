using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;
using WebApp.UnitTests.Models;

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

        // Register IDbContextFactory
        serviceCollection.AddDbContextFactory<ApplicationDbContext>();

        // Configure Identity
        serviceCollection.AddIdentityCore<ApplicationUser>(options => { })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Build the service provider
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
    public void Dispose()
    {
        // Clean up any resources if needed (optional)
    }
}
