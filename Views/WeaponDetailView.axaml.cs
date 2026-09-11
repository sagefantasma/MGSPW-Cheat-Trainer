using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MGSPW_MC_Cheat_Trainer.Models;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using Serilog.Events;

namespace MGSPW_MC_Cheat_Trainer.Views;

public partial class WeaponDetailView : UserControl
{
    private Constants.Weapon? _weapon;
    private readonly MemoryManager _memoryManager;
    public event EventHandler<string>? ValueChanged;
    
    public IImage? EntityImage
    {
        get => ObjectImage?.Source;
        set => ObjectImage?.Source = value;
    }
    
    public string? PwObject
    {
        get;
        set
        {
            field = value;
            GroupBox.Header = new TextBlock
            {
                Text = $"{PwObject}",
                Foreground = Brushes.Black,
                FontWeight = FontWeight.Bold
            };
            _weapon = Constants.WeaponsList.Find(x => x.Name == value);
        }
    }

    public bool? CanRankUp
    {
        get;
        set
        {
            field = value;
            ItemGrid.ColumnDefinitions = new ColumnDefinitions("1*, 0");
            RankUpButton.IsVisible = (bool)value!;
            RankDownButton.IsVisible = (bool)value;
        }
    }

    public bool? HasStock
    {
        get;
        set
        {
            field = value;
            StockButton.IsVisible = (bool)value!;
            StockUpDown.IsVisible = (bool)value;
        }
    } = false;

    public WeaponDetailView()
    {
        InitializeComponent();
        _memoryManager = App.Services.GetRequiredService<MemoryManager>();
        Loaded += OnLoad;
    }

    private void OnLoad(object? sender, RoutedEventArgs e)
    {
        try
        {
            while (MgsPwMonitor.MgsPwProcess == null)
            {
                Task.Delay(100).Wait(); 
                //TODO: technically doing it this way makes the app not load at all if the game isn't running,
                //does it make sense to do it another way instead? Piggy-backing off of MgsPwMonitor.OnGameHooked
                //doesn't do the trick for some reason, even if I want for the IsLoaded bool to get set to true.
            }
            _weapon ??= DetermineWeapon(PwObject!);
            var developed = _memoryManager.CheckWeaponDevelopment(_weapon.Index);
            if (developed)
            {
                DevelopCheckbox.IsChecked = true;
                StockButton.IsEnabled = true;
                StockUpDown.IsEnabled = true;
            }
            else
            {
                UsageUpButton.IsEnabled = false;
                UsageDownButton.IsEnabled = false;
                RankUpButton.IsEnabled = false;
                RankDownButton.IsEnabled = false;
                StockButton.IsEnabled = false;
                StockUpDown.IsEnabled = false;
                return;
            }
            var rank = _memoryManager.GetWeaponRank(_weapon);
            RankUpButton.IsEnabled = rank != _weapon.UpgradeIndices?.Length;
            RankDownButton.IsEnabled = rank != 0;
            var usage = _memoryManager.GetWeaponUsageLevel(_weapon);
            UsageDownButton.IsEnabled = usage > 1;
            UsageUpButton.IsEnabled = usage != 3;
        }
        catch
        {
            //Squelch.
        }
    }

    private static Constants.Weapon DetermineWeapon(string input)
    {
        try
        {
            return Constants.WeaponsList.Find(x => x.Name == input);
            //return Constants.WeaponsList.Find(x => input.ToLower().Contains($"{x.Shorthand}detailview", StringComparison.InvariantCultureIgnoreCase))!;
        }
        catch (Exception ex)
        {
            throw new AggregateException($"{input} is an unknown weapon", ex);
        }
    }

    public void Enabled_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var enabling = (bool)DevelopCheckbox.IsChecked!;
            _weapon ??= DetermineWeapon(PwObject!);
            _memoryManager.ResearchAndDevelopWeapon(_weapon!, enabling);
            SendStatusUpdate(enabling ? $"Developed {_weapon.Name}!" : $"Undeveloped {_weapon.Name}!");
            OnLoad(null, null);
        }
        catch (Exception ex)
        {
            string errorBrief = $"Failed to change development status of {Name!}";
            LogManager.Logger?.Error($"{errorBrief}: {ex.Message}");
            SendStatusUpdate(errorBrief);
            IMsBox<ButtonResult> msgBox = MessageBoxManager.GetMessageBoxStandard(
                errorBrief,
                ex.Message);
            msgBox.ShowAsync();
        }
    }

    public void ChangeUsage_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            _weapon ??= DetermineWeapon(PwObject!);
            var increasing = (sender as Control)!.Name == "UsageUpButton";
            _memoryManager.ChangeWeaponUseLevel(_weapon!, increasing);
            SendStatusUpdate(increasing ? $"Increased usage level for {_weapon.Name}!" : $"Decreased usage level for {_weapon.Name}!");
            var usageLevel = _memoryManager.GetWeaponUsageLevel(_weapon);
            UsageDownButton.IsEnabled = usageLevel > 1;
            UsageUpButton.IsEnabled = usageLevel < 3;
        }
        catch (Exception ex)
        {
            string errorBrief = $"Failed to modify usage level for {Name!}";
            LogManager.Logger?.Error($"{errorBrief}: {ex.Message}");
            SendStatusUpdate(errorBrief);
            IMsBox<ButtonResult> msgBox = MessageBoxManager.GetMessageBoxStandard(
                errorBrief,
                ex.Message);
            msgBox.ShowAsync();
        }
    }

    public void RankUp_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            _weapon ??= DetermineWeapon(PwObject!);
            var increasing = (sender as Control)!.Name == "RankUpButton";
            _memoryManager.ChangeWeaponRank(_weapon!, increasing);
            SendStatusUpdate(increasing ? $"Ranked up {_weapon.Name}!" : $"Ranked down {_weapon.Name}!");
            var currentRank = _memoryManager.GetWeaponRank(_weapon);
            RankDownButton.IsEnabled = currentRank > 0;
            RankUpButton.IsEnabled = currentRank < _weapon.UpgradeIndices?.Length;
        }
        catch (Exception ex)
        {
            string errorBrief = $"Failed to modify rank of {Name!}";
            LogManager.Logger?.Error($"{errorBrief}: {ex.Message}");
            SendStatusUpdate(errorBrief);
            IMsBox<ButtonResult> msgBox = MessageBoxManager.GetMessageBoxStandard(
                errorBrief,
                ex.Message);
            msgBox.ShowAsync();
        }
    }
    
    private void SendStatusUpdate(string message)
    {
        ValueChanged?.Invoke(null, message);
    }

    private void AddStock_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            _weapon ??= DetermineWeapon(PwObject!);
            if (StockUpDown.Value is not null)
            {
                var stockToAdd = (int)StockUpDown.Value;
                _memoryManager.ChangeWeaponStock(_weapon!, stockToAdd);
                SendStatusUpdate($"Added {stockToAdd} {_weapon.Name} to stock!");
            }
            else
            {
                throw new InvalidDataException("You must provide a value greater than zero in the stock box.");
            }
        }
        catch (Exception ex)
        {
            string errorBrief = $"Failed to add stock for {Name!}";
            LogManager.Logger?.Error($"{errorBrief}: {ex.Message}");
            SendStatusUpdate(errorBrief);
            IMsBox<ButtonResult> msgBox = MessageBoxManager.GetMessageBoxStandard(
                errorBrief,
                ex.Message);
            msgBox.ShowAsync();
        }
    }
}