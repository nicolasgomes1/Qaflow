using Microsoft.EntityFrameworkCore;
using WebApp.Api.Dto;
using WebApp.Data;

namespace WebApp.Api;

public class FetchApiData
{
    private readonly ApplicationDbContext _dbContext;
    
    public FetchApiData(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TestCasesDto>> Api_GetListTestCases()
    {
        var testCases = await _dbContext.TestCases
            .Include(tc => tc.Requirements)
            .ToListAsync();

        var dtos = testCases.Select(tc => new TestCasesDto
        {
            Name = tc.Name ?? string.Empty,
            Description = tc.Description ?? string.Empty,
            Priority = tc.Priority,
            ArchivedStatus = tc.ArchivedStatus,
            ProjectId = tc.ProjectsId,
            RequirementsDto = tc.Requirements.Select(r => new RequirementsDto
            {
                Name = r.Name,
                Description = r.Description,
                Priority = r.Priority.ToString(),
                ArchivedStatus = r.ArchivedStatus,
                ProjectId = r.ProjectsId
            }).ToList()
        }).ToList();

        return dtos;
    }
}