using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MGSPW_MC_Cheat_Trainer.Views;

public partial class OtherTabView : UserControl
{
    public static event EventHandler<string>? UpdateStatusBar;
    public static event EventHandler<bool>? OtherTabActivated;
    
    public OtherTabView()
    {
        InitializeComponent();
        MainWindow.TabActivated += OnTabActivated;
    }
    
    private void RequestStatusBarUpdate(object? obj, string message)
    {
        UpdateStatusBar?.Invoke(null, message);
    }
    
    private void OnTabActivated(object? sender, Tab e)
    {
        if (e == Tab.Other)
        {
            LogManager.Logger?.Information("Other tab activated...");
        }
    }
    
    private void ItemTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        LogManager.Logger?.Information($"Navigated to Other subtab: {(e.AddedItems[0] as TabItem)?.Name}");
    }
}