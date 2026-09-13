using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MGSPW_MC_Cheat_Trainer.Views;

public partial class WeaponsTabView : UserControl
{
    public static event EventHandler<string>? UpdateStatusBar;
    public static event EventHandler<bool>? WeaponsTabActivated;
    
    public WeaponsTabView()
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
        if (e == Tab.Weapons)
        {
            LogManager.Logger?.Information("Weapons tab activated...");
        }
    }
    
    private void WeaponTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        LogManager.Logger?.Information($"Navigated to Weapons subtab: {(e.AddedItems[0] as TabItem)?.Name}");
    }
}