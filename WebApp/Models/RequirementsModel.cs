using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;

namespace WebApp.Models;

public class RequirementsModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    RequirementsFilesModel requirementsFilesModel,
    UserService userService,
    ProjectStateService projectSateService)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();


    public Requirements Requirements { get; set; } = new();

    public List<Requirements> RequirementsList { get; private set; } = [];

    /// <summary>
    /// Ienumerable of Requirements
    /// </summary>
    public IEnumerable<Requirements> requirements = [];

    public IList<Requirements> selectedRequirements = new List<Requirements>();


    public async Task DisplayRequirementsIndexPage()
    {
        requirements = await _dbContext.Requirements.Include(r => r.TestCases)
            .Where(tc => tc.ProjectsId == projectSateService.ProjectId).ToListAsync();

        var selection = requirements.FirstOrDefault();
        if (selection == null)
        {
            return;
        }

        selectedRequirements = new List<Requirements> { selection };
    }


    /// <summary>
    /// Returns a list of requirements for the project
    /// </summary>
    /// <returns></returns>
    public async Task<List<Requirements>> GetallRequirements()
    {
        return await _dbContext.Requirements
            .Where(r => r.ProjectsId == projectSateService.ProjectId)
            .Where(r =>r.WorkflowStatus == WorkflowStatus.Completed)
            .ToListAsync();
    }

    public async Task GetallRequirementsWithTests()
    {
        RequirementsList = await _dbContext.Requirements.Include(r => r.TestCases).ToListAsync();
    }


    /// <summary>
    /// Load Requirement by Id
    /// </summary>
    /// <param name="requirementId"></param>
    /// <returns></returns>
    public async Task<Requirements> GetRequirementByIdAsync(int requirementId)
    {
        var requirement = await _dbContext.Requirements
            .Include(r => r.TestCases)
            .FirstOrDefaultAsync(r => r.Id == requirementId);

        if (requirement == null) throw new Exception("Requirement not found");

        return requirement;
    }

    public async Task<Requirements> CreateRequirement(Requirements requirement, List<IBrowserFile>? files)
    {
        // First, create the requirement
        _dbContext.Requirements.Add(requirement);
        requirement.ProjectsId = projectSateService.ProjectId;

        requirement.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        
        await _dbContext.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0)
        {
            await requirementsFilesModel.SaveFilesToDb(files, requirement.Id);
        }


        return requirement;
    }

    public async Task UpdateRequirement(Requirements requirement, List<IBrowserFile>? files)
    {
        // First, update the requirement
        _dbContext.Requirements.Update(requirement);
        requirement.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;

        requirement.ModifiedAt = DateTime.Now;
        await _dbContext.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0)
        {
            await requirementsFilesModel.SaveFilesToDb(files, requirement.Id);
        }
    }
}