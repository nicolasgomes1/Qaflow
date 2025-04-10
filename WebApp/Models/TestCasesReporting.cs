using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;

namespace WebApp.Models;

public class TestCasesReporting(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<int> GetTestCasesByTestType(int projectId, TestTypes testType)
    {
        using var cts = new CancellationTokenSource(); // auto-cancel after 10s

        using var factory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = factory.CreateLogger("Reporting");

        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testCases = await db.TestCases.Where(x => x.ProjectsId == projectId).ToListAsync();

        var testTypeCount = testCases.Count(tc => tc.TestType == testType);
        logger.LogInformation($"Test Type {testType} has {testTypeCount} test cases");
        return testTypeCount;
    }

    public async Task<int> GetTestCasesByTypeAndScope(int projectId, TestTypes testType, TestScope testScope)
    {
        using var factory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = factory.CreateLogger("Reporting");

        await using var db = await dbContextFactory.CreateDbContextAsync();
        var testCases = await db.TestCases.Where(x => x.ProjectsId == projectId).ToListAsync();
        var testTypeCount = testCases.Count(tc => tc.TestType == testType && tc.TestScope == testScope);
        logger.LogInformation($"Test Type {testType} has {testTypeCount} test cases");
        return testTypeCount;
    }
}