using WebApp.Services;
using WebApp.Services.TestData;

namespace WebApp.SetUp;

public static class ServiceCollectionExtensions
{
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
    
    
    public static void AddSeedingServices(this IServiceCollection services)
    {
        services.AddHostedService<RoleSeeder>();
        services.AddHostedService<ProjectDataSeeder>();
        services.AddHostedService<IntegrationDataSeeder>();
    }
}