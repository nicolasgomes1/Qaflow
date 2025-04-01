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
        await DeletePlaywrightProjectData();
        await DeletePlaywrightRequirementsSpecificationData();
        await DeletePlaywrightRequirementsData();
        await DeletePlaywrightTestExecutionData();
    }

    private async Task DeletePlaywrightProjectData()
    {
        var projects = await _dbContext.Projects
            .Where(p => p.Name.Contains("Playwright"))
            .ToListAsync();

        foreach (var project in projects) _dbContext.Projects.Remove(project);

        await _dbContext.SaveChangesAsync();
    }

    private async Task DeletePlaywrightRequirementsSpecificationData()
    {
        var data = await _dbContext.RequirementsSpecification
            .Where(p => p.Name.Contains("Playwright"))
            .ToListAsync();

        foreach (var item in data) _dbContext.RequirementsSpecification.Remove(item);

        await _dbContext.SaveChangesAsync();
    }

    private async Task DeletePlaywrightRequirementsData()
    {
        var data = await _dbContext.Requirements
            .Where(p => p.Name != null && p.Name.Contains("Playwright"))
            .ToListAsync();

        foreach (var item in data) _dbContext.Requirements.Remove(item);

        await _dbContext.SaveChangesAsync();
    }

    private async Task DeletePlaywrightTestExecutionData()
    {
        var data = await _dbContext.TestExecution
            .Where(p => p.Name != null && p.Name.Contains("Playwright"))
            .ToListAsync();

        foreach (var item in data) _dbContext.TestExecution.Remove(item);

        await _dbContext.SaveChangesAsync();
    }
}