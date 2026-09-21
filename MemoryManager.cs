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
    private static nint _stageLocation = nint.MinValue;
    private static nint _weaponsArrayLocation = nint.MinValue;
    //private static nint _itemsArrayLocation = nint.MinValue;

    private static nint ItemsArrayLocation =>
        WeaponsArrayLocation + 0x2580;

    private static nint WeaponsArrayLocation
    {
        get
        {
            if(_weaponsArrayLocation == nint.MinValue || _weaponsArrayLocation == PeaceWalkerApplicationNavigator.PeaceWalkerAoB.WeaponsPtrOffset)
            {
                try
                {
                    if (MgsPwMonitor.MgsPwProcess == null)
                    {
                        _weaponsArrayLocation = nint.MinValue;
                        throw new TrainerException();
                    }

                    lock (MgsPwMonitor.MgsPwProcess)
                    {
                        using SimpleProcessProxy spp = new(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                        nint ptrLocation =
                            spp.FollowPointer(PeaceWalkerApplicationNavigator.PeaceWalkerAoB.WeaponsPtrLocation, true);
                        _weaponsArrayLocation = IntPtr.Add(ptrLocation,
                            PeaceWalkerApplicationNavigator.PeaceWalkerAoB.WeaponsPtrOffset);
                    }
                }
                catch (TrainerException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    string baseMessage = "Failed to find weapons pointer";
                    Logger?.Error($"{baseMessage}: {e}");
                    throw new AggregateException(baseMessage, e);
                }
            }
            return _weaponsArrayLocation;
        }
    }

    public MemoryManager()
    {
    }

    private byte[] GetMemoryAtOffset(nint offset, int dataToRead)
    {
        try
        {
            if (MgsPwMonitor.MgsPwProcess == null)
                throw new NullReferenceException("Not hooked into PW, cannot get memory.");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                using SimpleProcessProxy spp = new(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                return spp.GetMemoryFromPointer(offset, dataToRead);
            }
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to get memory at offset {offset}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }

    private bool SetMemoryAtPointer(nint offset, byte[] data)
    {
        try
        {
            if (MgsPwMonitor.MgsPwProcess == null)
                throw new NullReferenceException("Not hooked into PW, cannot set memory.");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                using SimpleProcessProxy spp = new(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                spp.SetMemoryAtPointer(offset, data);

                return true;
            }
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to set memory at offset {offset}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }

    public string? GetCurrentStage()
    {
        if (MgsPwMonitor.MgsPwProcess == null)
            return null;
        lock (MgsPwMonitor.MgsPwProcess)
        {
            using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
            if (_stageLocation != nint.MinValue)
                return Encoding.UTF8.GetString(spp.GetMemoryFromPointer(_stageLocation, 18));
            
            nint ptrLocation =
                spp.FollowPointer(PeaceWalkerApplicationNavigator.PeaceWalkerAoB.StagePtrLocation, true);

            _stageLocation = IntPtr.Add(ptrLocation,
                PeaceWalkerApplicationNavigator.PeaceWalkerAoB.StagePtrOffset);

            return Encoding.UTF8.GetString(spp.GetMemoryFromPointer(_stageLocation, 18));
        }
    }
    
    private bool ResearchWeapon(Constants.Weapon weapon, bool research = true)
    {
        return ResearchWeapon(weapon.Index, research);
    }

    private bool ResearchWeapon(int index, bool research = true)
    {
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (index - 1) + (int)Constants.WeaponMemory.Research),
                research ? [0x03] : [0x01]);
        }
        catch (TrainerException)
        {
            return false;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to research weapon index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool ResearchItem(Constants.Item item, bool research = true)
    {
        return ResearchItem(item.Index, research);
    }

    private bool ResearchItem(int index, bool research = true)
    {
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(ItemsArrayLocation, Constants.ItemSize * (index - 1) + (int)Constants.ItemMemory.Research),
                research ? [0x03] : [0x01]);
        }
        catch (TrainerException)
        {
            return false;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to research weapon index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }

    private bool CheckWeaponResearch(int index)
    {
        try
        {
            return GetMemoryAtOffset(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (index - 1) + (int)Constants.WeaponMemory.Research), 1)[0] == 0x03;
        }
        catch (TrainerException)
        {
            return false;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to check research status for weapon index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool CheckItemResearch(int index)
    {
        try
        {
            return GetMemoryAtOffset(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (index - 1) + (int)Constants.ItemMemory.Research), 1)[0] == 0x03;
        }
        catch (TrainerException)
        {
            return false;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to check research status for item index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    public byte CheckItemResearchValue(int index)
    {
        try
        {
            return GetMemoryAtOffset(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (index - 1) + (int)Constants.ItemMemory.Research), 1)[0];
        }
        catch (TrainerException)
        {
            return 0xFF;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to check research status for item index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }

    public void SetItemDevelopmentValue(int index, byte value)
    {
        try
        {
            SetMemoryAtPointer(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (index - 1) + (int)Constants.ItemMemory.Development), [value]);
        }
        catch (TrainerException)
        {
            //Squelch
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to set development status for item index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    public byte CheckWeaponResearchValue(int index)
    {
        try
        {
            return GetMemoryAtOffset(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (index - 1) + (int)Constants.WeaponMemory.Research), 1)[0];
        }
        catch (TrainerException)
        {
            return 0xFF;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to check research status for weapon index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }

    public void SetWeaponDevelopmentValue(int index, byte value)
    {
        try
        {
            SetMemoryAtPointer(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (index - 1) + (int)Constants.WeaponMemory.Development), [value]);
        }
        catch (TrainerException)
        {
            //Squelch
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to set development status for weapon index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool DevelopWeapon(Constants.Weapon weapon, bool develop = true)
    {
        return DevelopWeapon(weapon.Index, develop);
    }

    private bool DevelopWeapon(int index, bool develop = true)
    {
        //Set 0x08 in the array to 64
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (index - 1) + (int)Constants.WeaponMemory.Development),
                develop ? [0x64] : [0x00]);
        }
        catch (TrainerException)
        {
            return false;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to develop weapon index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    public bool CheckWeaponDevelopment(int index)
    {
        try
        {
            return GetMemoryAtOffset(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (index - 1) + (int)Constants.WeaponMemory.Development), 1)[0] == 0x64;
        }
        catch (TrainerException)
        {
            return false;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to check development status for weapon index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool DevelopItem(Constants.Item item, bool develop = true)
    {
        return DevelopItem(item.Index, develop);
    }

    private bool DevelopItem(int index, bool develop = true)
    {
        //Set 0x08 in the array to 64
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (index - 1) + (int)Constants.ItemMemory.Development),
                develop ? [0x64] : [0x00]);
        }
        catch (TrainerException)
        {
            return false;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to develop item index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    public bool CheckItemDevelopment(int index)
    {
        try
        {
            return GetMemoryAtOffset(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (index - 1) + (int)Constants.ItemMemory.Development), 1)[0] == 0x64;
        }
        catch (TrainerException)
        {
            return false;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to check development status for item index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool UpdateWeaponStock(Constants.Weapon weapon, int stock)
    {
        return UpdateWeaponStock(weapon.Index, stock);
    }
    
    private bool UpdateWeaponStock(int weapon, int stock)
    {
        //Set 0x0C in the array to desired value
        try
        {
            if (stock > 9999)
            {
                Logger?.Debug("Requested stock value is greater than 9999, forcing 9999");
                stock = 9999;
            }
            
            return SetMemoryAtPointer(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (weapon - 1) + (int)Constants.WeaponMemory.Stock),
                BitConverter.GetBytes(stock));
        }
        catch (TrainerException)
        {
            return false;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to update stock for weapon index {weapon}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool UpdateItemStock(Constants.Item item, int stock)
    {
        return UpdateItemStock(item.Index, stock);
    }
    
    private bool UpdateItemStock(int item, int stock)
    {
        //Set 0x0C in the array to desired value
        try
        {
            if (stock > 9999)
            {
                Logger?.Debug("Requested stock value is greater than 9999, forcing 9999");
                stock = 9999;
            }

            return SetMemoryAtPointer(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (item - 1) + (int)Constants.ItemMemory.Stock),
                BitConverter.GetBytes(stock));
        }
        catch (TrainerException)
        {
            return false;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to update stock for item index {item}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private int GetItemStock(Constants.Item item)
    {
        return GetItemStock(item.Index);
    }
    
    private int GetWeaponStock(int weapon)
    {
        try
        {
            return BitConverter.ToInt32(GetMemoryAtOffset(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (weapon - 1) + (int)Constants.WeaponMemory.Stock), 4));
        }
        catch (TrainerException)
        {
            return -1;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to get stock for weapon index {weapon}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private int GetItemStock(int item)
    {
        try
        {
            return BitConverter.ToInt32(GetMemoryAtOffset(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (item - 1) + (int)Constants.ItemMemory.Stock), 4));
        }
        catch (TrainerException)
        {
            return -1;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to get stock for item index {item}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool UpdateWeaponUsageLevel(Constants.Weapon weapon, byte usageLevel)
    {
        return UpdateWeaponUsageLevel(weapon.Index, usageLevel);
    }
    
    private bool UpdateWeaponUsageLevel(int weapon, byte usageLevel)
    {
        //Set 0x16 in the array to desired value
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (weapon - 1) + (int)Constants.WeaponMemory.UsageLevel),
                [usageLevel]);
        }
        catch (TrainerException)
        {
            return false;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to update usage level for weapon index {weapon}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }

    public byte GetWeaponUsageLevel(Constants.Weapon weapon)
    {
        return GetWeaponUsageLevel(weapon.Index);
    }
    
    public byte GetWeaponUsageLevel(int weapon)
    {
        try
        {
            return GetMemoryAtOffset(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (weapon - 1) + (int)Constants.WeaponMemory.UsageLevel), 1)[0];
        }
        catch (TrainerException)
        {
            return 0xFF;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to get usage level for weapon index {weapon}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }

    public int GetWeaponRank(Constants.Weapon weapon)
    {
        try
        {
            int i = 0;
            if (weapon.UpgradeIndices != null)
            {
                foreach (int upgradeIndex in weapon.UpgradeIndices)
                {
                    if (!CheckWeaponResearch(upgradeIndex))
                        return i;
                    i++;
                }
            }

            return i;
        }
        catch (TrainerException)
        {
            return -1;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to get rank for {weapon.Name}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    public int GetItemRank(Constants.Item item)
    {
        try
        {
            int i = 0;
            if (item.UpgradeIndices != null)
            {
                foreach (int upgradeIndex in item.UpgradeIndices)
                {
                    if (!CheckItemResearch(upgradeIndex))
                        return i;
                    i++;
                }
            }

            return i;
        }
        catch (TrainerException)
        {
            return -1;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to get rank for {item.Name}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    public bool ResearchAndDevelopWeapon(Constants.IPwObject obj, bool research = true)
    {
        Logger?.Information($"Attempting to {(research ? "research" : "unresearch")} {obj.Name}");
        var weapon = (obj as Constants.Weapon)!;
        if (research)
            return ResearchWeapon(weapon, research) && DevelopWeapon(weapon, research) &&
                   UpdateWeaponUsageLevel(weapon, 1);
        return ResearchWeapon(weapon, research) && DevelopWeapon(weapon, research) &&
               UpdateWeaponUsageLevel(weapon, 0) && ChangeWeaponRank(weapon, research, 0);
    }

    public bool ResearchAndDevelopItem(Constants.IPwObject obj, bool research = true)
    {
        Logger?.Information($"Attempting to {(research ? "research" : "unresearch")} {obj.Name}");
        var item = (obj as Constants.Item)!;
        if (research)
            return ResearchItem(item, research) && DevelopItem(item, research);
        return ResearchItem(item, research) && DevelopItem(item, research) && ChangeItemRank(item, research, 0);
    }

    public bool ChangeWeaponUseLevel(Constants.IPwObject obj, bool increase = true)
    {
        Logger?.Information($"Attempting to level up {obj.Name}");
        Constants.Weapon weapon = (obj as Constants.Weapon)!;
        int allWeaponVersions = 1 + (weapon.UpgradeIndices?.Length ?? 0);
        int[] indices = new int[allWeaponVersions];
        indices[0] = weapon.Index;
        if (weapon.UpgradeIndices != null)
        {
            for(int i = 0; i < weapon.UpgradeIndices.Length; i++)
            {
                indices[i+1] = weapon.UpgradeIndices[i];
            }
        }

        foreach (int index in indices)
        {
            byte currentLevel = GetWeaponUsageLevel(index);
            if (currentLevel == 0xFF)
                return false;
            if (increase)
            {
                if (currentLevel < 3)
                    UpdateWeaponUsageLevel(index, (byte)(currentLevel + 1));
                else
                {
                    return false;
                }
            }
            else
            {
                if (currentLevel > 0)
                    UpdateWeaponUsageLevel(index, (byte)(currentLevel - 1));
                else
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool ChangeWeaponRank(Constants.IPwObject obj, bool increase = true, int? desiredRank = null)
    {
        Logger?.Information($"Attempting to rank up {obj.Name}");
        Constants.Weapon weapon = (obj as Constants.Weapon)!;
        
        int currentRank = GetWeaponRank(weapon);
        desiredRank ??= increase ? currentRank + 1 : currentRank - 1;
        while (currentRank != desiredRank)
        {
            if (increase)
            {
                if (currentRank < weapon.UpgradeIndices?.Length)
                {
                    ResearchWeapon(weapon.UpgradeIndices[currentRank]);
                    DevelopWeapon(weapon.UpgradeIndices[currentRank]);
                    UpdateWeaponUsageLevel(weapon.UpgradeIndices[currentRank], 1);
                    currentRank++;
                }
                else
                {
                    break;
                }
            }
            else
            {
                currentRank--;
                if (currentRank >= 0)
                {
                    ResearchWeapon(weapon.UpgradeIndices![currentRank], false);
                    DevelopWeapon(weapon.UpgradeIndices[currentRank], false);
                }
            }
        }

        return true;
    }
    
    public bool ChangeWeaponStock(Constants.IPwObject obj, int delta = 100)
    {
        Logger?.Information($"Attempting to adjust stock for {obj.Name} by {delta}");
        Constants.Weapon weapon = (obj as Constants.Weapon)!;
        int allItemVersions = 1 + (weapon.UpgradeIndices?.Length ?? 0);
        int[] indices = new int[allItemVersions];
        indices[0] = weapon.Index;
        if (weapon.UpgradeIndices != null)
        {
            for(int i = 0; i < weapon.UpgradeIndices.Length; i++)
            {
                indices[i+1] = weapon.UpgradeIndices[i];
            }
        }

        foreach (int index in indices)
        {
            int currentStock = GetWeaponStock(index);
            UpdateWeaponStock(index, currentStock + delta);
        }

        return true;
    }

    public bool ChangeItemStock(Constants.IPwObject obj, int delta = 100)
    {
        Logger?.Information($"Attempting to adjust stock for {obj.Name} by {delta}");
        var item = (obj as Constants.Item)!;
        var allItemVersions = 1 + (item.UpgradeIndices?.Length ?? 0);
        var indices = new int[allItemVersions];
        indices[0] = item.Index;
        if (item.UpgradeIndices != null)
        {
            for(var i = 0; i < item.UpgradeIndices.Length; i++)
            {
                indices[i+1] = item.UpgradeIndices[i];
            }
        }

        foreach (var index in indices)
        {
            var currentLevel = GetItemStock(index);
            UpdateItemStock(index, currentLevel + delta);
        }

        return true;
    }
    
    public bool ChangeItemRank(Constants.IPwObject obj, bool increase = true, int? desiredRank = null)
    {
        Logger?.Information($"Attempting to rank up {obj.Name}");
        Constants.Item item = (obj as Constants.Item)!;
        
        int currentRank = GetItemRank(item);
        desiredRank ??= increase ? currentRank + 1 : currentRank - 1;
        while (currentRank != desiredRank)
        {
            if (increase)
            {
                if (currentRank < item.UpgradeIndices?.Length)
                {
                    ResearchItem(item.UpgradeIndices[currentRank]);
                    DevelopItem(item.UpgradeIndices[currentRank]);
                    currentRank++;
                }
                else
                {
                    break;
                }
            }
            else
            {
                currentRank--;
                if (currentRank >= 0)
                {
                    ResearchItem(item.UpgradeIndices![currentRank], false);
                    DevelopItem(item.UpgradeIndices[currentRank], false);
                }
            }
        }

        return true;
    }

    public void SetAiBoardPullTimer(int time)
    {
        const int secondsToFrameTimeFactor = 300;
        Logger?.Information($"Attempting to set AI Board Pull Timer to {time} seconds...");
        time *= secondsToFrameTimeFactor; 
        
        try
        {
            if (MgsPwMonitor.MgsPwProcess == null)
            {
                throw new TrainerException();
            }

            nint ptrLocation;
            lock (MgsPwMonitor.MgsPwProcess)
            {
                using SimpleProcessProxy spp = new(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                ptrLocation =
                    spp.FollowPointer(PeaceWalkerApplicationNavigator.PeaceWalkerAoB.AiPullTimerPtrLocation, true);
                ptrLocation = IntPtr.Add(ptrLocation,
                    PeaceWalkerApplicationNavigator.PeaceWalkerAoB.AiPullTimerPtrOffset);
            }

            SetMemoryAtPointer(ptrLocation, BitConverter.GetBytes(time));
        }
        catch (TrainerException)
        {
            throw;
        }
        catch (Exception e)
        {
            string baseMessage = "Failed to set AI Board Pull Timer";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
}