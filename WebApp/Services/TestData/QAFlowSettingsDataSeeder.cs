using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;

namespace WebApp.Services.TestData;

public class QaFlowSettingsDataSeeder(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        await using var
            dbContext =
                await dbContextFactory.CreateDbContextAsync(cancellationToken); // Use using for context disposal

        // Seeding process
        await SeedQAflowSettingsDataAsync(dbContext);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask; // Nothing to clean up for this case
    }
    
    private static async Task SeedQAflowSettingsDataAsync(ApplicationDbContext dbContext)
    {
        await GetOrCreateQAflowSettingsAsync(dbContext);
    }

    private static async Task GetOrCreateQAflowSettingsAsync(ApplicationDbContext dbContext)
    {
        var settings = await dbContext.QAflowSettings.Where(p => p.QAflowOptionsSettings == QAflowOptionsSettings.ExternalIntegrations).FirstOrDefaultAsync();
        if (settings != null) return;
        settings = new QAflowSettings
        {
            IsIntegrationEnabled = false,
            QAflowOptionsSettings = QAflowOptionsSettings.ExternalIntegrations
        };
        await dbContext.QAflowSettings.AddAsync(settings);
        
        var settings1 = await dbContext.QAflowSettings.Where(p => p.QAflowOptionsSettings == QAflowOptionsSettings.OwnIntegrations).FirstOrDefaultAsync();
        if (settings1 != null) return;
        settings1 = new QAflowSettings
        {
            IsIntegrationEnabled = false,
            QAflowOptionsSettings = QAflowOptionsSettings.OwnIntegrations
        };
        await dbContext.QAflowSettings.AddAsync(settings1);
        await dbContext.SaveChangesAsync();
    }
}