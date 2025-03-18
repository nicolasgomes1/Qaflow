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
        
        
        var testCasesDtos = testCases.Select(tc => new TestCasesDto
        {
            Name = tc.Name ?? string.Empty,
            Description = tc.Description ?? string.Empty,
            Priority = tc.Priority,
            ArchivedStatus = tc.ArchivedStatus,
            ProjectId = tc.ProjectsId,
            RequirementsDto = tc.Requirements?.Select(r => new RequirementsDto
            {
                Name = r.Name,
                Description = r.Description,
                Priority = r.Priority.ToString(),
                ArchivedStatus = r.ArchivedStatus,
                ProjectId = r.ProjectsId
            }).ToList() ?? new List<RequirementsDto>()
        }
        ).ToList();

        return testCasesDtos;
    }

    public async Task<TestCasesDto> Api_GetTestCases(int id)
    {
        var testCase = await _dbContext.TestCases
            .Include(tc => tc.Requirements)
            .FirstOrDefaultAsync(tc => tc.Id == id);

        if (testCase is null) return new TestCasesDto();

        var testCaseDto = new TestCasesDto
        {
            Name = testCase.Name ?? string.Empty,
            Description = testCase.Description ?? string.Empty,
            Priority = testCase.Priority,
            ArchivedStatus = testCase.ArchivedStatus,
            ProjectId = testCase.ProjectsId,
            RequirementsDto = testCase.Requirements?.Select(r => new RequirementsDto
            {
                Name = r.Name,
                Description = r.Description,
                Priority = r.Priority.ToString(),
                ArchivedStatus = r.ArchivedStatus,
                ProjectId = r.ProjectsId
            }).ToList() ?? new List<RequirementsDto>()
        };

        return testCaseDto;
    }
}