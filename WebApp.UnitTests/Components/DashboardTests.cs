using Bunit;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Radzen;
using WebApp.Api.Jira;
using WebApp.Components.ReusableComponents;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Components;
using Xunit;

public class DashboardTests : TestContext, IClassFixture<TestFixture>
{
    
    public DashboardTests(TestFixture fixture)
    {
        // Configure JSInterop for Radzen components
        JSInterop.SetupVoid("Radzen.addMouseEnter", _ => true);
        JSInterop.SetupVoid("Radzen.addMouseLeave", _ => true);
        JSInterop.SetupVoid("Radzen.removeMouseEnter", _ => true);
        JSInterop.SetupVoid("Radzen.removeMouseLeave", _ => true);
        JSInterop.SetupVoid("Radzen.destroyTooltip", _ => true);
        JSInterop.SetupVoid("Radzen.createTooltip", _ => true);
        JSInterop.SetupVoid("Radzen.showTooltip", _ => true);
        JSInterop.SetupVoid("Radzen.hideTooltip", _ => true);
        
        // Add other common Radzen JSInterop calls that might be needed
        JSInterop.SetupVoid("Radzen.addContextMenu", _ => true);
        JSInterop.SetupVoid("Radzen.removeContextMenu", _ => true);
        JSInterop.SetupVoid("Radzen.closeContextMenu", _ => true);
        JSInterop.SetupVoid("Radzen.openContextMenu", _ => true);
        JSInterop.SetupVoid("Radzen.addDocumentClickHandler", _ => true);
        JSInterop.SetupVoid("Radzen.removeDocumentClickHandler", _ => true);

        
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
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestPlansModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestExecutionModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<DataGridSettingsService>());

      //  Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<Radzen.NotificationService>());
       // Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<Radzen.ContextMenuService>());

        Services.AddSingleton<Radzen.NotificationService>();
        Services.AddSingleton<Radzen.ContextMenuService>();
        Services.AddSingleton<Radzen.TooltipService>();
        Services.AddSingleton<FormNotificationService>();
        Services.AddSingleton<AppTooltipService>();
        Services.AddSingleton<EmailService>();
        Services.AddSingleton<Radzen.DialogService>();

    //    Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<FormNotificationService>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ApplicationUser>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestCasesFilesModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestStepsExecutionTimerService>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestExecutionTimerServicev2>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestCaseExecutionTimerService>());

        Services.AddSingleton<UserManager<ApplicationUser>, TestUserManager>();

       // Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ResourceManagerStringLocalizerFactory>());

        // Add localization services - DON'T try to get ResourceManagerStringLocalizerFactory
        Services.AddSingleton<IEmailSender<ApplicationUser>, TestEmailSender>();
        Services.AddSingleton<RoleManager<IdentityRole>, TestRoleManager>();
        Services.AddSingleton<IEmailService, EmailService>();  // Add this line
        Services.AddSingleton<UserService>();
        // Create a simple dummy localizer for the Resources type
        Services.AddLocalization();

        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>());

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







