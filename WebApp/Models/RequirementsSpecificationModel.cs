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

    public async Task<List<RequirementsSpecification>> GetRequirementsSpecificationListAsync(int projectId)
    {
        return await _dbContext.RequirementsSpecification
            .Include(r => r.LinkedRequirements)
            .Where(p => p.ProjectsId == projectId)
            .ToListAsync();
    }
}