using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class RequirementsSpecificationModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    UserService userService)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();


    public async Task<RequirementsSpecification> AddRequirementsSpecification(
        RequirementsSpecification requirementsSpecification, int projectId)
    {
        var currentTime = DateTime.UtcNow;
        requirementsSpecification.CreatedAt = currentTime;
        requirementsSpecification.ModifiedAt = currentTime;
        requirementsSpecification.ProjectsId = projectId;
        requirementsSpecification.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;

        await _dbContext.RequirementsSpecification.AddAsync(requirementsSpecification);
        await _dbContext.SaveChangesAsync();
        return requirementsSpecification;
    }

    public async Task<RequirementsSpecification> UpdateRequirementsSpecificationAsync(int requirementsSpecificationId)
    {
        var currentRequirement = await _dbContext.RequirementsSpecification.FindAsync(requirementsSpecificationId) ??
                                 throw new Exception("No requirements Specification Found");
        _dbContext.RequirementsSpecification.Update(currentRequirement);

        currentRequirement.ModifiedAt = DateTime.UtcNow;
        currentRequirement.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        await _dbContext.SaveChangesAsync();
        return currentRequirement;
    }


    public async Task<List<RequirementsSpecification>> GetRequirementsSpecificationListAsync(int projectId)
    {
        return await _dbContext.RequirementsSpecification
            .Include(r => r.LinkedRequirements)
            .Where(p => p.ProjectsId == projectId)
            .ToListAsync();
    }

    public async Task<RequirementsSpecification> GetRequirementsSpecificationByIdAsync(int requirementsSpecificationId)
    {
        var found = await _dbContext.RequirementsSpecification.FindAsync(requirementsSpecificationId) ??
                    throw new Exception("No requirements Specification Found");
        return found;
    }

    public async Task<bool> DeleteRequirementsSpecification(int requirementSpecificationId)
    {
        var noExceptions = 0;
        
        var logger = LoggerService.Logger;
        var found = await _dbContext.RequirementsSpecification.FindAsync(requirementSpecificationId);
        if (found == null) throw new Exception();
        var hasReq = found.LinkedRequirements.Any();
        switch (hasReq)
        {
            case false:
                noExceptions = 0;
                _dbContext.Remove(found);
                await _dbContext.SaveChangesAsync();
                logger.LogInformation("Removed Record");
                break;
            case true:
                noExceptions = 1;
                logger.LogInformation("Can't remove record has it has linked requirements");
                break;
        }

        if (noExceptions == 0) return false;
        return true;
    }
}