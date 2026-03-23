using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class RequirementsSpecificationModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    UserService userService)
{
    private static readonly ILoggerFactory LoggerFactory =
        Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());

    private static readonly ILogger Logger = LoggerFactory.CreateLogger(nameof(RequirementsSpecificationModel));


    public async Task<RequirementsSpecification> AddRequirementsSpecification(
        RequirementsSpecification requirementsSpecification, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var currentTime = DateTime.UtcNow;
        requirementsSpecification.CreatedAt = currentTime;
        requirementsSpecification.ModifiedAt = currentTime;
        requirementsSpecification.ProjectsId = projectId;

        //assign for now username not id
        requirementsSpecification.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;

        await db.RequirementsSpecification.AddAsync(requirementsSpecification);
        await db.SaveChangesAsync();
        return requirementsSpecification;
    }

    public async Task<RequirementsSpecification> UpdateRequirementsSpecificationAsync(
        RequirementsSpecification updated)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        updated.ModifiedAt = DateTime.UtcNow;
        updated.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;

        db.RequirementsSpecification.Update(updated);
        await db.SaveChangesAsync();

        return updated;
    }


    /// <summary>
    ///     Retrieves a list of requirements specifications associated with a specific project.
    /// </summary>
    /// <param name="projectId">
    ///     The unique identifier of the project for which the requirements specifications are to be
    ///     retrieved.
    /// </param>
    /// <returns>
    ///     A task representing an asynchronous operation. The task result contains a list of requirements specifications
    ///     associated with the specified project.
    /// </returns>
    public async Task<List<RequirementsSpecification>> GetRequirementsSpecificationListAsync(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.RequirementsSpecification
            .Where(p => p.ProjectsId == projectId)
            .Include(r => r.LinkedRequirements)
            .ToListAsync();
    }

    /// <summary>
    ///     Retrieves a specific requirements specification by its unique identifier.
    /// </summary>
    /// <param name="requirementsSpecificationId">
    ///     The unique identifier of the requirements specification to retrieve.
    /// </param>
    /// <returns>
    ///     A task representing an asynchronous operation. The task result contains the requirements specification associated
    ///     with the provided identifier.
    /// </returns>
    public async Task<RequirementsSpecification> GetRequirementsSpecificationByIdAsync(int requirementsSpecificationId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var found = await db.RequirementsSpecification.FindAsync(requirementsSpecificationId) ??
                    throw new Exception("No requirements Specification Found");
        return found;
    }

    /// <summary>
    ///     Deletes a specific requirements specification identified by its unique identifier.
    /// </summary>
    /// <param name="requirementSpecificationId">The unique identifier of the requirements specification to be deleted.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result is a boolean where true indicates the
    ///     deletion was not possible due to linked requirements, and false indicates successful deletion.
    /// </returns>
    public async Task<bool> DeleteRequirementsSpecification(int requirementSpecificationId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();


        var noExceptions = 0;

        var found = await db.RequirementsSpecification
            .Include(t => t.LinkedRequirements)
            .FirstOrDefaultAsync(t => t.Id == requirementSpecificationId);
        if (found == null) throw new Exception();
        var hasReq = found.LinkedRequirements.Any();
        switch (hasReq)
        {
            case false:
                noExceptions = 0;
                db.Remove(found);
                await db.SaveChangesAsync();
                Logger.LogInformation("Removed Record");
                break;
            case true:
                noExceptions = 1;
                Logger.LogInformation("Can't remove record has it has linked requirements");
                break;
        }

        if (noExceptions == 0) return false;
        return true;
    }

    public async Task<RequirementsSpecification> AddRequirementsSpecificationFromCsv(
        RequirementsSpecification requirementSpecification, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        requirementSpecification.ProjectsId = projectId;
        db.RequirementsSpecification.Add(requirementSpecification);
        await db.SaveChangesAsync(userService);

        return requirementSpecification;
    }
}