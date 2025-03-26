using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class TestCasesFilesModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();

    private const int MaxFileSize = 10 * 1024 * 1024; // 10MB


    public List<TestCasesFile> ExistingFiles = [];

    public async Task SaveFilesToDb(List<IBrowserFile>? files, int testCaseId, int projectId)
    {
        if (files != null && files.Count != 0)
        {
            foreach (var file in files)
            {
                using var memoryStream = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(memoryStream);

                //Validation at server side
                if (file.Size > MaxFileSize) throw new Exception("File size is too large. Maximum file size is 100KB");
                var testCaseFile = new TestCasesFile
                {
                    FileName = file.Name,
                    FileContent = memoryStream.ToArray(),
                    UploadedAt = DateTime.UtcNow,
                    TestCaseId = testCaseId,
                    ProjectsId = projectId
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