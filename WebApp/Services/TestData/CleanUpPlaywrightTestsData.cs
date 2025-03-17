using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Services.TestData;

public interface ICleanUpPlaywrightTestsData
{
    Task DeleteAllPlaywrightProjectData();
}

public class CleanUpPlaywrightTestsData : ICleanUpPlaywrightTestsData
{
    private readonly ApplicationDbContext _dbContext;

    public CleanUpPlaywrightTestsData(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DeleteAllPlaywrightProjectData()
    {
        var projects = await _dbContext.Projects
            .Where(p => p.Name.Contains("Playwright"))
            .ToListAsync();

        foreach (var project in projects)
        {
            _dbContext.Projects.Remove(project);
        }

        await _dbContext.SaveChangesAsync();
    }
}