using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using Serilog.Events;

namespace MGSPW_MC_Cheat_Trainer.Views;

public partial class MainWindow : Window
{
    private WindowNotificationManager _notificationManager;
    
    public MainWindow()
    {
        InitializeComponent();
        _notificationManager = new WindowNotificationManager(GetTopLevel(this))
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 3
        };
    }

    private void ViewLogsMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void ModifyConfigMenuItem_OnClickClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void JoinDiscordMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        string discordLink = "https://discord.gg/XUh58VfqDu";
        try
        {
            Launcher.LaunchUriAsync(new Uri(discordLink));
        }
        catch(Exception ex)
        {
            Clipboard?.SetTextAsync(discordLink);
            _notificationManager.Show(new Notification(
                title: "Link copied to clipboard!",
                message: "We tried to open the link directly for you, but could not. The link to our Discord is now in your clipboard :)",
                type: NotificationType.Success,
                expiration: new TimeSpan(0)));
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

    public void UpdateStatusLabel(string newStatus)
    {
        StatusLabel.Text = newStatus;
    }

    private void OpenInstallLocationMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        
    }

    private void VisitGithubRepoMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}