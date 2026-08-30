using System;
using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MGSPW_MC_Cheat_Trainer;

//TODO: for some reason this just *isn't* working. Definitely need to figure this out, but at later date

public interface ILogManager
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, params object[] args);
    void LogError(Exception ex, string message, params object[] args);
    void LogDebug(string message, params object[] args);
    void LogFatal(string message, params object[] args);
}

public class LogManager : ILogManager, IDisposable
{
    private readonly Logger? _logger;
    public LogManager()
    {
        string userDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string logDirectory = Path.Combine(userDocuments, "MGS Mod Manager and Trainer", "MGSPW");
        _logger = new LoggerConfiguration().WriteTo.File(Path.Combine(logDirectory, "MGSPW_MC_CheatTrainer_Log.log"), rollOnFileSizeLimit: false).
            MinimumLevel.Is(LogEventLevel.Verbose).CreateLogger();
        LogInformation($"Logging started -- Trainver v{Program.AppVersion}");
    }

    public void LogInformation(string message, params object[] args)
        => _logger?.Information(message, args);

    public void LogWarning(string message, params object[] args)
        => _logger?.Warning(message, args);

    public void LogError(string message, params object[] args)
        => _logger?.Error(message, args);

    public void LogError(Exception ex, string message, params object[] args)
        => _logger?.Error(ex, message, args);

    public void LogDebug(string message, params object[] args)
        => _logger?.Debug(message, args);

    public void LogFatal(string message, params object[] args)
        => _logger?.Fatal(message, args);

    public void Dispose()
    {
        // Flush and close all sinks cleanly on shutdown
        Serilog.Log.CloseAndFlush();
    }
}