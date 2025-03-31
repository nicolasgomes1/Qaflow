using System.IO.Compression;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class BugsFilesModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();

    private const int MaxFileSize = 10 * 1024 * 1024; // 10MB

    public List<BugsFiles> ExistingFiles = [];

    public async Task SaveFilesToDb1(List<IBrowserFile>? files, int bugId, int projectId )
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
                    ProjectsId = projectId
                };

                _dbContext.BugsFiles.Add(bugsFile);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
    
    public async Task SaveFilesToDb(List<IBrowserFile>? files, int bugId, int projectId)
{
    if (files == null || files.Count == 0) return;

    foreach (var file in files)
    {
        if (file.Size > MaxFileSize)
            throw new Exception($"File size is too large. Maximum file size is {MaxFileSize} bytes");

        using var memoryStream = new MemoryStream();
        await file.OpenReadStream(MaxFileSize * files.Count).CopyToAsync(memoryStream);
        memoryStream.Position = 0; // Reset stream position before compression

        using var compressedStream = new MemoryStream();
        await using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
        {
            await memoryStream.CopyToAsync(gzipStream);
        }

        compressedStream.Position = 0;

        var bugsFile = new BugsFiles
        {
            FileName = file.Name,
            FileContent = compressedStream.ToArray(),
            UploadedAt = DateTime.UtcNow,
            BugId = bugId,
            ProjectsId = projectId
        };

        _dbContext.BugsFiles.Add(bugsFile);
    }

    await _dbContext.SaveChangesAsync();
}


    public async Task<List<BugsFiles>> GetBugFilesById(int bugId)
    {
        ExistingFiles = await _dbContext.BugsFiles.Where(bf => bf.BugId == bugId).ToListAsync();
        return ExistingFiles;
    }
}