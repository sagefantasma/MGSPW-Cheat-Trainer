using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using MGSPW_MC_Cheat_Trainer.Models;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using Serilog.Events;

namespace MGSPW_MC_Cheat_Trainer.Views;

public partial class OtherDetailView : UserControl
{
    private Constants.Item? _item;
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
            if(value != null)
                _item = DetermineOtherItem(value);
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

    public OtherDetailView()
    {
        InitializeComponent();
        _memoryManager = App.Services.GetRequiredService<MemoryManager>();
        MgsPwMonitor.OnGameHooked += OnHooked;
        MainWindow.OnMyOuterStage += OnHooked;
        OtherTabView.OtherTabActivated += OnHooked;
    }

    private void OnHooked(object? sender, bool e)
    {
        Dispatcher.UIThread.Post(void () =>
        {
            if (!string.IsNullOrWhiteSpace(MainWindow.CurrentStage))
            {
                if (string.Equals(MainWindow.CurrentStage, "title"))
                    return;
            }
            else
                return;
            try
            {
                _item ??= DetermineOtherItem(PwObject!);
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
                }
            }
            catch
            {
                //Squelch.
            }
        });
    }

    private static Constants.Item DetermineOtherItem(string input)
    {
        try
        {
            return Constants.BulletsList.Find(x => x.Name == input) ??
                   Constants.KeyItemsList.Find(x => x.Name == input) ??
                   Constants.UniformsList.Find(x => x.Name == input);
        }
        catch (Exception ex)
        {
            throw new AggregateException($"{input} is an unknown Other Item", ex);
        }
    }

    public void Enabled_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var enabling = (bool)DevelopCheckbox.IsChecked!;
            _item ??= DetermineOtherItem(PwObject!);
            if (_item.Name == "Bandana" || _item.Name == "Rare Ration")
            {
                
            }
            var success = _memoryManager.ResearchAndDevelopItem(_item!, enabling);
            if (success)
            {
                SendStatusUpdate(enabling
                    ? $"Developed Other Item: {_item.Name}!"
                    : $"Undeveloped Other Item: {_item.Name}!");
                OnHooked(null, true);
            }
            else
                SendStatusUpdate($"Failed to change development of Other Item: {_item.Name}");
        }
        catch (Exception ex)
        {
            string errorBrief = $"Failed to change development status of Other Item: {Name!}";
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
            _item ??= DetermineOtherItem(PwObject!);
            if (StockUpDown.Value is not null)
            {
                var stockToAdd = (int)StockUpDown.Value;
                var success = _memoryManager.ChangeItemStock(_item!, stockToAdd);
                if(success)
                    SendStatusUpdate($"Added {stockToAdd} {_item.Name} to stock!");
                else
                    SendStatusUpdate($"Failed to modify stock of Other Item: {_item.Name}");
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
    
    private void SendStatusUpdate(string message)
    {
        ValueChanged?.Invoke(null, message);
    }
}