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
        services.AddScoped<EnumService>();
        services.AddScoped<FormNotificationService>();
        services.AddSingleton<ProjectStateService>();
        services.AddScoped<StylesService>();
        services.AddScoped<TestCaseExecutionTimerService>();
        services.AddScoped<TestExecutionTimerService>();
        services.AddScoped<TestExecutionTimerServicev2>();
        services.AddScoped<TestStepsExecutionTimerService>();
        services.AddScoped<AppTooltipService>();
        services.AddScoped<UserService>();
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
    }
}