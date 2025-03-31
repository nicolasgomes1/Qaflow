using System.IO.Compression;

namespace WebApp.Services;

public static class FileCompressing
{
    private static byte[] CompressData(byte[] data)
    {
        using var outputStream = new MemoryStream();
        using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal))
        {
            gzipStream.Write(data, 0, data.Length);
        }
        return outputStream.ToArray();
    }

    private static byte[] DecompressData(byte[] compressedData)
    {
        using var inputStream = new MemoryStream(compressedData);
        using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
        using var outputStream = new MemoryStream();
        gzipStream.CopyTo(outputStream);
        return outputStream.ToArray();
    }
}