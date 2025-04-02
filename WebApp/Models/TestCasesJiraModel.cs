using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Models;

public class TestCasesJiraModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<List<TestCasesJira>> GetSelectedJiraTickets(int testCaseId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.TestCasesJira.Where(x => x.TestCases != null && x.TestCases.Id == testCaseId).ToListAsync();
    }
}