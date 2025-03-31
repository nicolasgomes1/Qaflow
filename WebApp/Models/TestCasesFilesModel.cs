using System.IO.Compression;
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

            var testcasesFile = new TestCasesFile
            {
                FileName = file.Name,
                FileContent = compressedStream.ToArray(),
                UploadedAt = DateTime.UtcNow,
                TestCaseId = testCaseId,
                ProjectsId = projectId
            };

            _dbContext.TestCasesFiles.Add(testcasesFile);
        }

        await _dbContext.SaveChangesAsync();
    }


    public async Task<List<TestCasesFile>> GetFilesByTestCaseId(int testCaseId)
    {
        ExistingFiles = await _dbContext.TestCasesFiles.Where(rf => rf.TestCaseId == testCaseId).ToListAsync();
        return ExistingFiles;
    }
}