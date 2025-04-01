using System.IO.Compression;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class TestCasesFilesModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{

    public async Task SaveFilesToDb(List<IBrowserFile>? files, int testCaseId, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();


        if (files == null || files.Count == 0) return;

        foreach (var file in files)
        {
            using var compressedStream = await FileCompressing.CompressFileStream(files, file);


            var testcasesFile = new TestCasesFile
            {
                FileName = file.Name,
                FileContent = compressedStream.ToArray(),
                UploadedAt = DateTime.UtcNow,
                TestCaseId = testCaseId,
                ProjectsId = projectId
            };

            db.TestCasesFiles.Add(testcasesFile);
        }

        await db.SaveChangesAsync();
    }


    public async Task<List<TestCasesFile>> GetFilesByTestCaseId(int testCaseId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.TestCasesFiles.Where(rf => rf.TestCaseId == testCaseId).ToListAsync();
    }
}