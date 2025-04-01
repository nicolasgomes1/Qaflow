using System.IO.Compression;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class RequirementsFilesModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task SaveFilesToDb(List<IBrowserFile>? files, int requirementId, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

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

            db.RequirementsFiles.Add(requirementsFile);
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves a list of files associated with a specific requirement ID.
    /// </summary>
    /// <param name="requirementId">The unique identifier of the requirement for which to retrieve associated files.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="RequirementsFile"/> objects associated with the specified requirement ID.</returns>
    public async Task<List<RequirementsFile>> GetFilesByRequirementId(int requirementId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var existingFiles = await db.RequirementsFiles.Where(rf => rf.RequirementsId == requirementId)
            .ToListAsync();
        return existingFiles;
    }
}