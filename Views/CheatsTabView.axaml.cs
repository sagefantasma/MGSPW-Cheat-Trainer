using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MGSPW_MC_Cheat_Trainer.ViewModels;

namespace MGSPW_MC_Cheat_Trainer.Views;

public partial class CheatsTabView : UserControl
{
    public static EventHandler<string>? UpdateStatusBar;
    
    public CheatsTabView()
    {
        InitializeComponent();
        MainWindow.TabActivated += OnTabActivated;
        foreach (var control in PlayerCheats.Children)
            if (control is CheckboxCheatViewModel cheatViewModel)
                cheatViewModel.CheatToggled += RequestStatusBarUpdate;
        foreach (var control in EquipmentCheats.Children)
            if (control is CheckboxCheatViewModel cheatViewModel)
                cheatViewModel.CheatToggled += RequestStatusBarUpdate;
        foreach (var control in EnemyCheats.Children)
            if (control is CheckboxCheatViewModel cheatViewModel)
                cheatViewModel.CheatToggled += RequestStatusBarUpdate;
        foreach (var control in MissionCheats.Children)
            if (control is CheckboxCheatViewModel cheatViewModel)
                cheatViewModel.CheatToggled += RequestStatusBarUpdate;
    }
    
    private void OnTabActivated(object? sender, Tab e)
    {
        if (e == Tab.Cheats)
        {
            LogManager.Logger?.Information("Cheats tab activated...");
        }
    }
    
    private static void RequestStatusBarUpdate(object? obj, string message)
    {
        UpdateStatusBar?.Invoke(null, message);
    }
}