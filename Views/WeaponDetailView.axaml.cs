using System;
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
            RankButton.IsVisible = (bool)value!;
        }
    }

    public WeaponDetailView()
    {
        InitializeComponent();
        _memoryManager = App.Services.GetRequiredService<MemoryManager>();
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
            _weapon ??= DetermineWeapon(PwObject!);
            _memoryManager.ResearchAndDevelopWeapon(_weapon!);
            SendStatusUpdate($"Developed {_weapon.Name}!");
            DevelopCheckbox.IsEnabled = false; //NOTE: disable the development checkbox once developed for now, later update to allow de-development
        }
        catch (Exception ex)
        {
            string errorBrief = $"Failed to research {Name!}";
            LogManager.Logger?.Error($"{errorBrief}: {ex.Message}");
            SendStatusUpdate(errorBrief);
            IMsBox<ButtonResult> msgBox = MessageBoxManager.GetMessageBoxStandard(
                errorBrief,
                ex.Message);
            msgBox.ShowAsync();
        }
    }

    public void UsageUp_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            _weapon ??= DetermineWeapon(PwObject!);
            _memoryManager.ChangeWeaponLevel(_weapon!);
            SendStatusUpdate($"Increased usage level for {_weapon.Name}!");
        }
        catch (Exception ex)
        {
            string errorBrief = $"Failed to increase usage level for {Name!}";
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
            _memoryManager.ChangeWeaponRank(_weapon!);
            SendStatusUpdate($"Ranked up {_weapon.Name}!");
        }
        catch (Exception ex)
        {
            string errorBrief = $"Failed to rank up {Name!}";
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
}