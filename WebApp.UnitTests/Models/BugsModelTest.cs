using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

[TestSubject(typeof(BugsModel))]
public class BugsModelTest : TestBase
{
    private readonly TestFixture _fixture;
    private readonly ApplicationDbContext _db;


    public BugsModelTest(TestFixture fixture) : base(fixture)
    {
        _fixture = fixture;

        _db = _fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task ddd()
    {
        var count = await _db.Projects.CountAsync();

        Assert.Equal(5, count);
    }
}