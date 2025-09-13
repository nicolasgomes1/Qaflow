using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Models;

public class TestStepExecutionFileModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    private static readonly ILoggerFactory LoggerFactory =
        Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());

    private static readonly ILogger Logger = LoggerFactory.CreateLogger(nameof(TestStepExecutionFileModel));

    public byte[]? FileBytes = [];


    public async Task UploadFileToDatabase(IBrowserFile? selectedFile, int testStepExecutionId, int projectId)
    {
        if (selectedFile == null)
        {
            Logger.LogInformation(@"No file selected for upload.");
            return;
        }

        await using var db = await dbContextFactory.CreateDbContextAsync();
        try
        {
            using var stream = new MemoryStream();
            await selectedFile.OpenReadStream().CopyToAsync(stream);
            FileBytes = stream.ToArray();

            var testStepExecution = await db.TestStepsExecution.FindAsync(testStepExecutionId);
            if (testStepExecution == null)
            {
                Logger.LogInformation(@"Test Step Execution not found.");
                return;
            }

            var existingFile = await db.TestStepsExecutionFiles
                .FirstOrDefaultAsync(file => file.TestStepExecutionId == testStepExecutionId);

            var fileToSave = existingFile ?? new TestStepsExecutionFile
            {
                TestStepExecutionId = testStepExecutionId,
                CreatedAt = DateTime.UtcNow
            };

            fileToSave.FileContent = FileBytes;
            fileToSave.FileName = selectedFile.Name;
            fileToSave.UploadedAt = DateTime.UtcNow;
            fileToSave.TSEFProjectId = projectId;

            if (existingFile == null)
            {
                db.TestStepsExecutionFiles.Add(fileToSave);
                Logger.LogInformation(@"New file uploaded and saved to the database.");
            }
            else
            {
                Logger.LogInformation(@"Existing file updated in the database.");
            }

            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(@"Error uploading file: {msg}", ex.Message);
        }
    }
    
    
    /// <summary>
    /// Returns a list of test steps execution files for the current project
    /// </summary>
    /// <returns></returns>
    public async Task<List<TestStepsExecutionFile>> GetTestStepsExecutionFiles(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.TestStepsExecutionFiles
            .Where(tsef => tsef.TSEFProjectId == projectId)
            .ToListAsync();
    }
}