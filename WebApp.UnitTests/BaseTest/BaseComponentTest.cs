using Bunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using WebApp.Api.Jira;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.BaseTest;

public abstract class BaseComponentTest : BunitContext, IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    protected BaseComponentTest(TestFixture fixture)
    {
        _fixture = fixture;
        SetupJsInterop();
        RegisterServices();
    }

    private void SetupJsInterop()
    {
        var radzenInterops = new[]
        {
            "Radzen.addMouseEnter", "Radzen.addMouseLeave",
            "Radzen.removeMouseEnter", "Radzen.removeMouseLeave",
            "Radzen.destroyTooltip", "Radzen.createTooltip",
            "Radzen.showTooltip", "Radzen.hideTooltip",
            "Radzen.addContextMenu", "Radzen.removeContextMenu",
            "Radzen.closeContextMenu", "Radzen.openContextMenu",
            "Radzen.addDocumentClickHandler", "Radzen.removeDocumentClickHandler"
        };

        foreach (var interop in radzenInterops) JSInterop.SetupVoid(interop, _ => true);
    }

    private void RegisterServices()
    {
        // Register services from fixture
        RegisterFixtureServices();

        // Register standalone services
        RegisterStandaloneServices();

        // Register additional services
        Services.AddSingleton<UserManager<ApplicationUser>, TestUserManager>();
        Services.AddSingleton<IEmailSender<ApplicationUser>, TestEmailSender>();
        Services.AddSingleton<RoleManager<IdentityRole>, TestRoleManager>();
        Services.AddSingleton<IEmailService, EmailService>();
        Services.AddSingleton<UserService>();
        Services.AddLocalization();
    }

    private void RegisterFixtureServices()
    {
        var servicesToRegister = new Type[]
        {
            typeof(ProjectModel),
            typeof(TestCasesReporting),
            typeof(TestExecutionModelv2),
            typeof(ProjectState),
            typeof(RequirementsModel),
            typeof(RequirementsSpecificationModel),
            typeof(TestCasesJiraModel),
            typeof(GenerateJwtToken),
            typeof(BugsModel),
            typeof(BugsFilesModel),
            typeof(TestCasesModel),
            typeof(JiraServiceFromDb),
            typeof(JiraService),
            typeof(IntegrationsModel),
            typeof(TestPlansFilesModel),
            typeof(ReportsModel),
            typeof(RequirementsFilesModel),
            typeof(TestPlansModel),
            typeof(TestExecutionModel),
            typeof(DataGridSettingsService),
            typeof(ApplicationUser),
            typeof(TestCasesFilesModel),
            typeof(TestStepsExecutionTimerService),
            typeof(TestExecutionTimerServicev2),
            typeof(TestCaseExecutionTimerService),
            typeof(UiUserVisibility),
            typeof(IDbContextFactory<ApplicationDbContext>),
            typeof(BugsCommentsModel),
            typeof(CyclesModel),
            typeof(TestStepsModel),
            typeof(TestStepExecutionFileModel),
            typeof(QAflowSettingsModel),
            typeof(Radzen.DialogService),
            typeof(ManageCsvUpload)
        };

        foreach (var serviceType in servicesToRegister)
            Services.AddSingleton(serviceType, _fixture.ServiceProvider.GetRequiredService(serviceType));
    }

    private void RegisterStandaloneServices()
    {
        Services.AddSingleton<NotificationService>();
        Services.AddSingleton<ContextMenuService>();
        Services.AddSingleton<TooltipService>();
        Services.AddSingleton<DialogService>();
        Services.AddSingleton<FormNotificationService>();
        Services.AddSingleton<AppTooltipService>();
        Services.AddSingleton<EmailService>();
    }
}