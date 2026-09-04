using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using Serilog;
using Serilog.Events;

namespace MGSPW_MC_Cheat_Trainer.Views;

public partial class MainWindow : Window
{
    private WindowNotificationManager _notificationManager;
    private static ILogger? Logger => LogManager.Logger;
    
    public MainWindow()
    {
        InitializeComponent();
        Title = $"{Title} - v{Program.AppVersion}";
        try
        {
            LogManager.StartLogger();
        }
        catch
        {
            IMsBox<ButtonResult> msgBox = MessageBoxManager.GetMessageBoxStandard(
                "Logging initialization failed!",
                $"We tried to start a debuglog, but something went wrong. Is {LogManager.LogLocation} a valid directory on your PC?");
            msgBox.ShowAsync();
        }
        _notificationManager = new WindowNotificationManager(GetTopLevel(this))
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 3
        };
        MgsPwMonitor.EnableMonitor(new CancellationToken());
        MgsPwMonitor.OnGameHooked += OnGameHooked;
        MgsPwMonitor.OnInvalidVersionDetected += OnInvalidVersionDetected;
        CheatsTabView.UpdateStatusBar += OnUpdateStatusBar;
        Task.Run(CheckForUpdates);
    }
    
    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }
    
    private void CheckForUpdates()
    {
        bool newerVersionAvailable = VersionSupport.CheckIfNewUpdateExists(Program.AppVersion);
        if (newerVersionAvailable)
        {
            Logger?.Debug("Newer version available, notifying user");
            Dispatcher.UIThread.Post(async void () =>
            {
                try
                {
                    IMsBox<ButtonResult> msgBox = MessageBoxManager.GetMessageBoxStandard(
                        "Update available",
                        "There is an updated version of this trainer available, would you like to view the Releases page?",
                        ButtonEnum.YesNo, windowStartupLocation: WindowStartupLocation);
                    if (await msgBox.ShowAsPopupAsync(GetMainWindow()) ==
                        ButtonResult.Yes) 
                    {
                        OpenUrl("https://github.com/sagefantasma/MGSPW-Cheat-Trainer/releases");
                    }
                }
                catch (Exception e)
                {
                    //Squelch exception
                }
            });
        }
    }
    
    private void OnUpdateStatusBar(object? sender, string msg)
    {
        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                StatusLabel.Text = msg;
                //await Task.Delay(2000);
                //StatusLabel.Text = "Ready";
            }
            catch(Exception e)
            {
                Logger?.Error($"Failed to update status bar: {e}");
            }
        });
    }
    
    private void OnGameHooked(object? sender, bool hooked)
    {
        //Do stuff here later, if wanted
    }
    
    private void OnInvalidVersionDetected(object? sender, string msg)
    {
        Logger?.Error($"Incompatible game version detected: {msg}");
        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                IMsBox<ButtonResult> msgBox = MessageBoxManager.GetMessageBoxStandard(
                    "Incompatible game version detected!",
                    msg, windowStartupLocation: WindowStartupLocation);
                msgBox.ShowAsPopupAsync(GetMainWindow());
            }
            catch (Exception e)
            {
                Logger?.Error($"Failed to inform user of invalid version: {e}");
            }
        });
    }

    private void ViewLogsMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        OpenUrl(LogManager.LogLocation!);
    }

    private void JoinDiscordMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        OpenUrl("https://discord.gg/XUh58VfqDu");
    }
    
    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            OnUpdateStatusBar(null, $"Unable to open link, you can use this link instead: {url}");
        }
    }

    private void MainTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        //throw new System.NotImplementedException();
    }

    private void WeaponTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        //throw new System.NotImplementedException();
    }

    private void OpenInstallLocationMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        OpenUrl(AppDomain.CurrentDomain.BaseDirectory);
    }

    private void VisitGithubRepoMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        OpenUrl("https://github.com/sagefantasma/MGSPW-Cheat-Trainer/");
    }
}