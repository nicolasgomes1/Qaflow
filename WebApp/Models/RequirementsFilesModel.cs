using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class RequirementsFilesModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ProjectStateService projectSateService)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();

    private const int MaxFileSize = 10 * 1024 * 1024; // 10MB

    public List<RequirementsFile> ExistingFiles = [];

    public async Task SaveFilesToDb(List<IBrowserFile>? files, int requirementId)
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
                var requirementsFile = new RequirementsFile
                {
                    FileName = file.Name,
                    FileContent = memoryStream.ToArray(),
                    UploadedAt = DateTime.UtcNow,
                    RequirementId = requirementId,
                    ProjectsId = projectSateService.ProjectId
                };

                _dbContext.RequirementsFiles.Add(requirementsFile);
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Return the list of files for a given requirement
    /// </summary>
    /// <param name="requirementId"></param>
    /// <returns></returns>
    public async Task<List<RequirementsFile>> GetFilesByRequirementId(int requirementId)
    {
        ExistingFiles = await _dbContext.RequirementsFiles.Where(rf => rf.RequirementId == requirementId).ToListAsync();
        return ExistingFiles;
    }
}