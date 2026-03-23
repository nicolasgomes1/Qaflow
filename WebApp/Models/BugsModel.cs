using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Radzen;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class BugsModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    UserService userService,
    BugsFilesModel bugsFilesModel)
{
    private static readonly ILoggerFactory LoggerFactory =
        Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());

    private static readonly ILogger Logger = LoggerFactory.CreateLogger(nameof(BugsModel));

    public List<int> SelectedTestCasesIds { get; set; } = [];

    /// <summary>
    ///     Retrieves a list of bugs associated with a specific project.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains a list of <see cref="Bugs" /> objects
    ///     associated with the given project.
    /// </returns>
    public async Task<List<Bugs>> GetBugsAsync(int projectId)
    {
        Logger.LogInformation($"Getting bugs for project {projectId}");
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.Bugs.Where(bp => bp.ProjectsId == projectId).ToListAsync();
    }

    /// <summary>
    ///     Retrieves a list of bugs assigned to the current user within a specific project.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project whose bugs are to be retrieved.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains a list of <see cref="Bugs" /> objects
    ///     assigned to the current user for the specified project.
    /// </returns>
    public async Task<List<Bugs>> GetBugsAssignedToCurrentUser(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.Bugs.Where(rp =>
                rp.ProjectsId == projectId && rp.AssignedTo == userService.GetCurrentUserInfoAsync().Result.UserId)
            .ToListAsync();
    }

    public async Task<Bugs> GetBugByIdAsync(int id)
    {
        Logger.LogInformation($"Getting bug {id}");
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var bug = await db.Bugs.FindAsync(id);
        if (bug is null) throw new Exception("Bug not found");
        return bug;
    }

    public async Task<List<TestCases>> GetTestCasesAssociatedWithBugAsync(int bugId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var bug = await db.Bugs
            .Include(b => b.LinkedTestCases)
            .FirstOrDefaultAsync(b => b.Id == bugId);

        if (bug is null) throw new Exception("Bug not found");
        Logger.LogInformation($"Getting test cases for bug {bugId}");
        return bug.LinkedTestCases.ToList();
    }

    public async Task<Bugs> AddBug(Bugs bug, List<IBrowserFile>? files, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        bug.ProjectsId = projectId;
        bug.LinkedTestCases = new List<TestCases>();

        SetArchivedStatus.SetArchivedStatusBasedOnWorkflow(bug);

        // Fetch and add test cases asynchronously
        foreach (var testCaseId in SelectedTestCasesIds)
        {
            var testCase = await db.TestCases.FindAsync(testCaseId);
            if (testCase is null)
                throw new Exception($"Test case with ID {testCaseId} not found.");

            bug.LinkedTestCases.Add(testCase);
        }

        // Save the bug
        await db.Bugs.AddAsync(bug);


        await db.SaveChangesAsync(userService);

        // If there are files, attempt to save them
        if (files != null && files.Count != 0) await bugsFilesModel.SaveFilesToDb(files, bug.Id, projectId);
        Logger.LogInformation($"Bug {bug.Id} added to project {projectId}");
        return bug;
    }

    public async Task<Bugs> UpdateBugAsync(Bugs updatedBug, List<IBrowserFile>? files, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var bug = await db.Bugs.FindAsync(updatedBug.Id) ?? throw new Exception("Bug not found");


        SetArchivedStatus.SetArchivedStatusBasedOnWorkflow(bug);

        bug.Name = updatedBug.Name;
        bug.Description = updatedBug.Description;
        bug.ProjectsId = projectId;

        bug.LinkedTestCases = updatedBug.LinkedTestCases;
        bug.BugStatus = updatedBug.BugStatus;
        bug.Priority = updatedBug.Priority;
        bug.Severity = updatedBug.Severity;
        bug.WorkflowStatus = updatedBug.WorkflowStatus;
        bug.AssignedTo = updatedBug.AssignedTo;
        bug.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        bug.ModifiedAt = DateTime.UtcNow;
        db.Bugs.Update(bug);

        await db.SaveChangesAsync(userService);

        // If there are files, attempt to save them
        if (files is { Count: > 0 })
            await bugsFilesModel.SaveFilesToDb(files, bug.Id, projectId);
        Logger.LogInformation($"Bug {bug.Id} updated");
        return bug;
    }

    /// <summary>
    ///     Update Card when drag and drop in db for Bugs
    /// </summary>
    /// <param name="args"></param>
    public async Task UpdateCardOnDragDrop(RadzenDropZoneItemEventArgs<Bugs> args)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.Bugs.Update(args.Item);
        await db.SaveChangesAsync(userService);
    }

    public async Task DeleteBug(Bugs bug)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.Bugs.Remove(bug);
        await db.SaveChangesAsync(userService);
    }
}