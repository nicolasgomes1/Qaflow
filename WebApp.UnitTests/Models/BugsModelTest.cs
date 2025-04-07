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
    public async Task ddd()
    {
        var project = await db.Projects.CountAsync();
        var requirement = await db.Requirements.CountAsync();
        var requirmentsspec = await db.RequirementsSpecification.CountAsync();
        var testcase = await db.TestCases.CountAsync();
        var testplans = await db.TestPlans.CountAsync();

        output.WriteLine($"Total Projects: {project.ToString()}");
        output.WriteLine($"Total Requirements: {requirement.ToString()}");
        output.WriteLine($"Total Requirements Specification: {requirmentsspec.ToString()}");
        output.WriteLine($"Total Test Cases: {testcase.ToString()}");
        output.WriteLine($"Total Test Plans: {testplans.ToString()}");
    }
}