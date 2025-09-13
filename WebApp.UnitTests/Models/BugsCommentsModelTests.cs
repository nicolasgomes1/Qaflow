using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

public class BugsCommentsModelTests: IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext db;
    private readonly BugsModel bm;
    private readonly BugsCommentsModel bcm;
    
    public BugsCommentsModelTests(TestFixture fixture)
    {
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        bm = fixture.ServiceProvider.GetRequiredService<BugsModel>();
        bcm = fixture.ServiceProvider.GetRequiredService<BugsCommentsModel>();
    }
    
    private readonly BugsComments _comment = new()
    {
        BugId = 1,
        Comment = "Test Comment"
    };
    
    private readonly BugsComments _comment1 = new()
    {
        BugId = 1,
        Comment = "Test Comment 1"
    };

    [Fact]
    public async Task BugsCommentsModel_AddCommentToBug()
    {
        var bug = await db.Bugs.Where(x => x.Name == "Bug 1").FirstOrDefaultAsync();
        if (bug == null) throw new Exception("Bug not found");
        var bugById = await bm.GetBugByIdAsync(bug.Id);
        
        var initialCount = await db.BugsComments.Where(x => x.BugId == bugById.Id).CountAsync();;
        
        await bcm.AddBugComment(bugById.Id, _comment);
        
        var finalCount = await db.BugsComments.Where(x => x.BugId == bugById.Id).CountAsync();;
        
        Assert.Equal(initialCount + 1, finalCount);
    }
    
    [Fact]
    public async Task BugsCommentsModel_GetCommentsToBug()
    {
        var bug = await db.Bugs.Where(x => x.Name == "Bug 1").FirstOrDefaultAsync();
        if (bug == null) throw new Exception("Bug not found");
        var bugById = await bm.GetBugByIdAsync(bug.Id);
        
        
        await bcm.AddBugComment(bugById.Id, _comment);
        await bcm.AddBugComment(bugById.Id, _comment1);

        var final = await bcm.GetBugsComments(bugById.Id);
        
        Assert.Equal(2, final.Count);
    }
    
    [Fact]
    public async Task BugsCommentsModel_DeleteCommentsToBug()
    {
        var bug = await db.Bugs.Where(x => x.Name == "Bug 1").FirstOrDefaultAsync();
        if (bug == null) throw new Exception("Bug not found");
        var bugById = await bm.GetBugByIdAsync(bug.Id);
        
        
        await bcm.AddBugComment(bugById.Id, _comment);
        await bcm.AddBugComment(bugById.Id, _comment1);

        var final = await bcm.GetBugsComments(bugById.Id);
        
        Assert.Equal(2, final.Count);
        
        await bcm.DeleteBugComment(final[0].Id);
        
        final = await bcm.GetBugsComments(bugById.Id);
        
        Assert.Single(final);
    }
    
    [Fact]
    public async Task BugsCommentsModel_UpdateCommentToBug()
    {
        var bug = await db.Bugs.Where(x => x.Name == "Bug 1").FirstOrDefaultAsync();
        if (bug == null) throw new Exception("Bug not found");
        var bugById = await bm.GetBugByIdAsync(bug.Id);
        
        await bcm.AddBugComment(bugById.Id, _comment);
        await bcm.AddBugComment(bugById.Id, _comment1);
        
        var comments = await bcm.GetBugsComments(bugById.Id);
        
        Assert.Equal(2, comments.Count);
        
        var commentToUpdate = comments.FirstOrDefault(x => x.Comment == "Test Comment");
        Assert.NotNull(commentToUpdate);
        
        commentToUpdate.Comment = "Updated Comment";
        await bcm.UpdateBugComment(commentToUpdate.Id, commentToUpdate);
        
        var updatedComments = await bcm.GetBugsComments(bugById.Id);
        var updatedComment = updatedComments.FirstOrDefault(x => x.Id == commentToUpdate.Id);
        
        Assert.NotNull(updatedComment);
        Assert.Equal("Updated Comment", updatedComment.Comment);
    }

}