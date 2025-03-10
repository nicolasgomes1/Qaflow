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

    
    public async Task<(IEnumerable<Requirements> Requirements, IList<Requirements> SelectedRequirements)> DisplayRequirementsIndexPage1()
    {
        var requirements = await _dbContext.Requirements
            .Include(r => r.TestCases)
            .Where(tc => tc.ProjectsId == projectSateService.GetProjectIdAsync().Result)
            .ToListAsync();

        var selectedRequirements = new List<Requirements>();
        var selection = requirements.FirstOrDefault();
        if (selection != null)
        {
            selectedRequirements.Add(selection);
        }

        return (requirements, selectedRequirements);
    }


    /// <summary>
    /// Returns a list of requirements for the project with default workflow status completed
    /// </summary>
    /// <returns></returns>
    public async Task<List<Requirements>> GetRequirementsWithWorkflowStatus(WorkflowStatus workflowStatus = WorkflowStatus.Completed)
    {
        return await _dbContext.Requirements
            .Where(r => r.ProjectsId == projectSateService.GetProjectIdAsync().Result)
            .Where(r =>r.WorkflowStatus == workflowStatus)
            .ToListAsync();
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
        requirement.ProjectsId = projectSateService.GetProjectIdAsync().Result;

        requirement.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        
        await _dbContext.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0)
        {
            await requirementsFilesModel.SaveFilesToDb(files, requirement.Id);
        }
        
        return requirement;
    }

    public async Task UpdateRequirement(int requirementId, List<IBrowserFile>? files)
    {
        var requirement = await _dbContext.Requirements.FindAsync(requirementId);
        if (requirement == null) throw new Exception("Requirement not found");
        
        // First, update the requirement
        _dbContext.Requirements.Update(requirement);
        requirement.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;

        requirement.ModifiedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0)
        {
            await requirementsFilesModel.SaveFilesToDb(files, requirementId);
        }
    }
    
    public async Task<List<Requirements>> GetRequirementsToValidateAgainstCsv()
    {
        return await _dbContext.Requirements
            .Where(r => r.ProjectsId == projectSateService.GetProjectIdAsync().Result)
            .ToListAsync();
    }
    
    public async Task<Requirements> AddRequirementFromCsv(Requirements requirement)
    {
        _dbContext.Requirements.Add(requirement);
        requirement.ProjectsId = projectSateService.GetProjectIdAsync().Result;
        requirement.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        requirement.WorkflowStatus = WorkflowStatus.New;
        requirement.CreatedAt = DateTime.UtcNow;
        requirement.ModifiedAt = DateTime.UtcNow;
        requirement.ProjectsId = projectSateService.GetProjectIdAsync().Result;
        requirement.ArchivedStatus = ArchivedStatus.Active;
        await _dbContext.SaveChangesAsync();
        return requirement;
    }
}