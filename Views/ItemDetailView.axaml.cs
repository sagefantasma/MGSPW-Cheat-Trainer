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

public partial class ItemDetailView : UserControl
{
    private Constants.Item? _item;
    private readonly MemoryManager _memoryManager;
    public event EventHandler<string>? ValueChanged;
    public static event EventHandler<string>? WarnUser;
    private bool HasBeenWarned;
    
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
            _item = Constants.ItemsList.Find(x => x.Name == value);
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

    public bool? CanStockUp
    {
        get;
        set
        {
            field = value;
            StockButton.IsVisible = (bool)value!;
        }
    }

    public ItemDetailView()
    {
        InitializeComponent();
        _memoryManager = App.Services.GetRequiredService<MemoryManager>();
    }

    private static Constants.Item DetermineItem(string input)
    {
        try
        {
            return Constants.ItemsList.Find(x => x.Name == input);
        }
        catch (Exception ex)
        {
            throw new AggregateException($"{input} is an unknown item", ex);
        }
    }

    public void Enabled_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            _item ??= DetermineItem(PwObject!);
            if ((_item.Name == "Stealth Camo" || _item.Name == "Bandana") && !HasBeenWarned)
            {
                WarnUserOfActivity("WARNING: Researching this item this way does NOT count towards the achievement for doing so.\n\nYou will only be warned once.\n\nYou may attempt to research again to ignore this warning.");
                HasBeenWarned = true;
                this.DevelopCheckbox.IsChecked = false;
                return;
            }
            _memoryManager.ResearchAndDevelopItem(_item!);
            SendStatusUpdate($"Developed {_item.Name}!");
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

    public void AddStock_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            _item ??= DetermineItem(PwObject!);
            _memoryManager.ChangeItemStock(_item!);
            SendStatusUpdate($"Added 100 {_item.Name} to stock!");
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

    public void RankUp_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            _item ??= DetermineItem(PwObject!);
            _memoryManager.ChangeItemRank(_item!);
            SendStatusUpdate($"Ranked up {_item.Name}!");
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
    
    private static void WarnUserOfActivity(string message)
    {
        WarnUser?.Invoke(null, message);
    }
}