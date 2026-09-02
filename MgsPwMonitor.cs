using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MGSPW_MC_Cheat_Trainer.Models;
using Serilog;
using SimplifiedMemoryManager;

namespace MGSPW_MC_Cheat_Trainer;

public static class MgsPwMonitor
{
    #region Members & fields
    private const string MgsPwProcessName = "METAL GEAR SOLID PEACE WALKER.exe";
    private const string DesiredVersion = "2.1.0.0"; //TODO: get real app ver
    private static bool _versionWarned;

    private static Process? _mgsPwProcess;

    private static CancellationToken MonitorCancellationToken { get; set; }
    private static CancellationTokenSource MgsPwCancellationTokenSource { get; } = new ();
    //private static ILogger? Logger => Logging.Logger;
    private static Thread? ScanningThread { get; set; }
        
    public static event EventHandler<bool>? OnGameHooked;
    public static event EventHandler<string>? OnInvalidVersionDetected;
    #endregion
        
    #region Functions
    #region Event Handlers & Delegates

    private static void GameHooked(bool hooked)
    {
        OnGameHooked?.Invoke(null, hooked);
    }
        
    private static void TearDownMonitor()
    {
        OnGameHooked?.Invoke(null, false);
    }

    private static void InvalidVersionDetected(string message)
    {
        OnInvalidVersionDetected?.Invoke(null, message);
    }
    #endregion
    
    #region Threads
        private static void ScanForMgsPw()
        {
            while (!MonitorCancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (MgsPwProcess == null)
                    {
                        Process? process = null;

                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            process = Process.GetProcessesByName(MgsPwProcessName).FirstOrDefault();
                        else
                        {
                            Process[] processes = Process.GetProcessesByName("METAL");
                            if (processes.Length == 1)
                                process = processes[0];
                            else if (processes.Length == 0)
                                processes = Process.GetProcesses();

                            var test = processes.FirstOrDefault(x => x.MainWindowTitle.Contains("Peace Walker"));

                            if (process is null)
                            {
                                foreach (Process p in processes)
                                {
                                    //if (!p.ProcessName.Contains("METAL")) continue;
                                    try
                                    {
                                        using SimpleProcessProxy spp = new SimpleProcessProxy(p, MgsPwProcessName);
                                        nint signifyingMemory = 0x1591501;
                                        string determinantString = "METAL GEAR SOLID PEACE WALKER";
                                        long bytesToRead = determinantString.Length;
                                        try
                                        {
                                            byte[] memory = spp.ReadProcessOffset(signifyingMemory, bytesToRead);
                                            string decodedString = Encoding.UTF8.GetString(memory);
                                            if (determinantString.Equals(decodedString))
                                                process = p;
                                        }
                                        catch(Exception ex)
                                        {
                                            // ignored
                                        }
                                    }
                                    catch
                                    {
                                        //ignored
                                    }
                                }
                            }
                        }

                        if (process != null)
                        {
                            // Bug fix: only update if process actually changed
                            if (MgsPwProcess?.Id != process.Id)
                            {
                                MgsPwProcess = process;
                                GameHooked(true);
                                string? fileVersionString;
                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                {
                                    FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(
                                        MgsPwProcess.MainModule?.FileName!);
                                    fileVersionString = fileVersion.ProductVersion;
                                }
                                else
                                {
                                    //Linux path
                                    string? gameExePath = FindGameExePath(process.Id);
                                    if (gameExePath != null && File.Exists(gameExePath))
                                        fileVersionString = GetVersionFromPeFile(gameExePath);
                                    else
                                        fileVersionString = "UNKNOWN!";
                                }

                                //Logger?.Information($"MGSPW found and hooked, game version: {fileVersionString}");

                                if (string.Compare(fileVersionString, DesiredVersion,
                                        StringComparison.InvariantCultureIgnoreCase) != 0
                                    && !_versionWarned)
                                {
                                    _versionWarned = true;
                                    InvalidVersionDetected(
                                        $"The version of MGSPW we have hooked({fileVersionString}) " +
                                        $"does not match expected({DesiredVersion})!");
                                }
                            }

                            Thread.Sleep(60 * Constants.MillisecondsInSecond);
                        }
                        else
                        {
                            if (MgsPwProcess != null)
                                MgsPwProcess = null;
                            Thread.Sleep(10 * Constants.MillisecondsInSecond);
                        }
                    }
                }
                catch (Exception e)
                {
                    //Logger?.Error($"Something went wrong in ScanningThread: {e}");
                }
            }
        }
        
        private static string? FindGameExePath(int pid)
        {
            try
            {
                foreach (string line in File.ReadLines($"/proc/{pid}/maps"))
                {
                    // Find the path by taking everything from the first '/' onwards
                    int pathStart = line.IndexOf('/');
                    if (pathStart < 0) continue;

                    string path = line.Substring(pathStart).Trim();

                    if (!path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) continue;

                    // Verify it's a readable mapping by checking the permissions field
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 2 || !parts[1].Contains('r')) continue;

                    return path;
                }
            }
            catch
            {
                //Squelch errors.
            }
            return null;
        }
        
        private static string? GetVersionFromPeFile(string exePath)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(exePath);

                // Search for the VS_VERSION_INFO signature
                byte[] signature = Encoding.Unicode.GetBytes("VS_VERSION_INFO");
                for (int i = 0; i < fileBytes.Length - signature.Length; i++)
                {
                    bool match = true;
                    for (int j = 0; j < signature.Length; j++)
                    {
                        if (fileBytes[i + j] != signature[j])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        // Version numbers are at fixed offsets from VS_VERSION_INFO
                        // MS-DOS structure: after the wLength(2), wValueLength(2), wType(2), szKey
                        // then padding to DWORD boundary, then VS_FIXEDFILEINFO
                        int fixedInfoOffset = i + signature.Length + 2; // skip null terminator + padding
                        fixedInfoOffset = (fixedInfoOffset + 3) & ~3; // align to DWORD

                        if (fixedInfoOffset + 52 >= fileBytes.Length) continue;

                        // VS_FIXEDFILEINFO starts with dwSignature 0xFEEF04BD
                        uint sig = BitConverter.ToUInt32(fileBytes, fixedInfoOffset);
                        if (sig != 0xFEEF04BD) continue;

                        // FileVersion is at offset 8 in VS_FIXEDFILEINFO
                        ushort major = BitConverter.ToUInt16(fileBytes, fixedInfoOffset + 10);
                        ushort minor = BitConverter.ToUInt16(fileBytes, fixedInfoOffset + 8);
                        ushort build = BitConverter.ToUInt16(fileBytes, fixedInfoOffset + 14);
                        ushort revision = BitConverter.ToUInt16(fileBytes, fixedInfoOffset + 12);

                        return $"{major}.{minor}.{build}.{revision}";
                    }
                }
            }
            catch
            {
                //Squelch error
            }
            return null;
        }
        #endregion
        
    #region Constructor & Process Encapsulator
    static MgsPwMonitor()
    {
        //Logger?.Information($"MGSPW Monitor initialized...");
    }

    public static Process? MgsPwProcess
    {
        get
        {
            if (_mgsPwProcess != null && !_mgsPwProcess.HasExited)
                return _mgsPwProcess;

            try { MgsPwCancellationTokenSource.Cancel(); }
            catch { 
                //ignored
            }
            _mgsPwProcess = null;
            return null;
        }
        private set
        {
            if (value != null && value != _mgsPwProcess)
            {
                // Cancel any existing monitoring task before starting a new one
                try { MgsPwCancellationTokenSource.Cancel(); }
                catch
                {
                    // ignored
                }

                _mgsPwProcess = value;
            }
            else if (value == null)
            {
                // Just clear the process, don't start a new task
                _mgsPwProcess = null;
            }
        }
    }
    #endregion
    
    internal static void EnableMonitor(CancellationToken cancellationToken)
    {
        MonitorCancellationToken = cancellationToken;
        MonitorCancellationToken.Register(TearDownMonitor);
        //Logger?.Information("Starting MGSPW scanning thread...");
        Task.Run(ScanForMgsPw, cancellationToken);
    }
    #endregion
}