using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MGSPW_MC_Cheat_Trainer.Models;

namespace MGSPW_MC_Cheat_Trainer.ViewModels;

public partial class CheckboxCheatViewModel : UserControl
{
    public event EventHandler<string>? CheatToggled;
    
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
            CheatCheckBox.Content = value;
        }
    }

    public CheckboxCheatViewModel()
    {
        InitializeComponent();
    }
    
    private void ToggleCheat(string message)
    {
        CheatToggled?.Invoke(null, message);
    }
    
    private async void CheckboxCheat_OnIsCheckChanged(object? sender, RoutedEventArgs e)
    {
        try
        {
            GameCheat cheat = GameCheat.PeaceWalkerCheat.CheatList.Find(x => x.CheatType == Cheat);
            //Logging.Logger?.Information($"Attempting to toggle {CheatName}");
            ToggleCheat($"Attempting to toggle {CheatName}...");
            IsEnabled = false;

            bool toggleState = (bool)CheatCheckBox.IsChecked!;
            await Task.Run(() => cheat.CheatAction(toggleState));
            IsEnabled = true;
            //Logging.Logger?.Information($"{CheatName} 'successfully' toggled.");
            ToggleCheat($"Finished attempting to toggle {CheatName}. Results not guaranteed.");
        }
        catch (Exception ex)
        {
            string errorBrief = $"Failed to toggle {CheatName}";
            //Logging.Logger?.Error($"{errorBrief}: {ex.Message}");
            ToggleCheat(errorBrief);
        }
    }
}