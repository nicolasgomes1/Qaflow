using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Radzen;
using WebApp.Api.Jira;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;
using WebApp.Services.TestData;
using WebApp.UnitTests.BaseTest;
using RequirementsSpecificationModel = WebApp.Models.RequirementsSpecificationModel;

namespace WebApp.UnitTests.DIContainers;

public class TestFixture : IDisposable
{
    public TestFixture()
    {
        var serviceCollection = new ServiceCollection();


        // Add configuration with mock Jira settings
        var jiraOptions = new JiraApiOptions
        {
            BaseUrl = "https://qawebmaster.atlassian.net",
            Username = "nicolasdiasgomes@gmail.com",
            ApiKey =
                "mynewapikey"
        };

        // Register the JiraApiOptions with the DI container
        serviceCollection.AddSingleton(Options.Create(jiraOptions));


        // Set up in-memory database
        serviceCollection.AddSingleton<DbContextOptions<ApplicationDbContext>>(provider =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("TestDb");
            return optionsBuilder.Options;
        });

        // Register ApplicationDbContext as scoped (using the singleton options)
        serviceCollection.AddScoped<ApplicationDbContext>(provider =>
        {
            var options = provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
            return new ApplicationDbContext(options);
        });


        var testContext = new TestContext();
        serviceCollection.AddSingleton<IJSRuntime>(testContext.JSInterop.JSRuntime);
        serviceCollection.AddSingleton<NavigationManager>(testContext.Services.GetRequiredService<NavigationManager>());


        // Add Data Protection services
        serviceCollection.AddDataProtection();

        // Register your services here
        serviceCollection.AddScoped<ProjectModel>();
        serviceCollection.AddSingleton<DialogService>();
        serviceCollection.AddScoped<AuthenticationStateProvider, TestAuthenticationStateProvider>();
        serviceCollection.AddScoped<UserService>();
        serviceCollection.AddScoped<ProjectState>();
        serviceCollection.AddScoped<RequirementsModel>();
        serviceCollection.AddScoped<RequirementsFilesModel>();
        serviceCollection.AddScoped<RoleSeeder>();
        serviceCollection.AddScoped<IntegrationDataSeeder>();
        serviceCollection.AddScoped<ProjectDataSeeder>();
        serviceCollection.AddScoped<QaFlowSettingsDataSeeder>();
        serviceCollection.AddScoped<BugsModel>();
        serviceCollection.AddScoped<BugsFilesModel>();
        serviceCollection.AddScoped<RequirementsSpecificationModel>();
        serviceCollection.AddScoped<TestCasesModel>();
        serviceCollection.AddScoped<TestCasesFilesModel>();
        serviceCollection.AddScoped<TestCasesReporting>();
        serviceCollection.AddScoped<TestCasesReporting>();
        serviceCollection.AddScoped<TestExecutionModelv2>();
        serviceCollection.AddScoped<TestExecutionModel>();
        serviceCollection.AddScoped<TestCasesJiraModel>();
        serviceCollection.AddScoped<GenerateJwtToken>();
        serviceCollection.AddScoped<TestPlansFilesModel>();
        serviceCollection.AddScoped<TestPlansModel>();
        serviceCollection.AddScoped<JiraServiceFromDb>();
        serviceCollection.AddScoped<IntegrationsModel>();
        serviceCollection.AddScoped<JiraService>();
        serviceCollection.AddScoped<ReportsModel>();
        serviceCollection.AddScoped<ApplicationUser>();
        serviceCollection.AddScoped<FormNotificationService>();
        serviceCollection.AddScoped<EmailService>();
        serviceCollection.AddScoped<DataGridSettingsService>();
        serviceCollection.AddScoped<TestStepsExecutionTimerService>();
        serviceCollection.AddScoped<TestExecutionTimerServicev2>();
        serviceCollection.AddScoped<TestCaseExecutionTimerService>();
        serviceCollection.AddScoped<UiUserVisibility>();
        serviceCollection.AddScoped<BugsCommentsModel>();
        serviceCollection.AddScoped<CyclesModel>();
        serviceCollection.AddScoped<TestStepsModel>();
        serviceCollection.AddScoped<TestStepExecutionFileModel>();
        serviceCollection.AddScoped<QAflowSettingsModel>();
        serviceCollection.AddScoped<ManageCsvUpload>();
        serviceCollection.AddScoped<NotificationService>();


        serviceCollection.AddSingleton<IEmailSender<ApplicationUser>, TestEmailSender>();


        serviceCollection.AddHttpClient();

        serviceCollection.AddSingleton<IDbContextFactory<ApplicationDbContext>>(provider =>
        {
            var options = provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
            return new PooledDbContextFactory<ApplicationDbContext>(options);
        });

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

            var settingsSeeder = scopedServices.GetService<QaFlowSettingsDataSeeder>();
            if (settingsSeeder is null) throw new Exception("Settings seeder is null");
            settingsSeeder.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public IServiceProvider ServiceProvider { get; }


    public void Dispose()
    {
        // Clean up any resources if needed (optional)
    }
}