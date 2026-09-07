using System;
using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MGSPW_MC_Cheat_Trainer;


public class LogManager
{
    private const int KilobyteInBytes = 1000;
    private const int MegabyteInKilobytes = 1000 * KilobyteInBytes;
    private const int LogLimitSize = 20 * MegabyteInKilobytes;
    private const int LogFileCountLimit = 5;
    public static string? LogLocation { get; private set; }
    private static LogEventLevel MainLogEventLevel { get; set; } = LogEventLevel.Debug;
    public static ILogger? Logger;
    private static readonly string AppLogFolder = "MGS Mod Manager and Trainer";
    private static readonly string Game = "MGSPW";
    
    public static void StartLogger()
    {
        LogLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppLogFolder,
            Game);
        Logger = InitializeNewLogger("MGSPW_MC_Cheat_Trainer_Log.log", MainLogEventLevel);
        Logger?.Information($"Logging started -- Trainer v{Program.AppVersion}");
    }

    private static ILogger? InitializeNewLogger(string logFileName, LogEventLevel loggingLevel)
    {
        if (LogLocation == null) throw new Exception("Failed to initialize logs!");
        if (!Directory.Exists(LogLocation))
        {
            Directory.CreateDirectory(LogLocation);
        }
        return new LoggerConfiguration().WriteTo.File(Path.Combine(LogLocation, logFileName),
                rollOnFileSizeLimit: true, fileSizeLimitBytes: LogLimitSize,
                retainedFileCountLimit: LogFileCountLimit)
            .MinimumLevel.Is(loggingLevel).CreateLogger();
    }
}