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
            if(_weaponsArrayLocation == nint.MinValue)
            {
                try
                {
                    if (MgsPwMonitor.MgsPwProcess == null)
                        throw new NullReferenceException("Not hooked into PW, cannot set memory.");
                    lock (MgsPwMonitor.MgsPwProcess)
                    {
                        using SimpleProcessProxy spp = new(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName); //I'm fucking something up here, idk what though.
                        nint ptrLocation =
                            spp.FollowPointer(PeaceWalkerApplicationNavigator.PeaceWalkerAoB.WeaponsPtrLocation, true);
                        _weaponsArrayLocation = IntPtr.Add(ptrLocation,
                            PeaceWalkerApplicationNavigator.PeaceWalkerAoB.WeaponsPtrOffset);
                    }
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

    public string GetCurrentStage()
    {
        if (MgsPwMonitor.MgsPwProcess == null)
            return null;
        lock (MgsPwMonitor.MgsPwProcess)
        {
            using (SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName))
            {
                if (_stageLocation == nint.MinValue)
                {
                    nint ptrLocation =
                        spp.FollowPointer(PeaceWalkerApplicationNavigator.PeaceWalkerAoB.StagePtrLocation, true);

                    _stageLocation = IntPtr.Add(ptrLocation,
                        PeaceWalkerApplicationNavigator.PeaceWalkerAoB.StagePtrOffset);
                }

                return Encoding.UTF8.GetString(spp.GetMemoryFromPointer(_stageLocation, 18));
            }
        }
    }
    
    private bool ResearchWeapon(Constants.Weapon weapon, bool research = true)
    {
        //Set 0x04 in the array to 3
        try
        {
            IntPtr desiredPtr = IntPtr.Add(WeaponsArrayLocation,
                Constants.WeaponSize * (weapon.Index - 1) + (int)Constants.WeaponMemory.Research);
            return SetMemoryAtPointer(desiredPtr, research ? [0x03] : [0x01]);
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to research {weapon.Name}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }

    private bool ResearchWeapon(int index, bool research = true)
    {
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(WeaponsArrayLocation, Constants.WeaponSize * (index - 1) + (int)Constants.WeaponMemory.Research),
                research ? [0x03] : [0x01]);
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
        //Set 0x04 in the array to 3
        try
        {
            IntPtr desiredPtr = IntPtr.Add(ItemsArrayLocation,
                Constants.ItemSize * (item.Index - 1) + (int)Constants.ItemMemory.Research);
            return SetMemoryAtPointer(desiredPtr, research ? [0x03] : [0x01]);
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to research {item.Name}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }

    private bool ResearchItem(int index, bool research = true)
    {
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(ItemsArrayLocation, Constants.ItemSize * (index - 1) + (int)Constants.ItemMemory.Research),
                research ? [0x03] : [0x01]);
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
        catch (Exception e)
        {
            string baseMessage = $"Failed to check research status for item index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool DevelopWeapon(Constants.Weapon weapon, bool develop = true)
    {
        //Set 0x08 in the array to 64
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (weapon.Index - 1) + (int)Constants.WeaponMemory.Development),
                develop ? [0x64] : [0x00]);
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to develop {weapon.Name}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
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
        catch (Exception e)
        {
            string baseMessage = $"Failed to develop weapon index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool CheckWeaponDevelopment(int index)
    {
        try
        {
            return GetMemoryAtOffset(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (index - 1) + (int)Constants.WeaponMemory.Development), 1)[0] == 0x64;
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
        //Set 0x08 in the array to 64
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (item.Index - 1) + (int)Constants.ItemMemory.Development),
                develop ? [0x64] : [0x00]);
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to develop {item.Name}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
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
        catch (Exception e)
        {
            string baseMessage = $"Failed to develop item index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool CheckItemDevelopment(int index)
    {
        try
        {
            return GetMemoryAtOffset(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (index - 1) + (int)Constants.ItemMemory.Development), 1)[0] == 0x64;
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to check development status for item index {index}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool UpdateWeaponStock(Constants.Weapon weapon, uint stock)
    {
        //TODO: validate
        //Set 0x0C in the array to desired value
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (weapon.Index - 1) + (int)Constants.WeaponMemory.Stock),
                BitConverter.GetBytes(stock));
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to update stock for {weapon.Name}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool UpdateWeaponStock(int weapon, uint stock)
    {
        //TODO: validate
        //Set 0x0C in the array to desired value
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (weapon - 1) + (int)Constants.WeaponMemory.Stock),
                BitConverter.GetBytes(stock));
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
        //TODO: validate
        //Set 0x0C in the array to desired value
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (item.Index - 1) + (int)Constants.ItemMemory.Stock),
                BitConverter.GetBytes(stock));
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to update stock for {item.Name}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private bool UpdateItemStock(int item, int stock)
    {
        //TODO: validate
        //Set 0x0C in the array to desired value
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (item - 1) + (int)Constants.ItemMemory.Stock),
                BitConverter.GetBytes(stock));
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
        //TODO: validate
        //Set 0x0C in the array to desired value
        try
        {
            return BitConverter.ToInt32(GetMemoryAtOffset(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (item.Index - 1) + (int)Constants.ItemMemory.Stock), 4));
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to get stock for {item.Name}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private int GetItemStock(int item)
    {
        //TODO: validate
        //Set 0x0C in the array to desired value
        try
        {
            return BitConverter.ToInt32(GetMemoryAtOffset(
                IntPtr.Add(ItemsArrayLocation,
                    Constants.ItemSize * (item - 1) + (int)Constants.ItemMemory.Stock), 4));
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
        //Set 0x16 in the array to desired value
        try
        {
            return SetMemoryAtPointer(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (weapon.Index - 1) + (int)Constants.WeaponMemory.UsageLevel),
                [usageLevel]);
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to update usage level for {weapon.Name}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
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
        catch (Exception e)
        {
            string baseMessage = $"Failed to update usage level for weapon index {weapon}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }

    private byte GetWeaponUsageLevel(Constants.Weapon weapon)
    {
        try
        {
            return GetMemoryAtOffset(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (weapon.Index - 1) + (int)Constants.WeaponMemory.UsageLevel), 1)[0];
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to get usage level for {weapon.Name}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private byte GetWeaponUsageLevel(int weapon)
    {
        try
        {
            return GetMemoryAtOffset(
                IntPtr.Add(WeaponsArrayLocation,
                    Constants.WeaponSize * (weapon - 1) + (int)Constants.WeaponMemory.UsageLevel), 1)[0];
        }
        catch (Exception e)
        {
            string baseMessage = $"Failed to get usage level for weapon index {weapon}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }

    private int GetWeaponRank(Constants.Weapon weapon)
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
        catch (Exception e)
        {
            string baseMessage = $"Failed to get rank for {weapon.Name}";
            Logger?.Error($"{baseMessage}: {e}");
            throw new AggregateException(baseMessage, e);
        }
    }
    
    private int GetItemRank(Constants.Item item)
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
        return ResearchWeapon(weapon, research) && DevelopWeapon(weapon, research) &&
               UpdateWeaponUsageLevel(weapon, research ? (byte)1 : (byte)0);
    }

    public bool ResearchAndDevelopItem(Constants.IPwObject obj, bool research = true)
    {
        Logger?.Information($"Attempting to {(research ? "research" : "unresearch")} {obj.Name}");
        var item = (obj as Constants.Item)!;
        return ResearchItem(item, research) && DevelopItem(item, research);
    }

    public bool ChangeWeaponLevel(Constants.IPwObject obj, bool increase = true)
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
            if (increase)
            {
                if (currentLevel < 3)
                    UpdateWeaponUsageLevel(index, (byte)(currentLevel + 1));
            }
            else
            {
                if (currentLevel > 0)
                    UpdateWeaponUsageLevel(index, (byte)(currentLevel - 1));
            }
        }

        return false;
    }

    public bool ChangeWeaponRank(Constants.IPwObject obj, bool increase = true)
    {
        Logger?.Information($"Attempting to rank up {obj.Name}");
        Constants.Weapon weapon = (obj as Constants.Weapon)!;
        int currentRank = GetWeaponRank(weapon);
        if (increase)
        {
            if (currentRank < weapon.UpgradeIndices?.Length)
                return ResearchWeapon(weapon.UpgradeIndices[currentRank]) &&
                       DevelopWeapon(weapon.UpgradeIndices[currentRank]);
        }
        else
        {
            if (currentRank > 0)
                return ResearchWeapon(weapon.UpgradeIndices![currentRank], false) &&
                       DevelopWeapon(weapon.UpgradeIndices[currentRank], false);
        }

        return false;
    }

    public bool ChangeItemStock(Constants.IPwObject obj, int delta = 100)
    {
        Logger?.Information($"Attempting to adjust stock for {obj.Name} by {delta}");
        Constants.Item item = (obj as Constants.Item)!;
        int allItemVersions = 1 + (item.UpgradeIndices?.Length ?? 0);
        int[] indices = new int[allItemVersions];
        indices[0] = item.Index;
        if (item.UpgradeIndices != null)
        {
            for(int i = 0; i < item.UpgradeIndices.Length; i++)
            {
                indices[i+1] = item.UpgradeIndices[i];
            }
        }

        foreach (int index in indices)
        {
            int currentLevel = GetItemStock(index);
            UpdateItemStock(index, currentLevel + delta);
        }

        return true;
    }
    
    public bool ChangeItemRank(Constants.IPwObject obj, bool increase = true)
    {
        Logger?.Information($"Attempting to rank up {obj.Name}");
        Constants.Item item = (obj as Constants.Item)!;
        int currentRank = GetItemRank(item);
        if (increase)
        {
            if (currentRank < item.UpgradeIndices?.Length)
                return ResearchItem(item.UpgradeIndices[currentRank]) &&
                       DevelopItem(item.UpgradeIndices[currentRank]);
        }
        else
        {
            if (currentRank > 0)
                return ResearchItem(item.UpgradeIndices![currentRank], false) &&
                       DevelopItem(item.UpgradeIndices[currentRank], false);
        }

        return false;
    }
}