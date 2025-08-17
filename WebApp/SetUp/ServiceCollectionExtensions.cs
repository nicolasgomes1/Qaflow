using WebApp.Api;
using WebApp.Components.Pages.Requirements;
using WebApp.Models;
using WebApp.Services;
using WebApp.Services.TestData;

namespace WebApp.SetUp;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Include Services
    /// </summary>
    /// <param name="services"></param>
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<DataGridSettingsService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddScoped<FormNotificationService>();
        services.AddScoped<ProjectState>();
        services.AddScoped<TestCaseExecutionTimerService>();
        //services.AddScoped<TestExecutionTimerService>();
        services.AddScoped<TestExecutionTimerServicev2>();
        services.AddScoped<TestStepsExecutionTimerService>();
        services.AddScoped<AppTooltipService>();
        services.AddScoped<UserService>();
        services.AddScoped<GenerateJwtToken>();
        services.AddScoped<FetchApiData>();
        services.AddScoped<UiUserVisibility>();
    }

    /// <summary>
    /// Data Seeding
    /// </summary>
    /// <param name="services"></param>
    public static void AddSeedingServices(this IServiceCollection services)
    {
        services.AddHostedService<RoleSeeder>();
        services.AddHostedService<ProjectDataSeeder>();
        services.AddHostedService<IntegrationDataSeeder>();
    }

    /// <summary>
    /// Entity Models
    /// </summary>
    /// <param name="services"></param>
    public static void AddModels(this IServiceCollection services)
    {
        services.AddScoped<BugsFilesModel>();
        services.AddScoped<BugsModel>();
        services.AddScoped<IntegrationsModel>();
        services.AddScoped<ProjectModel>();
        services.AddScoped<ReportsModel>();
        services.AddScoped<RequirementsFilesModel>();
        services.AddScoped<RequirementsModel>();
        services.AddScoped<TestCasesFilesModel>();
        services.AddScoped<TestCasesModel>();
        services.AddScoped<TestExecutionModel>();
        services.AddScoped<TestPlansFilesModel>();
        services.AddScoped<TestPlansModel>();
        services.AddScoped<TestStepExecutionFileModel>();
        services.AddScoped<TestCasesJiraModel>();
        services.AddScoped<RequirementsSpecificationModel>();
        services.AddScoped<TestExecutionModelv2>();
        services.AddScoped<TestCasesReporting>();
        services.AddScoped<CyclesModel>();
        services.AddScoped<GridSettingsModel>();
        services.AddScoped<TestStepsModel>();
        services.AddScoped<BugsCommentsModel>();
    }
}