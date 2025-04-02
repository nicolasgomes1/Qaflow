using System.IO.Compression;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class TestPlansFilesModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task SaveFilesToDb(List<IBrowserFile>? files, int testPlanId, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

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

            db.TestPlansFiles.Add(testplanFile);
        }

        await db.SaveChangesAsync();
    }


    public async Task<List<TestPlansFile>> GetFilesByTestPlanId(int testPlanId, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var files = await db.TestPlansFiles
            .Where(tpf => tpf.TestPlanId == testPlanId && tpf.ProjectsId == projectId).ToListAsync();
        return files;
    }
}