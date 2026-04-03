using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using MGSPW_MC_Cheat_Trainer.ViewModels;
using MGSPW_MC_Cheat_Trainer.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;

namespace MGSPW_MC_Cheat_Trainer;

public partial class App : Application
{
    public static IServiceProvider Services =>
        _services ?? throw new InvalidOperationException("Services not initialized yet.");

    private static IServiceProvider? _services;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        ServiceCollection collection = new();
        collection.AddSingleton<ILogManager>(new LogManager());
        collection.AddSingleton<MemoryManager>();
        
        _services = collection.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };

            desktop.Exit += (_, _) =>
            {
                LogManager logManager = Services.GetRequiredService<LogManager>();
                logManager?.Dispose();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}