using Microsoft.AspNetCore.Components.Forms;

namespace WebApp.UnitTests.Models.Helpers;

/// <summary>
/// Helper to create files <see cref="IBrowserFile"/> implementation.
/// </summary>
public class TestBrowserFileImpl : IBrowserFile
{
    private readonly byte[] _content;

    public TestBrowserFileImpl(string name, long size)
    {
        Name = name;
        Size = size;
        // Create some test content based on the name
        _content = System.Text.Encoding.UTF8.GetBytes($"Test content for {name}");
    }

    public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = new CancellationToken())
    {
        if (Size > maxAllowedSize)
            throw new InvalidOperationException($"File size ({Size}) exceeds maximum allowed size ({maxAllowedSize})");

        return new MemoryStream(_content);
    }

    public string Name { get; }
    public long Size { get; }
    public DateTimeOffset LastModified => DateTimeOffset.Now;
    public string ContentType => "text/plain";
}