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

public partial class ItemDetailView : UserControl
{
    private Constants.Item? _item;
    private readonly MemoryManager _memoryManager;
    public event EventHandler<string>? ValueChanged;
    public static event EventHandler<string>? WarnUser;
    private bool _hasBeenWarned;
    
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
            ItemGrid.ColumnDefinitions = new ColumnDefinitions("1*,0");
            RankUpButton.IsVisible = (bool)value!;
            RankDownButton.IsVisible = (bool)value;
        }
    }

    public bool? CanStockUp
    {
        get;
        set
        {
            field = value;
            StockButton.IsVisible = (bool)value!;
            StockUpDown.IsVisible = (bool)value;
        }
    }

    public ItemDetailView()
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
            }
            _item ??= DetermineItem(PwObject!);
            var developed = _memoryManager.CheckItemDevelopment(_item.Index);
            if (developed)
            {
                DevelopCheckbox.IsChecked = true;
                StockButton.IsEnabled = true;
                StockUpDown.IsEnabled = true;
            }
            else
            {
                StockButton.IsEnabled = false;
                StockUpDown.IsEnabled = false;
                RankUpButton.IsEnabled = false;
                RankDownButton.IsEnabled = false;
                return;
            }
            var rank = _memoryManager.GetItemRank(_item);
            RankUpButton.IsEnabled = rank != _item.UpgradeIndices?.Length;
            RankDownButton.IsEnabled = rank != 0;
        }
        catch
        {
            //Squelch.
        }
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
            var enabling = (bool)DevelopCheckbox.IsChecked!;
            _item ??= DetermineItem(PwObject!);
            if ((_item.Name == "Stealth Camo" || _item.Name == "Bandana") && !_hasBeenWarned && enabling)
            {
                WarnUserOfActivity("WARNING: Researching this item this way does NOT count towards the achievement for doing so.\n\nYou will only be warned once.\n\nYou may attempt to research again to ignore this warning.");
                _hasBeenWarned = true;
                this.DevelopCheckbox.IsChecked = false;
                return;
            }
            _memoryManager.ResearchAndDevelopItem(_item!, enabling);
            SendStatusUpdate(enabling ? $"Developed {_item.Name}!" : $"Undeveloped {_item.Name}!");
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

    public void AddStock_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            _item ??= DetermineItem(PwObject!);
            if (StockUpDown.Value is not null)
            {
                var stockToAdd = (int)StockUpDown.Value;
                _memoryManager.ChangeItemStock(_item!, stockToAdd);
                SendStatusUpdate($"Added {stockToAdd} {_item.Name} to stock!");
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

    public void ChangeRank_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            _item ??= DetermineItem(PwObject!);
            var increasing = (sender as Control)!.Name == "RankUpButton";
            _memoryManager.ChangeItemRank(_item!, increasing);
            SendStatusUpdate(increasing ? $"Ranked up {_item.Name}!" : $"Ranked down {_item.Name}!");
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
    
    private static void WarnUserOfActivity(string message)
    {
        WarnUser?.Invoke(null, message);
    }
}