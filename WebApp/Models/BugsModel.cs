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
            .Include(b => b.LinkedTestCases)
            .FirstOrDefaultAsync(b => b.Id == bugId);

        if (bug is null) throw new Exception("Bug not found");

        return bug.LinkedTestCases.ToList();
    }

    public async Task<Bugs> AddBug(Bugs bug, List<IBrowserFile>? files, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        bug.CreatedAt = DateTime.UtcNow;
        bug.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
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


        await db.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0) await bugsFilesModel.SaveFilesToDb(files, bug.Id, projectId);

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
        bug.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        bug.ModifiedAt = DateTime.UtcNow;
        db.Bugs.Update(bug);

        await db.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files is { Count: > 0 })
            await bugsFilesModel.SaveFilesToDb(files, bug.Id, projectId);

        return bug;
    }
}