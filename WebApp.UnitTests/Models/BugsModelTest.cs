using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;
using Xunit.Abstractions;

namespace WebApp.UnitTests.Models;

[TestSubject(typeof(BugsModel))]
public class BugsModelTest : IClassFixture<TestFixture>
{
    private readonly ITestOutputHelper output;
    private readonly ApplicationDbContext db;
    private readonly BugsModel bm;

    public BugsModelTest(ITestOutputHelper helper, TestFixture fixture)
    {
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        bm = fixture.ServiceProvider.GetRequiredService<BugsModel>();
        output = helper;
    }

    [Fact]
    public async Task BugModel_GetBugsAsync()
    {
        var project = await db.Projects.Where(x => x.Name == "Demo Project With Data").FirstOrDefaultAsync();

        var bugs = await bm.GetBugsAsync(project.Id);

        Assert.NotEmpty(bugs);
        Assert.True(bugs.Count >= 5);
    }

    [Fact]
    public async Task BugModel_GetBugByIdAsync()
    {
        var bug = await db.Bugs.Where(x => x.Name == "Bug 1").FirstOrDefaultAsync();

        var bugById = await bm.GetBugByIdAsync(bug.Id);

        Assert.NotNull(bugById);
        Assert.Equal(bug.Id, bugById.Id);
    }
}