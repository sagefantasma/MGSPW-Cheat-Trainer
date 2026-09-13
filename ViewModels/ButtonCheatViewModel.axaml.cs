using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MGSPW_MC_Cheat_Trainer.Models;
using Serilog;

namespace MGSPW_MC_Cheat_Trainer.ViewModels;

public partial class ButtonCheatViewModel : UserControl
{
    public event EventHandler<string>? CheatToggled;
    private static ILogger? Logger => LogManager.Logger;
    
    public required Constants.Cheat Cheat
    {
        get;
        set;
    }

    public required string CheatName
    {
        get;
        set
        {
            field = value;
            ButtonText.Text = value;
        }
    }

    public ButtonCheatViewModel()
    {
        InitializeComponent();
    }
    
    private void ActivateCheat(string message)
    {
        CheatToggled?.Invoke(null, message);
    }
    
    private async void CheckboxCheat_OnIsCheckChanged(object? sender, RoutedEventArgs e)
    {
        try
        {
            GameCheat cheat = GameCheat.PeaceWalkerCheat.CheatList.Find(x => x.CheatType == Cheat);
            Logger?.Information($"Attempting to activate {CheatName}");
            ActivateCheat($"Attempting to activate {CheatName}...");
            IsEnabled = false;

            await Task.Run(() => cheat?.CheatAction(true));
            IsEnabled = true;
            Logger?.Information($"{CheatName} 'successfully' activated.");
            ActivateCheat($"Finished attempting to activate {CheatName}. Results not guaranteed.");
        }
        catch (Exception ex)
        {
            string errorBrief = $"Failed to toggle {CheatName}";
            Logger?.Error($"{errorBrief}: {ex.Message}");
            ActivateCheat(errorBrief);
        }
    }
}