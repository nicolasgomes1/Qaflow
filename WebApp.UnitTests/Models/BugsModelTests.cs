using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Models;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;
using Xunit.Abstractions;

namespace WebApp.UnitTests.Models;

[TestSubject(typeof(BugsModel))]
public class BugsModelTests : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext db;
    private readonly BugsModel bm;

    public BugsModelTests(TestFixture fixture)
    {
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        bm = fixture.ServiceProvider.GetRequiredService<BugsModel>();
    }

    [Fact]
    public async Task BugModel_GetBugsAsync()
    {
        var project = await db.Projects.Where(x => x.Name == "Demo Project With Data").FirstOrDefaultAsync();
        if (project == null) throw new Exception("Project not found");
        var bugs = await bm.GetBugsAsync(project.Id);

        Assert.NotEmpty(bugs);
        Assert.True(bugs.Count >= 5);
    }

    [Fact]
    public async Task BugModel_GetBugByIdAsync()
    {
        var bug = await db.Bugs.Where(x => x.Name == "Bug 1").FirstOrDefaultAsync();
        if (bug == null) throw new Exception("Bug not found");
        var bugById = await bm.GetBugByIdAsync(bug.Id);

        Assert.NotNull(bugById);
        Assert.Equal(bug.Id, bugById.Id);
    }

    [Fact]
    public async Task BugModel_AddBugAsync()
    {
        var project = await db.Projects.Where(x => x.Name == "Demo Project With Data").FirstOrDefaultAsync();
        if (project == null) throw new Exception("Project not found");
        var newBug = new Bugs
        {
            Name = "Bug 1",
            Description = "Bug 1 description",
            ProjectsId = project.Id
        };

        var countbefore = await db.Bugs.CountAsync();

        await bm.AddBug(newBug, null, project.Id);

        var countafter = await db.Bugs.CountAsync();

        Assert.Equal(countbefore + 1, countafter);
    }

    [Fact]
    public async Task BugModel_AddBugAsync_Status_New()
    {
        var project = await db.Projects.Where(x => x.Name == "Demo Project With Data").FirstOrDefaultAsync();
        if (project == null) throw new Exception("Project not found");
        var newBug = new Bugs
        {
            Name = "Bug 1",
            Description = "Bug 1 description",
            ProjectsId = project.Id,
            WorkflowStatus = WorkflowStatus.New
        };

        var countbefore = await db.Bugs.CountAsync();

        await bm.AddBug(newBug, null, project.Id);

        var countafter = await db.Bugs.CountAsync();

        Assert.True(newBug.WorkflowStatus == WorkflowStatus.New);
        Assert.Equal(ArchivedStatus.Active, newBug.ArchivedStatus);
        Assert.Equal(countbefore + 1, countafter);
    }

    [Fact]
    public async Task BugModel_AddBugAsync_Status_InReview()
    {
        var project = await db.Projects.Where(x => x.Name == "Demo Project With Data").FirstOrDefaultAsync();
        if (project == null) throw new Exception("Project not found");
        var newBug = new Bugs
        {
            Name = "Bug 1",
            Description = "Bug 1 description",
            ProjectsId = project.Id,
            WorkflowStatus = WorkflowStatus.InReview
        };

        var countbefore = await db.Bugs.CountAsync();

        await bm.AddBug(newBug, null, project.Id);

        var countafter = await db.Bugs.CountAsync();

        Assert.True(newBug.WorkflowStatus == WorkflowStatus.InReview);
        Assert.Equal(ArchivedStatus.Active, newBug.ArchivedStatus);
        Assert.Equal(countbefore + 1, countafter);
    }

    [Fact]
    public async Task BugModel_AddBugAsync_Status_Completed()
    {
        var project = await db.Projects.Where(x => x.Name == "Demo Project With Data").FirstOrDefaultAsync();
        if (project == null) throw new Exception("Project not found");
        var newBug = new Bugs
        {
            Name = "Bug 1",
            Description = "Bug 1 description",
            ProjectsId = project.Id,
            WorkflowStatus = WorkflowStatus.Completed
        };

        var countbefore = await db.Bugs.CountAsync();

        await bm.AddBug(newBug, null, project.Id);

        var countafter = await db.Bugs.CountAsync();

        Assert.True(newBug.WorkflowStatus == WorkflowStatus.Completed);
        Assert.Equal(ArchivedStatus.Archived, newBug.ArchivedStatus);
        Assert.Equal(countbefore + 1, countafter);
    }

    [Fact]
    public async Task BugModel_UpdateBugAsync()
    {
        var project = await db.Projects.Where(x => x.Name == "Demo Project With Data").FirstOrDefaultAsync();
        if (project == null) throw new Exception("Project not found");
        var newBug = new Bugs
        {
            Name = "Bug 1",
            Description = "Bug 1 description",
            ProjectsId = project.Id,
            WorkflowStatus = WorkflowStatus.New
        };
        await bm.AddBug(newBug, null, project.Id);

        var updatedBug = await bm.GetBugByIdAsync(newBug.Id);

        updatedBug.Name = "Bug 1 updated";
        updatedBug.Description = "Bug 1 description updated";

        db.Bugs.Update(updatedBug);


        Assert.Equal("Bug 1 updated", updatedBug.Name);
        Assert.Equal("Bug 1 description updated", updatedBug.Description);
    }

    [Fact]
    public async Task BugModel_UpdateBugAsync_Completed()
    {
        // Arrange
        var project = await db.Projects.FirstOrDefaultAsync(x => x.Name == "Demo Project With Data");
        Assert.NotNull(project);

        var newBug = new Bugs
        {
            Name = "Bug 1",
            Description = "Bug 1 description",
            ProjectsId = project.Id,
            WorkflowStatus = WorkflowStatus.Completed
        };
        await bm.AddBug(newBug, null, project.Id);

        // Act
        var bugToUpdate = await bm.GetBugByIdAsync(newBug.Id);
        Assert.NotNull(bugToUpdate);

        bugToUpdate.Name = "Bug 1 updated";
        bugToUpdate.Description = "Bug 1 description updated";
        bugToUpdate.WorkflowStatus = WorkflowStatus.Completed;

        await bm.UpdateBugAsync(bugToUpdate, null, project.Id);

        // Retrieve the bug again from the database to ensure changes were saved
        var finalBug = await db.Bugs.FirstOrDefaultAsync(x => x.Id == bugToUpdate.Id);

        // Assert
        Assert.NotNull(finalBug);
        Assert.Equal("Bug 1 updated", finalBug.Name);
        Assert.Equal("Bug 1 description updated", finalBug.Description);
        Assert.Equal(WorkflowStatus.Completed, finalBug.WorkflowStatus);

        // Check ArchivedStatus property based on workflow status
        Assert.Equal(ArchivedStatus.Archived, finalBug.ArchivedStatus);
    }

    [Fact]
    public async Task BugModel_DeleteBug()
    {
        var intialCount = await db.Bugs.CountAsync();
        var bug = await db.Bugs.Where(x => x.Name == "Bug 1").FirstOrDefaultAsync();
        if (bug == null) throw new Exception("Bug not found");
        await bm.DeleteBug(bug);
        var finalCount = await db.Bugs.CountAsync();
        Assert.Equal(intialCount - 1, finalCount);
    }
}