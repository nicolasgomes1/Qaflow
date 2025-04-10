using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;

namespace WebApp.Models;

public class TestCasesReporting(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<int> GetTestCasesByTestType(int projectId, TestTypes testType)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testCases = await db.TestCases.Where(x => x.ProjectsId == projectId).ToListAsync();

        var testTypeCount = testCases.Count(tc => tc.TestType == testType);
        return testTypeCount;
    }

    public async Task<int> GetTestCasesByTypeAndScope(int projectId, TestTypes testType, TestScope testScope)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        var testCases = await db.TestCases.Where(x => x.ProjectsId == projectId).ToListAsync();
        var testTypeCount = testCases.Count(tc => tc.TestType == testType && tc.TestScope == testScope);
        return testTypeCount;
    }
}