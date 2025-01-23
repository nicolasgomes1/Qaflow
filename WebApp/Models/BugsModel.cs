using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class BugsModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    UserService userService,
    ProjectStateService projectSateService,
    BugsFilesModel bugsFilesModel)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();

    public Bugs Bugs { get; set; } = new();

    public List<int> SelectedTestCasesIds { get; set; } = [];

    public async Task<List<Bugs>> GetBugsAsync()
    {
        return await _dbContext.Bugs.Where(bp => bp.ProjectsId == projectSateService.ProjectId).ToListAsync();
    }

    public async Task<Bugs> GetBugByIdAsync(int id)
    {
        var bug = await _dbContext.Bugs.FindAsync(id);
        if (bug == null) throw new Exception("Bug not found");
        return bug;
    }
    
    public async Task<List<TestCases>> GetTestCasesAssociatedWithBugAsync(int bugId)
    {
        var bug = await _dbContext.Bugs
            .Include(b => b.TestCases)
            .FirstOrDefaultAsync(b => b.Id == bugId);

        if (bug is null) throw new Exception("Bug not found");

        return bug.TestCases.ToList();
    }

    public async Task<Bugs> CreateBug(Bugs bug, List<IBrowserFile>? files)
    {
        bug.CreatedAt = DateTime.UtcNow;
        bug.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        bug.ProjectsId = projectSateService.ProjectId;
        bug.TestCases = new List<TestCases>();

        // Fetch and add test cases asynchronously
        foreach (var testCaseId in SelectedTestCasesIds)
        {
            var testCase = await _dbContext.TestCases.FindAsync(testCaseId);
            if (testCase == null)
                throw new Exception($"Test case with ID {testCaseId} not found.");

            bug.TestCases.Add(testCase);
        }

        // Save the bug
        await _dbContext.Bugs.AddAsync(bug);
        
        
        await _dbContext.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0)
        {
            await bugsFilesModel.SaveFilesToDb(files, bug.Id);
        }

        return bug;
    }

    public async Task UpdateBugAsync(Bugs bug, List<IBrowserFile>? files)
    {
        _dbContext.Bugs.Update(bug);
        Bugs.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        Bugs.ModifiedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0)
        {
            await bugsFilesModel.SaveFilesToDb(files, bug.Id);
        }
    }
}