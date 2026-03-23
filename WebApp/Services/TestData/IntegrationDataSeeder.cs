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
        var project = await ProjectDataSeeder.GetOrCreateProjectAsync(dbContext, "Demo Project With Data");

        var integration =
            await dbContext.Integrations.FirstOrDefaultAsync(p => p.IntegrationType == IntegrationType.Jira);
        if (integration != null) return integration;
        integration = new Integrations
        {
            IntegrationType = IntegrationType.Jira,
            Username = "nicolasdiasgomes@gmail.com",
            BaseUrl = "https://qawebmaster.atlassian.net",
            ApiKey =
                "jirakey",
            UniqueKey = "Jira-01",
            JiraProjectKey = "MFLP",
            ProjectsId = project.Id
        };
        dbContext.Integrations.Add(integration);
        await dbContext.SaveChangesAsync();

        return integration;
    }
}