using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class ProjectModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    UserService userService)
{
    /// <summary>
    ///     Get Project by Id
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns>Returns the Project with the specified id.</returns>
    public async Task<Projects> GetProjectById(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.Projects.FindAsync(projectId) ?? throw new Exception("Projects is null");
    }

    public async Task AddProject(Projects project)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        db.Projects.Add(project);
        project.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        project.CreatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task AddProjectFromCsv(Projects project)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        db.Projects.Add(project);
        project.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        project.CreatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    /// <summary>
    ///     Updates the project with the specified id.
    /// </summary>
    /// <param name="projectId"></param>
    /// <exception cref="Exception"></exception>
    public async Task<Projects> UpdateProject(Projects updatedProject)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var project = await db.Projects.FindAsync(updatedProject.Id)
                      ?? throw new Exception("Project is null");

        // Update the fields from the updatedProject
        project.Name = updatedProject.Name;
        project.Description = updatedProject.Description;

        project.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        project.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return project;
    }


    /// <summary>
    ///     Retrieves a list of projects based on the project state.
    ///     If the project state has a ProjectId set, it returns the specific project with its requirements and test cases.
    ///     If the project state does not have a ProjectId set, it returns all projects with related requirements and test
    ///     cases.
    /// </summary>
    /// <returns>A list of projects with their requirements and test cases.</returns>
    public async Task<List<Projects>> GetProjectsTestCasesRequirements(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.Projects
            .AsSplitQuery()
            .Include(p => p.Requirements)
            .ThenInclude(r => r.LinkedTestCases)
            .Where(p => p.Id == projectId) // Filter by ProjectId
            .ToListAsync();
    }

    public async Task<List<Projects>> GetProjectsTestplansTestCases(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.Projects
            .Where(p => p.Id == projectId) // Ensure filtering is done early
            .AsSplitQuery() // Helps optimize loading related data
            .Include(p => p.TestPlans)
            .ThenInclude(tp => tp.LinkedTestCases)
            .ToListAsync();
    }


    public async Task<List<Projects>> GetProjectsData(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.Projects
            .Where(p => p.Id == projectId) // Filter by ProjectId
            .AsSplitQuery()
            .Include(p => p.Requirements)
            .ThenInclude(r => r.LinkedTestCases)
            .Include(p => p.TestPlans)
            .ThenInclude(r => r.LinkedTestCases)
            .ToListAsync();
    }

    /// <summary>
    ///     Returns a List of all Projects
    /// </summary>
    /// <returns></returns>
    public async Task<List<Projects>> GetProjects()
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.Projects.ToListAsync();
    }

    public async Task RemoveProject(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var project = await db.Projects.FindAsync(projectId) ?? throw new Exception("Project is Null");
        db.Projects.Remove(project);
        await db.SaveChangesAsync();
    }
}