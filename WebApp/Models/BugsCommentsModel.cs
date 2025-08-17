using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class BugsCommentsModel(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserService userService)
{
    public async Task<List<BugsComments>> GetBugsComments(int bugId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.BugsComments.Where(x => x.BugId == bugId).ToListAsync();
    }

    public async Task AddBugComment(int bugId, BugsComments bugsComment)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        bugsComment.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        bugsComment.CreatedAt = DateTime.UtcNow;
        bugsComment.BugId = bugId;


        await db.BugsComments.AddAsync(bugsComment);
        await db.SaveChangesAsync();
    }
}