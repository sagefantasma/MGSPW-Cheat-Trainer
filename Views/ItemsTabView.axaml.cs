using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MGSPW_MC_Cheat_Trainer.Views;

public partial class ItemsTabView : UserControl
{
    public static event EventHandler<string>? UpdateStatusBar;
    public static event EventHandler<bool>? ItemsTabActivated;
    
    public ItemsTabView()
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
            LogManager.Logger?.Information("Items tab activated...");
        }
    }
    
    private void ItemTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ItemsTabActivated?.Invoke(null, true);
        //throw new System.NotImplementedException();
    }
}