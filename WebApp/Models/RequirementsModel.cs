using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;

namespace WebApp.Models;

public class RequirementsModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    RequirementsFilesModel requirementsFilesModel,
    UserService userService)
{
    private static readonly ILoggerFactory LoggerFactory =
        Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());

    private static readonly ILogger Logger = LoggerFactory.CreateLogger(nameof(RequirementsModel));

    public int SelectedRequirementSpecificationId = -1;


    public async Task<List<Requirements>> DisplayRequirementsIndexPage(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var requirements = await db.Requirements
            .Include(r => r.LinkedTestCases)
            .Include(r => r.RequirementsSpecification)
            .Where(tc => tc.ProjectsId == projectId)
            .ToListAsync();

        return requirements;
    }

    public async Task<List<Requirements>> GetRequirementsAssignedToAll(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        Logger.LogInformation("Getting Requirements for Project {projectId}", projectId);
        return await db.Requirements.Where(rp => rp.ProjectsId == projectId).ToListAsync();
    }

    public async Task<List<Requirements>> GetRequirementsAssignedToCurrentUser(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.Requirements.Where(rp =>
                rp.ProjectsId == projectId && rp.AssignedTo == userService.GetCurrentUserInfoAsync().Result.UserId)
            .ToListAsync();
    }


    /// <summary>
    /// Returns a list of requirements for the project with default workflow status completed
    /// </summary>
    /// <returns></returns>
    public async Task<List<Requirements>> GetRequirementsWithWorkflowStatus(WorkflowStatus workflowStatus,
        int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.Requirements
            .Where(r => r.ProjectsId == projectId)
            .Where(r => r.WorkflowStatus == workflowStatus)
            .ToListAsync();
    }

    /// <summary>
    /// Load Requirement by Id
    /// With Test Cases and Linked RequirementsSpecifications
    /// </summary>
    /// <param name="requirementId"></param>
    /// <returns>Requirements</returns>
    public async Task<Requirements> GetRequirementByIdAsync(int requirementId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var requirement = await db.Requirements
            .Include(r => r.LinkedTestCases)
            .Include(r => r.RequirementsSpecification)
            .FirstOrDefaultAsync(r => r.Id == requirementId);

        if (requirement == null) throw new Exception("Requirement not found");

        return requirement;
    }

    public async Task<Requirements> AddRequirement(Requirements requirement, List<IBrowserFile>? files, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        // First, create the requirement
        db.Requirements.Add(requirement);
        requirement.ProjectsId = projectId;
        requirement.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;

        if (SelectedRequirementSpecificationId != -1)
            requirement.RequirementsSpecificationId = SelectedRequirementSpecificationId;

        SetArchivedStatus.SetArchivedStatusBasedOnWorkflow(requirement);

        await db.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0)
            await requirementsFilesModel.SaveFilesToDb(files, requirement.Id, projectId);

        return requirement;
    }


    public async Task<Requirements> UpdateRequirement(Requirements requirements, List<IBrowserFile>? files,
        int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();


        SetArchivedStatus.SetArchivedStatusBasedOnWorkflow(requirements);
        db.Entry(requirements).Property(r => r.RequirementsSpecificationId).IsModified = true;

        requirements.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        requirements.ModifiedAt = DateTime.UtcNow;

        if (SelectedRequirementSpecificationId != -1)
            requirements.RequirementsSpecificationId = SelectedRequirementSpecificationId;

        db.Requirements.Update(requirements);
        await db.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0)
            await requirementsFilesModel.SaveFilesToDb(files, requirements.Id, projectId);
        return requirements;
    }


    public async Task<List<Requirements>> GetRequirementsToValidateAgainstCsv(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.Requirements
            .Where(r => r.ProjectsId == projectId)
            .ToListAsync();
    }

    public async Task<Requirements> AddRequirementFromCsv(Requirements requirement, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        requirement.ProjectsId = projectId;
        requirement.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        requirement.WorkflowStatus = WorkflowStatus.New;
        requirement.CreatedAt = DateTime.UtcNow;
        requirement.ModifiedAt = DateTime.UtcNow;
        requirement.ArchivedStatus = ArchivedStatus.Active;
        db.Requirements.Add(requirement);
        await db.SaveChangesAsync();
        return requirement;
    }

    public async Task<List<RequirementsSpecification>> GetAssociatedRequirementsSpecifications(
        int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var requirementsSpecifications =
            await db.RequirementsSpecification.Where(p => p.ProjectsId == projectId)
                .ToListAsync();
        return requirementsSpecifications;
    }

    public async Task GetAssociatedRequirementSpecification(int requirementId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var requirement = await db.Requirements.FindAsync(requirementId);
        if (requirement is null) return;

        var current = requirement.RequirementsSpecificationId;
        if (current is not null)
            SelectedRequirementSpecificationId = (int)current;
    }
}