using System;
using System.Text;
using Avalonia;
using MGSPW_MC_Cheat_Trainer.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using SimplifiedMemoryManager;

namespace MGSPW_MC_Cheat_Trainer;

public class MemoryManager
{
    private static ILogger? Logger => LogManager.Logger;
    private static nint StageLocation = nint.MinValue;
    private static nint _weaponsArrayLocation = nint.MinValue;

    private static nint WeaponsArrayLocation
    {
        get
        {
            if(_weaponsArrayLocation == nint.MinValue)
            {
                try
                {
                    if (MgsPwMonitor.MgsPwProcess == null)
                        throw new NullReferenceException("Not hooked into PW, cannot set memory.");
                    lock (MgsPwMonitor.MgsPwProcess)
                    {
                        using SimpleProcessProxy spp = new(MgsPwMonitor.MgsPwProcess);
                        nint ptrLocation =
                            spp.FollowPointer(PeaceWalkerApplicationNavigator.PeaceWalkerAoB.WeaponsPtrLocation, true);
                        _weaponsArrayLocation = IntPtr.Add(ptrLocation,
                            PeaceWalkerApplicationNavigator.PeaceWalkerAoB.WeaponsPtrOffset);
                    }
                }
                catch (Exception e)
                {
                    Logger?.Error($"Failed to find weapons pointer: {e}");
                    throw new AggregateException("Failed to find weapons pointer.", e);
                }
            }
            return _weaponsArrayLocation;
        }
    }

    public MemoryManager()
    {
    }

    private bool SetMemoryAtOffset(nint offset, byte[] data)
    {
        try
        {
            if (MgsPwMonitor.MgsPwProcess == null)
                throw new NullReferenceException("Not hooked into PW, cannot set memory.");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                using SimpleProcessProxy spp = new(MgsPwMonitor.MgsPwProcess);
                spp.ModifyProcessOffset(offset, data);

                return true;
            }
        }
        catch (Exception e)
        {
            Logger?.Error($"Failed to set memory at offset {offset}: {e}");
            throw new AggregateException("Could not set memory", e);
        }
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
    
    private bool Research(Constants.Weapon weapon)
    {
        //TODO: validate
        //Set 0x04 in the array to 3
        try
        {
            return SetMemoryAtOffset(
                IntPtr.Add(WeaponsArrayLocation, 0x1C * weapon.Index + (int)Constants.WeaponMemory.Research), [0x03]);
        }
        catch (Exception e)
        {
            Logger?.Error($"Failed to research {weapon.Name}: {e}");
            throw new AggregateException($"Failed to research {weapon.Name}", e);
        }
    }
    
    private bool DevelopWeapon(Constants.Weapon weapon)
    {
        //TODO: validate
        //Set 0x08 in the array to 64
        try
        {
            return SetMemoryAtOffset(
                IntPtr.Add(WeaponsArrayLocation, 0x1C * weapon.Index + (int)Constants.WeaponMemory.Development), [0x64]);
        }
        catch (Exception e)
        {
            Logger?.Error($"Failed to develop {weapon.Name}: {e}");
            throw new AggregateException($"Failed to develop {weapon.Name}", e);
        }
    }
    
    private bool UpdateWeaponStock(Constants.Weapon weapon, uint stock)
    {
        //TODO: validate
        //Set 0x0C in the array to desired value
        try
        {
            return SetMemoryAtOffset(
                IntPtr.Add(WeaponsArrayLocation, 0x1C * weapon.Index + (int)Constants.WeaponMemory.Stock), BitConverter.GetBytes(stock));
        }
        catch (Exception e)
        {
            Logger?.Error($"Failed to update stock for {weapon.Name}: {e}");
            throw new AggregateException($"Failed to update stock for {weapon.Name}", e);
        }
    }
    
    private bool UpdateWeaponUsageLevel(Constants.Weapon weapon, byte usageLevel)
    {
        //TODO: validate
        //Set 0x16 in the array to desired value... isn't it actually 15?
        try
        {
            return SetMemoryAtOffset(
                IntPtr.Add(WeaponsArrayLocation, 0x1C * weapon.Index + (int)Constants.WeaponMemory.UsageLevel),
                [usageLevel]);
        }
        catch (Exception e)
        {
            Logger?.Error($"Failed to update stock for {weapon.Name}: {e}");
            throw new AggregateException($"Failed to update stock for {weapon.Name}", e);
        }
    }
    
    public bool ToggleObject(Constants.IPwObject obj)
    {
        //TODO: implement
        Logger?.Information($"Attempting to toggle object {obj.Name}");
        return false;
    }

    public bool LevelUpObject(Constants.IPwObject obj)
    {
        //TODO: implement
        Logger?.Information($"Attempting to level up object {obj.Name}");
        return false;
    }

    public bool MaxAmmo(Constants.IPwObject obj)
    {
        //TODO: implement
        Logger?.Information($"Attempting to max ammo for {obj.Name}");
        return false;
    }
}