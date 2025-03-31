using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Forms;

public class BugsFilesModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();
    private const int MaxFileSize = 10 * 1024 * 1024; // 10MB
    public List<BugsFiles> ExistingFiles = [];

    public async Task SaveFilesToDb(List<IBrowserFile>? files, int bugId, int projectId)
    {
        var logger = LoggerFactory.Create(builder => builder.AddConsole());

        if (files == null || files.Count == 0) return;

        foreach (var file in files)
        {
            using var memoryStream = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(memoryStream);

            // Validation
            if (file.Size > MaxFileSize)
                throw new Exception("File size is too large. Maximum file size is 10MB");

            var compressedData = CompressData(memoryStream.ToArray());

            var bugsFile = new BugsFiles
            {
                FileName = file.Name,
                FileContent = compressedData, // Store compressed data
                UploadedAt = DateTime.UtcNow,
                BugId = bugId,
                ProjectsId = projectId
            };

            _dbContext.BugsFiles.Add(bugsFile);
            logger.CreateLogger("FileLogger")
                .LogInformation($"[INFO] File '{file.Name}' uploaded at {DateTime.UtcNow}");
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<BugsFiles>> GetBugFilesById(int bugId)
    {
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("FileLogger");

        ExistingFiles = await _dbContext.BugsFiles.Where(bf => bf.BugId == bugId).ToListAsync();

        // Decompress file content before returning
        foreach (var file in ExistingFiles)
        {
            file.FileContent = DecompressData(file.FileContent);
            logger.LogInformation($"[INFO] File '{file.FileName}' decompressed.");
        }

        return ExistingFiles;
    }

    private byte[] CompressData(byte[] data)
    {
        using var outputStream = new MemoryStream();
        using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal))
        {
            gzipStream.Write(data, 0, data.Length);
        }

        return outputStream.ToArray();
    }

    private byte[] DecompressData(byte[] compressedData)
    {
        using var inputStream = new MemoryStream(compressedData);
        using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
        using var outputStream = new MemoryStream();
        gzipStream.CopyTo(outputStream);
        return outputStream.ToArray();
    }
}