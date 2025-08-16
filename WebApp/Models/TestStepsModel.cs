using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;

namespace WebApp.Models;

public class TestStepsModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<List<TestSteps>> GetTestStepsForTestCase(int id)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.TestSteps.Where(t => t.TestCasesId == id && t.ArchivedStatus == ArchivedStatus.Active)
            .ToListAsync();
    }
}