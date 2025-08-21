using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Radzen;
using WebApp.Api.Jira;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;
using WebApp.Services.TestData;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.Components;
using WebApp.UnitTests.Models;
using RequirementsSpecificationModel = WebApp.Models.RequirementsSpecificationModel;

namespace WebApp.UnitTests.DIContainers;

public class TestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; private set; }

    public TestFixture()
    {
        var serviceCollection = new ServiceCollection();


        // Add configuration with mock Jira settings
        var jiraOptions = new JiraApiOptions
        {
            BaseUrl = "https://qawebmaster.atlassian.net",
            Username = "nicolasdiasgomes@gmail.com",
            ApiKey =
                "ATATT3xFfGF0wVSvi8EeTnlFjtFO0fTILF54Vz4rCtHgxFEofpOARSWh_MaH_qSJTD5hi5fP8ubZv2w301lnf66Jx-llk0NnppHr6LRzh6RSZdS3yaDzsNATd3_h9-yWbrCfApgiuK9eHYna0bw4VRjT9ITC7J-3fFeD7wngx6mw57cKzuBhYqA=7F44A4CE"
        };

        // Register the JiraApiOptions with the DI container
        serviceCollection.AddSingleton(Microsoft.Extensions.Options.Options.Create(jiraOptions));


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
        serviceCollection.AddScoped<TestCasesModel>();
        serviceCollection.AddScoped<TestCasesFilesModel>();
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