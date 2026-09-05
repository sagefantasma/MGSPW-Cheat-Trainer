using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MGSPW_MC_Cheat_Trainer.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;

namespace MGSPW_MC_Cheat_Trainer.Views;

public partial class WeaponDetailView : UserControl
{
    private Constants.Weapon? _weapon;
    private readonly MemoryManager _memoryManager;
    
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

    public WeaponDetailView()
    {
        InitializeComponent();
        _memoryManager = App.Services.GetRequiredService<MemoryManager>();
    }

    private static Constants.Weapon DetermineWeapon(string input)
    {
        try
        {
            return Constants.WeaponsList.Find(x => input.ToLower().Contains($"{x.Shorthand}detailview", StringComparison.InvariantCultureIgnoreCase))!;
        }
        catch (Exception ex)
        {
            throw new NullReferenceException($"{input} is an unknown weapon");
        }
    }

    public void Enabled_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO: implement
        _weapon ??= DetermineWeapon(Name!);
        _memoryManager.ToggleObject(_weapon);
    }

    public void UsageUp_OnClick(object? sender, RoutedEventArgs e)
    {
        //TODO: implement
        _weapon ??= DetermineWeapon(Name!);
        _memoryManager.LevelUpObject(_weapon);
    }

    public void RankUp_OnClick(object? sender, RoutedEventArgs e)
    {
        //TODO: implement
        _weapon ??= DetermineWeapon(Name!);
        _memoryManager.MaxAmmo(_weapon);
    }
}