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


    public BugsModelTest(TestFixture fixture)
    {
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task ddd()
    {
        var count = await db.Projects.CountAsync();

        Assert.Equal(5, count);
    }
}