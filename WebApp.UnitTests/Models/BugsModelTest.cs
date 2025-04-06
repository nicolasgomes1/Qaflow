using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

[TestSubject(typeof(BugsModel))]
public class BugsModelTest : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext db;

    private readonly BugsModel bm;

    public BugsModelTest(TestFixture fixture)
    {
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        bm = fixture.ServiceProvider.GetRequiredService<BugsModel>();
    }

    [Fact]
    public async Task ddd()
    {
        var project = await db.Projects.FindAsync(1);
        if (project == null) return;
        var rr =await db.Bugs.CountAsync();
        await bm.GetBugByIdAsync(project.Id);
        Assert.Equal(4, rr);
    }
}