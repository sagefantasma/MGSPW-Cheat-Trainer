using System.Text;
using MGSPW_MC_Cheat_Trainer.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using SimplifiedMemoryManager;

namespace MGSPW_MC_Cheat_Trainer;

public class MemoryManager
{
    private readonly ILogManager _logger;
    private static nint StageLocation = nint.MinValue;

    public MemoryManager()
    {
        _logger = App.Services.GetRequiredService<ILogManager>();
    }

    public string GetCurrentStage()
    {
        //TODO: validate
        if (MgsPwMonitor.MgsPwProcess == null)
            return null;
        lock (MgsPwMonitor.MgsPwProcess)
        {
            using (SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess))
            {
                if (StageLocation == nint.MinValue)
                {
                    SimpleProcessProxy.SimpleMemory result = spp.ScanMemoryForUniquePatternAsync(
                            new SimplePattern(PeaceWalkerApplicationNavigator.PeaceWalkerAoB.StartOfSaveDataBlockAoB))
                        .Result;
                    StageLocation = result.Offset;
                }

                return Encoding.UTF8.GetString(spp.ReadProcessOffset(StageLocation, 18));
            }
        }
    }
    
    public bool ToggleObject(Constants.IPwObject obj)
    {
        //TODO: implement
        _logger.LogInformation($"Attempting to toggle object {obj.Name}");
        return false;
    }

    public bool LevelUpObject(Constants.IPwObject obj)
    {
        //TODO: implement
        _logger.LogInformation($"Attempting to level up object {obj.Name}");
        return false;
    }

    public bool MaxAmmo(Constants.IPwObject obj)
    {
        //TODO: implement
        _logger.LogInformation($"Attempting to max ammo for {obj.Name}");
        return false;
    }
}