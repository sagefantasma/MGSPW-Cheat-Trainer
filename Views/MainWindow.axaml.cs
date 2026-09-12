using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using MGSPW_MC_Cheat_Trainer.Models;
using MGSPW_MC_Cheat_Trainer.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using Serilog;
using Serilog.Events;
using SimplifiedMemoryManager;
using Tmds.DBus.Protocol;

namespace MGSPW_MC_Cheat_Trainer.Views;

public enum Tab
{
    Cheats,
    Weapons,
    Items,
    Other,
    Staff
}

public partial class MainWindow : Window
{
    public static event EventHandler<Tab>? TabActivated;
    public static event EventHandler<bool>? OnMyOuterStage;
    private static ILogger? Logger => LogManager.Logger;
    private readonly MemoryManager _memoryManager;

    public static string? CurrentStage
    {
        get;
        private set
        {
            if (field == value) return;
            field = value;
            if (value == null) return;
            if (value.Contains("my_outer"))
            {
                Logger?.Information("Stage my_outer, invoking OnMyOuterStage");
                OnMyOuterStage?.Invoke(null, true);
            }
            else
            {
                Logger?.Verbose($"Current stage: {value}");
            }
        }
    }

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
        _memoryManager = App.Services.GetRequiredService<MemoryManager>();
        StatusLabel.Text = "Searching for active Peace Walker instance...";
        MgsPwMonitor.EnableMonitor(new CancellationToken());
        MgsPwMonitor.OnGameHooked += OnGameHooked;
        MgsPwMonitor.OnInvalidVersionDetected += OnInvalidVersionDetected;
        WeaponsTabView.UpdateStatusBar += OnUpdateStatusBar;
        ItemDetailView.WarnUser += OnWarnUser;
        ItemsTabView.UpdateStatusBar += OnUpdateStatusBar;
        OtherTabView.UpdateStatusBar += OnUpdateStatusBar;
        CheatsTabView.UpdateStatusBar += OnUpdateStatusBar;
        this.Closing += OnClosing;
        Task.Run(CheckForUpdates);
        PeriodicTask.Run(ScanForMultiplayer, TimeSpan.FromSeconds(5));
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        //Turn off any and all active cheats
        GameCheat.DeactivateActiveCheats();
    }

    private void DeactivateAllCheats()
    {
        foreach (var control in VisibleCheatsTabView.PlayerCheats.Children)
        {
            (control as CheckboxCheatViewModel)?.CheatCheckBox.IsChecked = false;
            control.IsEnabled = false;
        }
        foreach (var control in VisibleCheatsTabView.EquipmentCheats.Children)
        {
            (control as CheckboxCheatViewModel)?.CheatCheckBox.IsChecked = false;
            control.IsEnabled = false;
        }
        foreach (var control in VisibleCheatsTabView.EnemyCheats.Children)
        {
            (control as CheckboxCheatViewModel)?.CheatCheckBox.IsChecked = false;
            control.IsEnabled = false;
        }
        foreach (var control in VisibleCheatsTabView.MissionCheats.Children)
        {
            (control as CheckboxCheatViewModel)?.CheatCheckBox.IsChecked = false;
            control.IsEnabled = false;
        }
    }

    private void EnableCheatUse()
    {
        foreach (var control in VisibleCheatsTabView.PlayerCheats.Children)
        {
            control.IsEnabled = true;
        }
        foreach (var control in VisibleCheatsTabView.EquipmentCheats.Children)
        {
            control.IsEnabled = true;
        }
        foreach (var control in VisibleCheatsTabView.EnemyCheats.Children)
        {
            control.IsEnabled = true;
        }
        foreach (var control in VisibleCheatsTabView.MissionCheats.Children)
        {
            control.IsEnabled = true;
        }
    }

    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }

    private void ScanForMultiplayer()
    {
        try
        {
            CurrentStage = _memoryManager.GetCurrentStage();
            if (CurrentStage.Contains("vs_lobby"))
            {
                //Turn off all cheats and disable their use
                Logger?.Information("Stage is vs_lobby, disabling cheats.");
                DeactivateAllCheats();
                return;
            }

            if (MgsPwMonitor.MgsPwProcess == null)
                return;
            lock (MgsPwMonitor.MgsPwProcess)
            {
                using SimpleProcessProxy spp =
                    new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                var startingLocation =
                    spp.FollowPointer(PeaceWalkerApplicationNavigator.PeaceWalkerAoB.MaxCoopPlayerCountLocation,
                        true);
                foreach (var offset in PeaceWalkerApplicationNavigator.PeaceWalkerAoB.MaxCoopPlayerCountOffsets)
                {
                    if (offset == PeaceWalkerApplicationNavigator.PeaceWalkerAoB.MaxCoopPlayerCountOffsets.Last())
                        break;
                    startingLocation =
                        new nint(BitConverter.ToInt64(
                            spp.GetMemoryFromPointer(IntPtr.Add(startingLocation, offset), 8)));
                }

                if (spp.GetMemoryFromPointer(
                        IntPtr.Add(startingLocation,
                            PeaceWalkerApplicationNavigator.PeaceWalkerAoB.MaxCoopPlayerCountOffsets.Last()),
                        1)[0] != 0x01)
                {
                    //Turn off all cheats and disable their use
                    Logger?.Information("Co-op max player count > 1; disabling cheats.");
                    DeactivateAllCheats();
                    return;
                }
            }
            
            //If not in versus or co-op, enable cheats
            EnableCheatUse();
        }
        catch
        {
            //Fail silently.
        }
    }

    private async Task ManualDownloadPrompt()
    {
        IMsBox<ButtonResult> msgBox2 = MessageBoxManager.GetMessageBoxStandard(
            "Go to GitHub?",
            "Would you like to view GitHub instead to download it yourself manually?",
            ButtonEnum.YesNo, windowStartupLocation: WindowStartupLocation);
        if (await msgBox2.ShowAsPopupAsync(GetMainWindow()) == ButtonResult.Yes)
        {
            OnUpdateStatusBar(null, "Opening releases page in your browser...");
            OpenUrl("https://github.com/sagefantasma/MGSPW-Cheat-Trainer/releases");
        }
    }
    
    private void CheckForUpdates()
    {
        bool newerVersionAvailable = VersionSupport.CheckIfNewUpdateExists(Program.AppVersion);
        if (newerVersionAvailable)
        {
            Logger?.Debug("Newer version available, notifying user");
            OnUpdateStatusBar(null, "Newer version of this trainer is available on GitHub, please consider downloading it for the best experience");
            Dispatcher.UIThread.Post(async void () =>
            {
                try
                {
                    IMsBox<ButtonResult> msgBox = MessageBoxManager.GetMessageBoxStandard(
                        "Update available",
                        "There is an updated version of this trainer available, would you like to automatically download it?",
                        ButtonEnum.YesNo, windowStartupLocation: WindowStartupLocation);
                    if (await msgBox.ShowAsPopupAsync(GetMainWindow()) == ButtonResult.No)
                    {
                        await ManualDownloadPrompt();
                    }
                    else
                    {
                        if (Directory.GetParent(Environment.CurrentDirectory)!.GetFiles("AutoUpdater*").Length == 0)
                        {
                            IMsBox<ButtonResult> msgBox2 = MessageBoxManager.GetMessageBoxStandard(
                                "AutoUpdater not found",
                                "It looks like AutoUpdater isn't currently installed, do you to install it automatically?",
                                ButtonEnum.YesNo, windowStartupLocation: WindowStartupLocation);
                            var result = await msgBox2.ShowAsPopupAsync(GetMainWindow());
                            if (result == ButtonResult.Yes)
                            {
                                DownloadAutoUpdaterMenuItem_Click(null, null);
                            }
                            else
                            {
                                OnUpdateStatusBar(null, "Cannot auto update without AutoUpdater, cancelling auto update process.");
                                await ManualDownloadPrompt();
                            }
                        }
                        VersionSupport.StartAutoUpdater();
                        Close();
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
        OnUpdateStatusBar(sender, "Peace Walker found and hooked! Ready to go.");
    }

    private void OnWarnUser(object? sender, string warning)
    {
        Logger?.Information("Warning user about enabling specific item");
        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                IMsBox<ButtonResult> msgBox = MessageBoxManager.GetMessageBoxStandard(
                    "Achievement Warning!",
                    warning, ButtonEnum.Ok, windowStartupLocation: WindowStartupLocation);
                msgBox.ShowAsPopupAsync(GetMainWindow());
            }
            catch (Exception e)
            {
                Logger?.Error($"Failed to inform user of warning: {e}");
            }
        });
    }
    
    private void OnInvalidVersionDetected(object? sender, string msg)
    {
        Logger?.Error($"Incompatible game version detected: {msg}");
        OnUpdateStatusBar(null, "Incompatible game version detected - expect issues if you try to use this trainer.");
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
        OnUpdateStatusBar(null, "Opening logs folder in your file manager...");
        OpenUrl(LogManager.LogLocation!);
    }

    private void JoinDiscordMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        OnUpdateStatusBar(null, "Opening our Discord server in your browser...");
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
        if (MainTabControl?.SelectedItem?.Equals(CheatsTab) == true)
        {
            TabActivated?.Invoke(this, Tab.Cheats);
        }
        else if (MainTabControl?.SelectedItem?.Equals(WeaponsTab) == true)
        {
            TabActivated?.Invoke(this, Tab.Weapons);
        }
        else if (MainTabControl?.SelectedItem?.Equals(ItemsTab) == true)
        {
            TabActivated?.Invoke(this, Tab.Items);
        }
        else if (MainTabControl?.SelectedItem?.Equals(OtherTab) == true)
        {
            TabActivated?.Invoke(this, Tab.Other);
        }
        else if (MainTabControl?.SelectedItem?.Equals(StaffTab) == true)
        {
            TabActivated?.Invoke(this, Tab.Staff);
        }
    }

    private void OpenInstallLocationMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        OnUpdateStatusBar(null, "Opening install location in your file manager...");
        OpenUrl(AppDomain.CurrentDomain.BaseDirectory);
    }

    private async void DownloadAutoUpdaterMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            OnUpdateStatusBar(null, "Downloading Auto Updater...");
            await Task.Run(VersionSupport.DownloadAutoUpdater);
            OnUpdateStatusBar(null, "Auto Updater download complete!");
        }
        catch
        {
            //Squelch
        }
    }

    private void VisitGithubRepoMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        OnUpdateStatusBar(null, "Opening GitHub repo in your browser...");
        OpenUrl("https://github.com/sagefantasma/MGSPW-Cheat-Trainer/");
    }
}