using System.IO.Compression;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class RequirementsFilesModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();

    public List<RequirementsFile> ExistingFiles = [];

    public async Task SaveFilesToDb(List<IBrowserFile>? files, int requirementId, int projectId)
    {
        if (files == null || files.Count == 0) return;

        foreach (var file in files)
        {
            using var compressedStream = await FileCompressing.CompressFileStream(files, file);


            var requirementsFile = new RequirementsFile
            {
                FileName = file.Name,
                FileContent = compressedStream.ToArray(),
                UploadedAt = DateTime.UtcNow,
                RequirementsId = requirementId,
                ProjectsId = projectId
            };

            _dbContext.RequirementsFiles.Add(requirementsFile);
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Return the list of files for a given requirement
    /// </summary>
    /// <param name="requirementId"></param>
    /// <returns></returns>
    public async Task<List<RequirementsFile>> GetFilesByRequirementId(int requirementId)
    {
        ExistingFiles = await _dbContext.RequirementsFiles.Where(rf => rf.RequirementsId == requirementId)
            .ToListAsync();
        return ExistingFiles;
    }
}