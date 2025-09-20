using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using WebApp.Data;
using WebApp.Services.TestData;

namespace WebApp.SetUp;

public static class WebApplicationExtentions
{
    public static async Task ConfigureDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await EnsureDatabaseAsync(dbContext);
        await RunMigrationsAsync(dbContext);
        var cleanupService = scope.ServiceProvider.GetRequiredService<ICleanUpPlaywrightTestsData>();
        await cleanupService.DeleteAllPlaywrightProjectData();
    }


    private static async Task EnsureDatabaseAsync(ApplicationDbContext dbContext)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            if (!await dbCreator.ExistsAsync()) await dbCreator.CreateAsync();
        });
    }

    private static async Task RunMigrationsAsync(ApplicationDbContext dbContext)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () => { await dbContext.Database.MigrateAsync(); });
    }
}