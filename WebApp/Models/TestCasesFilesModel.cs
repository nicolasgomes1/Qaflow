using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class TestCasesFilesModel(IDbContextFactory<ApplicationDbContext> dbContextFactory, ProjectStateService projectSateService)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();

    public List<TestCasesFile> ExistingFiles = [];
    
    public async Task SaveFilesToDb(List<IBrowserFile>? files, int testCaseId)
    {
        if (files != null && files.Count != 0)
        {
            foreach (var file in files)
            {
                using var memoryStream = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(memoryStream);

                var testCaseFile = new TestCasesFile
                {
                    FileName = file.Name,
                    FileContent = memoryStream.ToArray(),
                    UploadedAt = DateTime.UtcNow,
                    TestCaseId = testCaseId,
                    TcfProjectId = projectSateService.ProjectId
                };

                _dbContext.TestCasesFiles.Add(testCaseFile);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
    
    
    public async Task<List<TestCasesFile>> GetFilesByTestCaseId(int testCaseId)
    {
        ExistingFiles = await _dbContext.TestCasesFiles.Where(rf => rf.TestCaseId == testCaseId).ToListAsync();
        return ExistingFiles;
    }
    
}