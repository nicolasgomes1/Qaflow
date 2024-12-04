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

    public async Task<Bugs> CreateBug(Bugs bug, List<IBrowserFile>? files)
    {
        bug.CreatedAt = DateTime.UtcNow;
        bug.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        bug.ProjectsId = projectSateService.ProjectId;

        _dbContext.Bugs.Add(bug);
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