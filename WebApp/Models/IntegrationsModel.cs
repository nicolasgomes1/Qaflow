using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Models;

public class IntegrationsModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    private static readonly ILoggerFactory LoggerFactory =
        Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());

    private static readonly ILogger Logger = LoggerFactory.CreateLogger("IntegrationsModel");

    /// <summary>
    /// Creates a new integration and saves it to the database.
    /// </summary>
    /// <param name="integration">The integration entity to be created.</param>
    /// <returns>Returns the created integration entity.</returns>
    /// <exception cref="DbUpdateException">Thrown when the database update fails.</exception>
    public async Task<Integrations> AddIntegration(Integrations integration)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        db.Integrations.Add(integration);
        await db.SaveChangesAsync();
        Logger.LogInformation("Added new integration");
        return integration;
    }

    /// <summary>
    /// Updates an existing integration in the database.
    /// </summary>
    /// <param name="integration">The integration entity to be updated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="DbUpdateException">Thrown when the database update fails.</exception>
    public async Task UpdateIntegration(int integrationId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var integration = await db.Integrations.FindAsync(integrationId);
        if (integration is null) throw new Exception("Bug not found");
        db.Integrations.Update(integration);
        await db.SaveChangesAsync();
        Logger.LogInformation("Updated integration");
    }


    /// <summary>
    /// Retrieves an integration by its ID.
    /// </summary>
    /// <param name="integrationId">The ID of the integration to retrieve.</param>
    /// <returns>Returns the integration with the specified ID.</returns>
    /// <exception cref="Exception">Thrown when the integration is not found.</exception>
    public async Task<Integrations> GetIntegrationByIdAsync(int integrationId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        Logger.LogInformation($"Getting integration {integrationId}");
        return await db.Integrations.FindAsync(integrationId) ?? throw new Exception("Integration is null");
    }


    /// <summary>
    /// Retrieves an integration by its unique key.
    /// </summary>
    /// <param name="uniqueKey">The unique key of the integration to retrieve.</param>
    /// <returns>Returns the integration with the specified unique key.</returns>
    /// <exception cref="Exception">Thrown when the integration is not found.</exception>
    public async Task<Integrations> GetIntegrationByUniqueKey(string uniqueKey)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.Integrations.FirstOrDefaultAsync(x => x.UniqueKey == uniqueKey) ??
               throw new Exception("Integration is null");
    }

    public async Task<List<Integrations>> GetListIntegrations()
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        Logger.LogInformation($"Getting integrations");
        return await db.Integrations.ToListAsync();
    }

    public async Task RemoveIntegrationAsync(int integrationId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var integration = await db.Integrations.FindAsync(integrationId) ??
                          throw new Exception("Integration is Null");
        db.Integrations.Remove(integration);
        await db.SaveChangesAsync();
        Logger.LogInformation("Removed integration with id:  {id}", integrationId);
        ;
    }
}