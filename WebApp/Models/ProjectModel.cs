using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class ProjectModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ProjectStateService _projectStateService;
    private readonly UserService _userService;

    public ProjectModel(
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        UserService userService,
        ProjectStateService projectStateService)
    {
        _userService = userService;
        _projectStateService = projectStateService;
        _dbContextFactory = dbContextFactory;
        _dbContext = _dbContextFactory.CreateDbContext();
    }

    /// <summary>
    ///     Get Project by Id
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns>Returns the Project with the specified id.</returns>
    public async Task<Projects> GetProjectById(int projectId)
    {
        return await _dbContext.Projects.FindAsync(projectId) ?? throw new Exception("Projects is null");
    }

    public async Task AddProject(Projects project)
    {
        _dbContext.Projects.Add(project);
        project.CreatedBy = _userService.GetCurrentUserInfoAsync().Result.UserName;
        project.CreatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddProjectFromCsv(Projects project)
    {
        _dbContext.Projects.Add(project);
        project.CreatedBy = _userService.GetCurrentUserInfoAsync().Result.UserName;
        project.CreatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Updates the project with the specified id.
    /// </summary>
    /// <param name="projectId"></param>
    /// <exception cref="Exception"></exception>
    public async Task UpdateProject(int projectId)
    {
        var project = await _dbContext.Projects.FindAsync(projectId) ?? throw new Exception("Project is Null");
        project.ModifiedBy = _userService.GetCurrentUserInfoAsync().Result.UserName;
        project.ModifiedAt = DateTime.UtcNow;
        _dbContext.Projects.Update(project);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Retrieves a list of projects based on the project state.
    ///     If the project state has a ProjectId set, it returns the specific project with its requirements and test cases.
    ///     If the project state does not have a ProjectId set, it returns all projects with related requirements and test
    ///     cases.
    /// </summary>
    /// <returns>A list of projects with their requirements and test cases.</returns>
    public async Task<List<Projects>> GetProjectsTestCasesRequirements()
    {
        // Check if ProjectId is set
        if (_projectStateService.ProjectId == 0) throw new InvalidOperationException("ProjectId is not set.");

        // If ProjectId is set, return the specific project with its requirements and test cases
        return await _dbContext.Projects
            .AsSplitQuery()
            .Include(p => p.Requirements)
            .ThenInclude(r => r.TestCases)
            .Where(p => p.Id == _projectStateService.ProjectId) // Filter by ProjectId
            .ToListAsync();
    }

    public async Task<List<Projects>> GetProjectsTestplansTestCases()
    {
        // Check if ProjectId is set
        if (_projectStateService.ProjectId == 0) throw new InvalidOperationException("ProjectId is not set.");

        // If ProjectId is set, return the specific project with its requirements and test cases
        return await _dbContext.Projects
            .AsSplitQuery()
            .Include(p => p.TestPlans)
            .ThenInclude(r => r.TestCases)
            .Where(p => p.Id == _projectStateService.ProjectId) // Filter by ProjectId
            .ToListAsync();
    }

    public async Task<List<Projects>> GetProjectsData()
    {
        // Check if ProjectId is set
        if (_projectStateService.ProjectId == 0) throw new InvalidOperationException("ProjectId is not set.");

        // If ProjectId is set, return the specific project with its requirements and test cases
        return await _dbContext.Projects
            .AsSplitQuery()
            .Include(p => p.Requirements)
            .ThenInclude(r => r.TestCases)
            .Include(p => p.TestPlans)
            .ThenInclude(r => r.TestCases)
            .Where(p => p.Id == _projectStateService.ProjectId) // Filter by ProjectId
            .ToListAsync();
    }

    /// <summary>
    ///     Returns a List of all Projects
    /// </summary>
    /// <returns></returns>
    public async Task<List<Projects>> GetProjects()
    {
        return await _dbContext.Projects.ToListAsync();
    }

    public async Task RemoveProject(int projectId)
    {
        var project = await _dbContext.Projects.FindAsync(projectId) ?? throw new Exception("Project is Null");
        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync();
    }
}