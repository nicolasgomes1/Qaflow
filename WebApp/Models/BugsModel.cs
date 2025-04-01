using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;

namespace WebApp.Models;

public class BugsModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    UserService userService,
    BugsFilesModel bugsFilesModel)
{
    public List<int> SelectedTestCasesIds { get; set; } = [];

    public async Task<List<Bugs>> GetBugsAsync(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.Bugs.Where(bp => bp.ProjectsId == projectId).ToListAsync();
    }

    public async Task<Bugs> GetBugByIdAsync(int id)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var bug = await db.Bugs.FindAsync(id);
        if (bug is null) throw new Exception("Bug not found");
        return bug;
    }

    public async Task<List<TestCases>> GetTestCasesAssociatedWithBugAsync(int bugId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var bug = await db.Bugs
            .Include(b => b.TestCases)
            .FirstOrDefaultAsync(b => b.Id == bugId);

        if (bug is null) throw new Exception("Bug not found");

        return bug.TestCases.ToList();
    }

    public async Task<Bugs> AddBug(Bugs bug, List<IBrowserFile>? files, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        bug.CreatedAt = DateTime.UtcNow;
        bug.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        bug.ProjectsId = projectId;
        bug.TestCases = new List<TestCases>();

        // Fetch and add test cases asynchronously
        foreach (var testCaseId in SelectedTestCasesIds)
        {
            var testCase = await db.TestCases.FindAsync(testCaseId);
            if (testCase is null)
                throw new Exception($"Test case with ID {testCaseId} not found.");

            bug.TestCases.Add(testCase);
        }

        // Save the bug
        await db.Bugs.AddAsync(bug);


        await db.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0) await bugsFilesModel.SaveFilesToDb(files, bug.Id, projectId);

        return bug;
    }

    public async Task UpdateBugAsync(int bugId, List<IBrowserFile>? files, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var bug = await db.Bugs.FindAsync(bugId);
        if (bug is null) throw new Exception("Bug not found");

        db.Bugs.Update(bug);
        UpdateArchivedStatus(bug);
        bug.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        bug.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0) await bugsFilesModel.SaveFilesToDb(files, bugId, projectId);
    }

    private static void UpdateArchivedStatus(Bugs bugs)
    {
        if (bugs.WorkflowStatus == WorkflowStatus.Completed)
            bugs.ArchivedStatus = ArchivedStatus.Archived;
    }
}