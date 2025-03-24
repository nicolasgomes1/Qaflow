namespace WebApp.Services;

public static class LoggerService
{
    private static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });

    public static readonly ILogger Logger = LoggerFactory.CreateLogger("StaticLogger");
}