using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;

namespace WebApp.Services.TestData;

public class IntegrationDataSeeder(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        await using var
            dbContext =
                await dbContextFactory.CreateDbContextAsync(cancellationToken); // Use using for context disposal

        // Seeding process
        await SeedIntegrationDataAsync(dbContext);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask; // Nothing to clean up for this case
    }

    private static async Task SeedIntegrationDataAsync(ApplicationDbContext dbContext)
    {
        await GetOrCreateIntegrationAsync(dbContext);
    }

    private static async Task<Integrations> GetOrCreateIntegrationAsync(ApplicationDbContext dbContext)
    {
        var integration =
            await dbContext.Integrations.FirstOrDefaultAsync(p => p.IntegrationType == IntegrationType.Jira);
        if (integration == null)
        {
            integration = new Integrations
            {
                IntegrationType = IntegrationType.Jira,
                Username = "nicolasdiasgomes@gmail.com",
                BaseUrl = "https://qawebmaster.atlassian.net",
                ApiKey =
                    "ATATT3xFfGF0wVSvi8EeTnlFjtFO0fTILF54Vz4rCtHgxFEofpOARSWh_MaH_qSJTD5hi5fP8ubZv2w301lnf66Jx-llk0NnppHr6LRzh6RSZdS3yaDzsNATd3_h9-yWbrCfApgiuK9eHYna0bw4VRjT9ITC7J-3fFeD7wngx6mw57cKzuBhYqA=7F44A4CE",
                UniqueKey = "Jira-01"
            };
            dbContext.Integrations.Add(integration);
            await dbContext.SaveChangesAsync();
        }

        return integration;
    }
}