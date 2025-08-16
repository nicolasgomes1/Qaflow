using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

public class TestCasesModelTests : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext _db;
    private readonly TestCasesModel _tc;

    public TestCasesModelTests(TestFixture fixture)
    {
        _db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _tc = fixture.ServiceProvider.GetRequiredService<TestCasesModel>();
    }

    [Fact]
    public async Task TestCasesModel_AddTestCaseTests()
    {
        var project = await _db.Projects.Where(x => x.Name == "Demo Project With Data").FirstOrDefaultAsync();
        if (project == null) throw new Exception("Project not found");

        var name = $"sample 6969{Guid.NewGuid()}";
        var testCase = new TestCases
        {
            Name = name,
            Description = "Description",
            ProjectsId = project.Id
        };

        await _tc.AddTestCases(testCase, null, project.Id);

        var dbtest = _db.TestCases.Where(x => x.Name == name).FirstOrDefaultAsync();
        Assert.NotNull(dbtest);
    }

    [Fact]
    public async Task TestCasesModel_GetTestCaseDataTests()
    {
        var project = await _db.Projects.Where(x => x.Name == "Demo Project With Data").FirstOrDefaultAsync();
        if (project == null) throw new Exception("Project not found");

        var name = $"sample 6969{Guid.NewGuid()}";
        var testCase = new TestCases
        {
            Name = name,
            Description = "Description",
            ProjectsId = project.Id
        };

        await _tc.AddTestCases(testCase, null, project.Id);

        var dbtest = await _tc.GetTestCasesByIdAsync(testCase.Id);
        Assert.NotNull(dbtest);
    }
}