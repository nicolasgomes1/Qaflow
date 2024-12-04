using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class TestPlansFilesModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ProjectStateService projectSateService)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();
    
    const int MaxFileSize = 10 * 1024 * 1024; // 10MB
    
    public List<TestPlansFile> ExistingFiles = [];
    
    public async Task SaveFilesToDb(List<IBrowserFile>? files, int testPlanId)
    {
        if (files != null && files.Count != 0)
        {
            foreach (var file in files)
            {
                using var memoryStream = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(memoryStream);
                
                //Validation at server side
                if (file.Size > MaxFileSize)
                {
                    throw new Exception("File size is too large. Maximum file size is 100KB");
                }
                var testPlansFile = new TestPlansFile
                {
                    FileName = file.Name,
                    FileContent = memoryStream.ToArray(),
                    UploadedAt = DateTime.UtcNow,
                    TestPlanId = testPlanId,
                    ProjectsId = projectSateService.ProjectId
                };
                
                _dbContext.TestPlansFiles.Add(testPlansFile);
            }
            
            await _dbContext.SaveChangesAsync();
        }
    }
    
    public async Task<List<TestPlansFile>> GetFilesByTestPlanId(int testPlanId)
    {
        ExistingFiles = await _dbContext.TestPlansFiles.Where(tpf => tpf.TestPlanId == testPlanId && tpf.ProjectsId == projectSateService.ProjectId).ToListAsync();
        return ExistingFiles;
    }
    
}