using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;

namespace MGSPW_MC_Cheat_Trainer;

public class MemoryManager
{
    private readonly ILogManager _logger;

    public MemoryManager()
    {
        _logger = App.Services.GetRequiredService<ILogManager>();
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