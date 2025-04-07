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

    public async Task<Requirements> UpdateRequirement(Requirements updatedRequirements, List<IBrowserFile>? files,
        int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var requirement = await db.Requirements.FindAsync(updatedRequirements.Id);
        if (requirement is null) throw new Exception("Requirement not found");

        SetArchivedStatus.SetArchivedStatusBasedOnWorkflow(requirement);

        requirement.Name = updatedRequirements.Name;
        requirement.Description = updatedRequirements.Description;
        requirement.Priority = updatedRequirements.Priority;
        requirement.WorkflowStatus = updatedRequirements.WorkflowStatus;
        requirement.AssignedTo = updatedRequirements.AssignedTo;
        requirement.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        requirement.ModifiedAt = DateTime.UtcNow;

        if (SelectedRequirementSpecificationId != -1)
            requirement.RequirementsSpecificationId = updatedRequirements.RequirementsSpecificationId;
        updatedRequirements.RequirementsSpecificationId = SelectedRequirementSpecificationId;
        if (SelectedRequirementSpecificationId != -10)
            requirement.RequirementsSpecificationId = null;

        db.Requirements.Update(requirement);
        await db.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0)
            await requirementsFilesModel.SaveFilesToDb(files, updatedRequirements.Id, projectId);
        return requirement;
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
}