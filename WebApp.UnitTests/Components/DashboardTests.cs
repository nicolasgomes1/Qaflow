using Bunit;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using WebApp.Api.Jira;
using WebApp.Components.ReusableComponents;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Components;
using Xunit;

public class DashboardTests : TestContext, IClassFixture<TestFixture>
{
    
    public DashboardTests(TestFixture fixture)
    {
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ProjectModel>());
        // Register all services from the fixture with the BUnit TestContext
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestCasesReporting>());
        // Add other services that Dashboard component might need
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ProjectModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestExecutionModelv2>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ProjectState>());

        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<RequirementsModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<RequirementsSpecificationModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestCasesJiraModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<GenerateJwtToken>());

        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<BugsModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<BugsFilesModel>());

        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestCasesModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<JiraServiceFromDb>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<JiraService>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<IntegrationsModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestPlansFilesModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ReportsModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<RequirementsFilesModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<Radzen.NotificationService>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<FormNotificationService>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ApplicationUser>());

       // Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ResourceManagerStringLocalizerFactory>());

        // Add localization services - DON'T try to get ResourceManagerStringLocalizerFactory
        
        // Create a simple dummy localizer for the Resources type
        Services.AddLocalization();


    }
    
    [Fact]
    public void HelloWorldComponentRendersCorrectly()
    {
        // Act
        var cut = RenderComponent<Dashboard>(parameters => parameters
            .Add(p => p.ProjectId, 1)); // Dashboard requires ProjectId parameter
        var localizer = Services.GetService<IStringLocalizer<SharedStrings>>();

        // Assert
        // Update the assertion to match what Dashboard actually renders
        Assert.NotNull(cut.Find("[data-testid='d_requirementsSpecification']"));
        Assert.NotNull(cut.Find("[data-testid='d_testcases']"));
        Assert.NotNull(cut.Find("[data-testid='d_bugs']"));
    }
}

public class SharedStrings
{
    // This class is just a marker for the resource files
}

// Test implementation of IEmailSender<ApplicationUser> for testing
public class TestEmailSender : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        // Do nothing for tests
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        // Do nothing for tests
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        // Do nothing for tests
        return Task.CompletedTask;
    }
}





