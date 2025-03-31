using System.IO.Compression;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class TestPlansFilesModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();

    private const int MaxFileSize = 10 * 1024 * 1024; // 10MB

    public List<TestPlansFile> ExistingFiles = [];

    public async Task SaveFilesToDb(List<IBrowserFile>? files, int testPlanId, int projectId)
    {
        if (files == null || files.Count == 0) return;

        foreach (var file in files)
        {
            using var compressedStream = await FileCompressing.CompressFileStream(files, file);


            var testplanFile = new TestPlansFile
            {
                FileName = file.Name,
                FileContent = compressedStream.ToArray(),
                UploadedAt = DateTime.UtcNow,
                TestPlanId = testPlanId,
                ProjectsId = projectId
            };

            _dbContext.TestPlansFiles.Add(testplanFile);
        }

        await _dbContext.SaveChangesAsync();
    }


    public async Task<List<TestPlansFile>> GetFilesByTestPlanId(int testPlanId, int projectId)
    {
        ExistingFiles = await _dbContext.TestPlansFiles
            .Where(tpf => tpf.TestPlanId == testPlanId && tpf.ProjectsId == projectId).ToListAsync();
        return ExistingFiles;
    }
}