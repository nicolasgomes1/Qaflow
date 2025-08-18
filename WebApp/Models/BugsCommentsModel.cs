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

        // bugsComment.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        // bugsComment.CreatedAt = DateTime.UtcNow;
        bugsComment.BugId = bugId;


        await db.BugsComments.AddAsync(bugsComment);
        await db.SaveChangesAsync(userService);
    }

    public async Task DeleteBugComment(int bugCommentId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        var bugComment = await db.BugsComments.FindAsync(bugCommentId);
        if (bugComment is null) throw new Exception("Bug Comment not found");
        db.BugsComments.Remove(bugComment);
        await db.SaveChangesAsync();
    }

    public async Task UpdateBugComment(int bugCommentId, BugsComments bugsComment)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        var bugCommentDb = await db.BugsComments.FindAsync(bugCommentId);
        if (bugCommentDb is null) throw new Exception("Bug Comment not found");

        bugCommentDb.Comment = bugsComment.Comment;
        //  bugCommentDb.ModifiedAt = DateTime.UtcNow;
        //  bugCommentDb.ModifiedBy = (await userService.GetCurrentUserInfoAsync()).UserName;

        db.BugsComments.Update(bugCommentDb);
        await db.SaveChangesAsync(userService);
    }
}