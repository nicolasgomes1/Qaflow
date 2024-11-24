using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class BugsFilesModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ProjectStateService projectSateService)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();

    private const int MaxFileSize = 10 * 1024 * 1024; // 10MB

    public List<BugsFiles> ExistingFiles = [];

    public async Task SaveFilesToDb(List<IBrowserFile>? files, int bugId)
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

                var bugsFile = new BugsFiles
                {
                    FileName = file.Name,
                    FileContent = memoryStream.ToArray(),
                    UploadedAt = DateTime.UtcNow,
                    BugId = bugId,
                    BfProjectId = projectSateService.ProjectId
                };

                _dbContext.BugsFiles.Add(bugsFile);
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<BugsFiles>> GetBugFilesById(int bugId)
    {
        ExistingFiles = await _dbContext.BugsFiles.Where(bf => bf.BugId == bugId).ToListAsync();
        return ExistingFiles;
    }
}