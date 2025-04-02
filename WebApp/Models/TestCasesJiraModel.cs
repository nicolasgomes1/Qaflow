using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Models;

public class TestCasesJiraModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();


    public async Task<List<TestCasesJira>> GetSelectedJiraTickets(int testCaseId)
{
    return await _dbContext.TestCasesJira.Where(x => x.TestCases != null && x.TestCases.Id == testCaseId).ToListAsync();
}
    
}