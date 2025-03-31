using System.IO.Compression;
using Microsoft.AspNetCore.Components.Forms;

namespace WebApp.Services;

public static class FileCompressing
{
    private const int MaxFileSize = 10 * 1024 * 1024; // 10MB

    public static async Task<MemoryStream> CompressFileStream(List<IBrowserFile> files, IBrowserFile file)
    {
        MemoryStream? compressedStream = null;
        try
        {
            using var memoryStream = new MemoryStream();
            await file.OpenReadStream(MaxFileSize * files.Count).CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset stream position before compression

            compressedStream = new MemoryStream();
            await using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
            {
                await memoryStream.CopyToAsync(gzipStream);
            }

            compressedStream.Position = 0;
            return compressedStream;
        }
        catch
        {
            compressedStream?.Dispose();
            throw;
        }
    }
}