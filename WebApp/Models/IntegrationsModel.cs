using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Models;

public class IntegrationsModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();


    /// <summary>
    /// Creates a new integration and saves it to the database.
    /// </summary>
    /// <param name="integration">The integration entity to be created.</param>
    /// <returns>Returns the created integration entity.</returns>
    /// <exception cref="DbUpdateException">Thrown when the database update fails.</exception>
    public async Task<Integrations> AddIntegration(Integrations integration)
    {
        _dbContext.Integrations.Add(integration);
        await _dbContext.SaveChangesAsync();
        return integration;
    }

    /// <summary>
    /// Updates an existing integration in the database.
    /// </summary>
    /// <param name="integration">The integration entity to be updated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="DbUpdateException">Thrown when the database update fails.</exception>
    public async Task UpdateIntegration(Integrations integration)
    {
        _dbContext.Integrations.Update(integration);
        await _dbContext.SaveChangesAsync();
    }


    /// <summary>
    /// Retrieves an integration by its ID.
    /// </summary>
    /// <param name="integrationId">The ID of the integration to retrieve.</param>
    /// <returns>Returns the integration with the specified ID.</returns>
    /// <exception cref="Exception">Thrown when the integration is not found.</exception>
    public async Task<Integrations> GetIntegrationById(int integrationId)
    {
        return await _dbContext.Integrations.FindAsync(integrationId) ?? throw new Exception("Integration is null");
    }


    /// <summary>
    /// Retrieves an integration by its unique key.
    /// </summary>
    /// <param name="uniqueKey">The unique key of the integration to retrieve.</param>
    /// <returns>Returns the integration with the specified unique key.</returns>
    /// <exception cref="Exception">Thrown when the integration is not found.</exception>
    public async Task<Integrations> GetIntegrationByUniqueKey(string uniqueKey)
    {
        return await _dbContext.Integrations.FirstOrDefaultAsync(x => x.UniqueKey == uniqueKey) ??
               throw new Exception("Integration is null");
    }
}