using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

using WebApp.Data.enums;

namespace WebApp.Models;

public class ProjectModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    UserService userService)
{
    private static readonly ILoggerFactory LoggerFactory =
        Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());

    private static readonly ILogger Logger = LoggerFactory.CreateLogger(nameof(ProjectModel));

    /// <summary>
    ///     Get Project by Id
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns>Returns the Project with the specified id.</returns>
    public async Task<Projects> GetProjectById(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        Logger.LogInformation($"Getting project {projectId}");
        return await db.Projects.FindAsync(projectId) ?? throw new Exception("Projects is null");
    }

    public async Task AddProject(Projects project)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        project.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        project.CreatedAt = DateTime.UtcNow;

        project.QAflowSettings = new List<QAflowSettings>
        {
            new()
            {
                QAflowOptionsSettings = QAflowOptionsSettings.ExternalIntegrations,
                IsIntegrationEnabled = false,
                CreatedBy = project.CreatedBy,
                CreatedAt = project.CreatedAt
            },
            new()
            {
                QAflowOptionsSettings = QAflowOptionsSettings.OwnIntegrations,
                IsIntegrationEnabled = false,
                CreatedBy = project.CreatedBy,
                CreatedAt = project.CreatedAt
            }
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();
        Logger.LogInformation($"Project {project.Name} added with default QAflowSettings");
    }

    public async Task AddProjectFromCsv(Projects project)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        project.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        project.CreatedAt = DateTime.UtcNow;

        project.QAflowSettings = new List<QAflowSettings>
        {
            new()
            {
                QAflowOptionsSettings = QAflowOptionsSettings.ExternalIntegrations,
                IsIntegrationEnabled = false,
                CreatedBy = project.CreatedBy,
                CreatedAt = project.CreatedAt
            },
            new()
            {
                QAflowOptionsSettings = QAflowOptionsSettings.OwnIntegrations,
                IsIntegrationEnabled = false,
                CreatedBy = project.CreatedBy,
                CreatedAt = project.CreatedAt
            }
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();
        Logger.LogInformation($"Project {project.Name} added from CSV with default QAflowSettings");
    }

    /// <summary>
    ///     Updates the project with the specified id.
    /// </summary>
    /// <param name="projects"></param>
    /// <exception cref="Exception"></exception>
    public async Task<Projects> UpdateProject(Projects projects)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        projects.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        projects.ModifiedAt = DateTime.UtcNow;

        db.Projects.Update(projects);
        await db.SaveChangesAsync();
        Logger.LogInformation($"Project {projects.Name} updated");
        return projects;
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
        Logger.LogInformation("Getting projects {projectId} with Test Cases and Requirements", projectId);
        ;
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