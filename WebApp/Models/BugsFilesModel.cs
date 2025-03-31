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


    public async Task SaveFilesToDb(List<IBrowserFile>? files, int bugId, int projectId)
    {
        if (files == null || files.Count == 0) return;

        foreach (var file in files)
        {
            using var compressedStream = await FileCompressing.CompressFileStream(files, file);

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