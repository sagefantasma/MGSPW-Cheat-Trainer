using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using MGSPW_MC_Cheat_Trainer.Models;
using Serilog;
using SimplifiedMemoryManager;
using static MGSPW_MC_Cheat_Trainer.Models.PeaceWalkerApplicationNavigator.PeaceWalkerAoB;

namespace MGSPW_MC_Cheat_Trainer;

public class GameCheat(Action<bool> action, byte[]? originalBytes, Constants.Cheat? cheatType)
{
    public Action<bool> CheatAction { get; private set; } = action;
    private byte[]? OriginalBytes { get; set; } = originalBytes;
    private IntPtr CodeLocation { get; set; } = IntPtr.Zero;
    public Constants.Cheat? CheatType { get; set; } = cheatType;
    private static ILogger? Logger => LogManager.Logger;
    private static List<GameCheat> ActiveCheats { get; set; } = new();

    private static class BaseActions
    {
        internal static void ReplaceWithOriginalCode(IntPtr memoryLocation, byte[] bytesToReplace, MemoryOffset offset, int startIndexToReplace = 0)
        {
            //NOTE: this is only really usable if the Offset starts with zero because I'm an idiot :)
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                bool successful = false;
                int retries = 5;
                do
                {
                    try
                    {
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                        if (memoryLocation != IntPtr.Zero)
                        {
                            byte[] memoryContent = spp.GetMemoryFromPointer(IntPtr.Add(memoryLocation, offset.Start),
                                    offset.Length);

                            for (int i = startIndexToReplace; i < startIndexToReplace + bytesToReplace.Length; i++)
                            {
                                if(memoryContent.Length > i)
                                    memoryContent[i] = bytesToReplace[i];
                            }

                            spp.SetMemoryAtPointer(memoryLocation, memoryContent);
                            successful = true;
                        }
                    }
                    catch (Exception e)
                    {
                        retries--;
                        if (retries == 0)
                        {
                            throw new AggregateException("Failed to activate cheat, abandoning process", e);
                        }
                    }
                } while (!successful && retries > 0);
            }
        }

        internal static IntPtr ReplaceWithInvalidCode(string aob, MemoryOffset offset, int bytesToReplace, int startIndexToReplace = 0)
        {
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            bool successful = false;
            int retries = 5;
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                do
                {
                    try
                    {
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                        SimplePattern pattern = new SimplePattern(aob);
                        IntPtr memoryLocation = spp.ScanMemoryForUniquePatternAsync(pattern).Result.Offset;

                        if (memoryLocation != -1)
                        {
                            byte[] memoryContent = spp.GetMemoryFromPointer(
                                IntPtr.Add(memoryLocation, offset.Start),
                                offset.Length);

                            for (int i = startIndexToReplace; i < startIndexToReplace + bytesToReplace; i++)
                            {
                                memoryContent[i] = 0x90;
                            }

                            spp.SetMemoryAtPointer(IntPtr.Add(memoryLocation, offset.Start), memoryContent);
                            successful = true;

                            return memoryLocation;
                        }
                    }
                    catch (Exception e)
                    {
                        retries--;
                        if (retries == 0)
                        {
                            throw new AggregateException("Failed to activate cheat, abandoning process", e);
                        }
                    }
                } while (!successful && retries > 0);
            }

            return IntPtr.Zero;
        }

        internal static void ReplaceWithInvalidCode(IntPtr memoryLocation, MemoryOffset offset, int bytesToReplace, int startIndexToReplace = 0)
        {
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                bool successful = false;
                int retries = 5;
                do
                {
                    try
                    {
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                        if (memoryLocation != IntPtr.Zero)
                        {
                            byte[] memoryContent = spp.GetMemoryFromPointer(IntPtr.Add(memoryLocation, offset.Start),
                                offset.Length);

                            for (int i = startIndexToReplace; i < startIndexToReplace + bytesToReplace; i++)
                            {
                                memoryContent[i] = 0x90;
                            }

                            spp.SetMemoryAtPointer(IntPtr.Add(memoryLocation, offset.Start), memoryContent);
                            successful = true;
                        }
                    }
                    catch (Exception e)
                    {
                        retries--;
                        if (retries == 0)
                        {
                            throw new AggregateException("Failed to activate cheat, abandoning process", e);
                        }
                    }
                } while (!successful && retries > 0);
            }
        }

        internal static IntPtr ReplaceWithSpecificCode(string patternToScan, byte[] replacementBytes, MemoryOffset offset)
        {
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            bool successful = false;
            int retries = 5;
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                do
                {
                    try
                    {
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                        SimplePattern pattern = new SimplePattern(patternToScan);
                        IntPtr memoryLocation = spp.ScanMemoryForUniquePatternAsync(pattern).Result.Offset;

                        if (memoryLocation != -1)
                        {
                            byte[] memoryContent = spp.GetMemoryFromPointer(
                                IntPtr.Add(memoryLocation, offset.Start),
                                offset.Length);

                            for (int i = 0; i < replacementBytes.Length; i++)
                            {
                                memoryContent[i] = replacementBytes[i];
                            }

                            spp.SetMemoryAtPointer(IntPtr.Add(memoryLocation, offset.Start), memoryContent);
                            successful = true;

                            return memoryLocation;
                        }
                    }
                    catch (Exception e)
                    {
                        retries--;
                        if (retries == 0)
                        {
                            throw new AggregateException("Failed to activate cheat, abandoning process", e);
                        }
                    }
                } while (!successful && retries > 0);
            }
            throw new Exception("Failed to replace code, aborting the process");
        }

        internal static void ReplaceWithSpecificCode(IntPtr memoryLocation, byte[] replacementBytes, MemoryOffset offset)
        {
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                bool successful = false;
                int retries = 5;
                do
                {
                    try
                    {
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                        if (memoryLocation != IntPtr.Zero)
                        {
                            byte[] memoryContent = spp.GetMemoryFromPointer(IntPtr.Add(memoryLocation, offset.Start),
                                offset.Length);

                            for (int i = 0; i < replacementBytes.Length; i++)
                            {
                                memoryContent[i] = replacementBytes[i];
                            }

                            spp.SetMemoryAtPointer(new IntPtr(memoryLocation + offset.Start), memoryContent);
                            successful = true;
                        }
                    }
                    catch (Exception e)
                    {
                        retries--;
                        if (retries == 0)
                        {
                            throw new AggregateException("Failed to activate cheat, abandoning process", e);
                        }
                    }
                } while (!successful && retries > 0);
            }
        }

        internal static IntPtr ModifySingleByte(string aob, MemoryOffset offset, byte replacementValue)
        {
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            bool successful = false;
            int retries = 5;
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                do
                {
                    try
                    {
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                        SimplePattern pattern = new SimplePattern(aob);
                        IntPtr memoryLocation = spp.ScanMemoryForUniquePatternAsync(pattern).Result.Offset;

                        if (memoryLocation != -1)
                        {
                            spp.SetMemoryAtPointer(IntPtr.Add(memoryLocation, offset.Start), [replacementValue]);
                            successful = true;

                            return memoryLocation;
                        }
                    }
                    catch (Exception e)
                    {
                        retries--;
                        if (retries == 0)
                        {
                            throw new AggregateException("Failed to activate cheat, abandoning process", e);
                        }
                    }
                } while (!successful && retries > 0);
            }

            return IntPtr.Zero;
        }

        internal static void ModifySingleByte(IntPtr memoryLocation, MemoryOffset offset, byte replacementValue)
        {
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                bool successful = false;
                int retries = 5;
                do
                {
                    try
                    {
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                        if (memoryLocation != IntPtr.Zero)
                        {
                            spp.SetMemoryAtPointer(IntPtr.Add(memoryLocation, offset.Start), [replacementValue]);
                            successful = true;
                        }
                    }
                    catch (Exception e)
                    {
                        retries--;
                        if (retries == 0)
                        {
                            throw new AggregateException("Failed to activate cheat, abandoning process", e);
                        }
                    }
                } while (!successful && retries > 0);
            }
        }

        internal static byte[] ReadMemory(string aob, MemoryOffset offset)
        {
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            bool successful = false;
            int retries = 5;
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                do
                {
                    try
                    {
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                        SimplePattern pattern = new SimplePattern(aob);
                        IntPtr memoryLocation = spp.ScanMemoryForUniquePatternAsync(pattern).Result.Offset;

                        if (memoryLocation != -1)
                            return spp.GetMemoryFromPointer(IntPtr.Add(memoryLocation, offset.Start),
                                offset.Length);
                    }
                    catch (Exception e)
                    {
                        retries--;
                        if (retries == 0)
                        {
                            throw new AggregateException("Failed to activate cheat, abandoning process", e);
                        }
                    }
                } while (!successful && retries > 0);
            }

            throw new Exception("Failed to read process memory, aborting cheat process");
        }
        
        internal static byte[] ReadMemory(IntPtr memoryLocation, MemoryOffset offset)
        {
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            bool successful = false;
            int retries = 5;
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                do
                {
                    try
                    {
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess, MgsPwMonitor.MgsPwProcessName);
                        
                        return spp.GetMemoryFromPointer(IntPtr.Add(memoryLocation, offset.Start),
                            offset.Length);
                    }
                    catch (Exception e)
                    {
                        retries--;
                        if (retries == 0)
                        {
                            throw new AggregateException("Failed to activate cheat, abandoning process", e);
                        }
                    }
                } while (!successful && retries > 0);
            }

            throw new Exception("Failed to read process memory, aborting cheat process");
        }
    }

    public static void DeactivateActiveCheats()
    {
        //foreach (GameCheat cheat in ActiveCheats)
        int cheatsToDisable = ActiveCheats.Count;
        for(int i = 0; i < cheatsToDisable; i++)
        {
            ActiveCheats[0].CheatAction(false); //Disabling a cheat removes it from the list, so just deactivate 0 i times.
        }
    }

    private static class CheatActions
    {
        public static void ToggleUnlimitedLife(bool activate)
        {
            //Works as intended
            GameCheat activeGameCheat = PeaceWalkerCheat.UnlimitedLife;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                            UnlimitedLifeAoB,
                            UnlimitedLifeOffset,
                            UnlimitedLifeOffset.Length);
                        Logger?.Debug($"Unlimited life AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, UnlimitedLifeOffset,
                                UnlimitedLifeOffset.Length);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleUnlimitedLife(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.UnlimitedLife = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, UnlimitedLifeOffset,
                        UnlimitedLifeOffset.Length);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }

        public static void ToggleInvulnerable(bool activate)
        {
            //Not quite working as expected, but is fine enough
            GameCheat activeGameCheat = PeaceWalkerCheat.Invulnerable;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithSpecificCode(InvulnerableAoB,
                            InvulnerableBytes, InvulnerableOffset);
                        Logger?.Debug($"Invulnerability AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, InvulnerableBytes,
                                UnlimitedLifeOffset);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleInvulnerable(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.Invulnerable = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, InvulnerableBytes,
                        InvulnerableOffset);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleNoReload(bool activate)
        {
            //Works as expected
            GameCheat activeGameCheat = PeaceWalkerCheat.NoReload;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                            NoReloadAoB,
                            NoReloadOffset,
                            NoReloadOffset.Length);
                        Logger?.Debug($"No Reload AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!,
                            activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, NoReloadOffset,
                                NoReloadOffset.Length);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleNoReload(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.NoReload = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, NoReloadOffset,
                        NoReloadOffset.Length);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleUnlimitedAmmo(bool activate)
        {
            //Works as expected
            GameCheat activeGameCheat = PeaceWalkerCheat.UnlimitedAmmo;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                            UnlimitedAmmoAoB,
                            UnlimitedAmmoOffset,
                            UnlimitedAmmoOffset.Length);
                        Logger?.Debug($"Unlimited ammo AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, UnlimitedAmmoOffset,
                                UnlimitedAmmoOffset.Length);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleUnlimitedAmmo(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.UnlimitedAmmo = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, UnlimitedAmmoOffset,
                        UnlimitedAmmoOffset.Length);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleInfiniteSuppressor(bool activate)
        {
            //Works as expected
            GameCheat activeGameCheat = PeaceWalkerCheat.InfiniteSuppressor;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                            InfiniteSuppressorAoB,
                            InfiniteSuppressorOffset,
                            InfiniteSuppressorOffset.Length);
                        Logger?.Debug($"Infinite suppressor AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, InfiniteSuppressorOffset,
                                InfiniteSuppressorOffset.Length);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleInfiniteSuppressor(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.InfiniteSuppressor = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, InfiniteSuppressorOffset,
                        InfiniteSuppressorOffset.Length);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleUnlimitedPsyche(bool activate)
        {
            //Doesn't work, but also doesn't matter xdd
            GameCheat activeGameCheat = PeaceWalkerCheat.UnlimitedPsyche;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                            UnlimitedPsycheAoB,
                            UnlimitedPsycheOffset,
                            UnlimitedPsycheOffset.Length);
                        Logger?.Debug($"Unlimited psyche AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, UnlimitedPsycheOffset,
                                UnlimitedPsycheOffset.Length);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleUnlimitedPsyche(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.UnlimitedPsyche = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, UnlimitedPsycheOffset,
                        UnlimitedPsycheOffset.Length);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleFreezeAi(bool activate)
        {
            //Works as intended
            GameCheat activeGameCheat = PeaceWalkerCheat.FreezeAi;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithSpecificCode(FreezeAiAoB,
                            FreezeAiBytes, FreezeAiOffset);
                        Logger?.Debug($"Freeze AI AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, FreezeAiBytes,
                                FreezeAiOffset);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleFreezeAi(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.FreezeAi = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, FreezeAiBytes,
                        FreezeAiOffset);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleInvisibleToAi(bool activate)
        {
            //Works as intended, need renaming
            GameCheat activeGameCheat = PeaceWalkerCheat.InvisibleToAi;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithSpecificCode(InvisibleToAiAoB,
                            InvisibleToAiBytes, InvisibleToAiOffset);
                        Logger?.Debug($"Invisible to AI AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, InvisibleToAiBytes,
                                InvisibleToAiOffset);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleInvisibleToAi(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.InvisibleToAi = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, InvisibleToAiBytes,
                        NoTimeLimitOffset);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }

        internal static void ToggleMaxCamoSub1(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.MaxCamo1;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                            MaxCamo1AoB,
                            MaxCamo1Offset,
                            MaxCamo1Offset.Length);
                        Logger?.Debug($"Max Camo 1 AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, MaxCamo1Offset,
                                MaxCamo1Offset.Length);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleMaxCamoSub1(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.MaxCamo1 = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, MaxCamo1Offset,
                        MaxCamo1Offset.Length);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }

        internal static void ToggleMaxCamoSub2(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.MaxCamo2;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                            MaxCamo2AoB,
                            MaxCamo2Offset,
                            MaxCamo2Offset.Length);
                        Logger?.Debug($"Max Camo 2 AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, MaxCamo2Offset,
                                MaxCamo2Offset.Length);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleMaxCamoSub2(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.MaxCamo2 = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, MaxCamo2Offset,
                        MaxCamo2Offset.Length);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleMaxCamo(bool activate)
        {
            //Appears to work as expected
            ToggleMaxCamoSub1(activate);
            ToggleMaxCamoSub2(activate);
        }
        
        public static void ToggleUnlimitedEquipment(bool activate)
        {
            //Works as intended
            GameCheat activeGameCheat = PeaceWalkerCheat.UnlimitedEquipment;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                            UnlimitedEquipmentAoB,
                            UnlimitedEquipmentOffset,
                            UnlimitedEquipmentOffset.Length);
                        Logger?.Debug($"Unlimited equipment AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, UnlimitedEquipmentOffset,
                                UnlimitedEquipmentOffset.Length);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleUnlimitedEquipment(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.UnlimitedEquipment = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, UnlimitedEquipmentOffset,
                        UnlimitedEquipmentOffset.Length);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleNoTimeLimit(bool activate)
        {
            //Works on CheatEngine, not in trainer - my issue
            //Retest, everything looks right between CE <-> trainer
            GameCheat activeGameCheat = PeaceWalkerCheat.NoTimeLimit;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithSpecificCode(NoTimeLimitAoB,
                            NoTimeLimitBytes, NoTimeLimitOffset);
                        Logger?.Debug($"No time limit AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, NoTimeLimitOffset,
                                NoTimeLimitOffset.Length);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleNoTimeLimit(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.NoTimeLimit = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, NoTimeLimitBytes,
                        NoTimeLimitOffset);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleMaxStockOnPickup(bool activate)
        {
            //Works as intended
            GameCheat activeGameCheat = PeaceWalkerCheat.MaxStockOnPickup;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithSpecificCode(MaxStockOnPickupAoB,
                            MaxStockOnPickupBytes, MaxStockOnPickupOffset);
                        Logger?.Debug($"Max stock on pickup AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, MaxStockOnPickupBytes,
                                MaxStockOnPickupOffset);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleMaxStockOnPickup(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.MaxStockOnPickup = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, MaxStockOnPickupBytes,
                        MaxStockOnPickupOffset);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleFreezeMissionTime(bool activate)
        {
            //TODO: does not appear to work, need help from Swiss
            GameCheat activeGameCheat = PeaceWalkerCheat.FreezeMissionTime;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                            FreezeMissionTimeAoB,
                            FreezeMissionTimeOffset,
                            FreezeMissionTimeOffset.Length);
                        Logger?.Debug($"Freeze mission time AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, FreezeMissionTimeOffset,
                                FreezeMissionTimeOffset.Length);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleFreezeMissionTime(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.FreezeMissionTime = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, FreezeMissionTimeOffset,
                        FreezeMissionTimeOffset.Length);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleFreezeMissionStats(bool activate)
        {
            //Inconsistent, need help from Swiss to fix
            GameCheat activeGameCheat = PeaceWalkerCheat.FreezeMissionStats;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                            FreezeMissionStatsAoB,
                            FreezeMissionStatsOffset,
                            FreezeMissionStatsOffset.Length);
                        Logger?.Debug($"Freeze mission stats AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, FreezeMissionStatsOffset,
                                FreezeMissionStatsOffset.Length);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ToggleFreezeMissionStats(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.FreezeMissionStats = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, FreezeMissionStatsOffset,
                        FreezeMissionStatsOffset.Length);
                }
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }

        //private static Task? _vehicleCommanderPeriodicTask;
        private static CancellationTokenSource _vehicleCommanderCancellationTokenSource = new ();
        public static void ForceVehicleCommander(bool activate)
        {
            //Working as expected
            GameCheat activeGameCheat = PeaceWalkerCheat.ForceVehicleCommander;
            if (activate)
            {
                ActiveCheats.Add(activeGameCheat);
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        IntPtr location = BaseActions.ReplaceWithSpecificCode(VehicleBossAoB, new byte[0], VehicleBossOffset);
                        int vehicleBossLocation = BitConverter.ToInt32(BaseActions.ReadMemory(location, VehicleBossOffset));
                        activeGameCheat.CodeLocation = IntPtr.Add(location, vehicleBossLocation);
                        Logger?.Debug($"Force Vehicle Commander location found at: {activeGameCheat.CodeLocation}");
                        BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, ForceVehicleCommanderBytes, EscortCountOffset);
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, ForceVehicleCommanderBytes,
                                EscortCountOffset);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            ForceVehicleCommander(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.ForceVehicleCommander = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, ForceVehicleCommanderBytes, EscortCountOffset);
                }
                PeriodicTask.Run(() =>
                {
                        BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, ForceVehicleCommanderBytes, EscortCountOffset);
                }, TimeSpan.FromSeconds(.25), _vehicleCommanderCancellationTokenSource.Token);
            }
            else
            {
                ActiveCheats.Remove(activeGameCheat);
                _vehicleCommanderCancellationTokenSource?.Cancel();
                _vehicleCommanderCancellationTokenSource = new CancellationTokenSource(); //Prep for a possible re-run
            }
        }

        internal static void WalkThroughWalls(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.WalkThroughWalls;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithSpecificCode(
                            NoClipAoB,
                            NoClipBytes,
                            NoClipOffset);
                        Logger?.Debug($"Walk through walls AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, NoClipBytes,
                                NoClipOffset);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            WalkThroughWalls(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.NoClip = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, NoClipBytes,
                        NoClipOffset);
                }
            }
            else
            {
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }

        internal static void MaintainHeight(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.MaintainHeight;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = (nint)AoBCacheManager.CheckCache(activeGameCheat.CheatType.ToString()!);
                    if (activeGameCheat.CodeLocation == nint.MinValue)
                    {
                        activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                            HoldYPositionAoB,
                            HoldYPositionOffset,
                            HoldYPositionOffset.Length);
                        Logger?.Debug($"Maintain height AoB found at: {activeGameCheat.CodeLocation}");
                        AoBCacheManager.SaveToCache(activeGameCheat.CheatType.ToString()!, activeGameCheat.CodeLocation);
                    }
                    else
                    {
                        Logger?.Information($"Attempting to use cached AoB info for {activeGameCheat.CheatType}...");
                        try
                        {
                            BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, HoldYPositionOffset,
                                HoldYPositionOffset.Length);
                        }
                        catch
                        {
                            Logger?.Error("Cached AoB value failed, removing from cache and retrying...");
                            AoBCacheManager.RemoveFromCache(activeGameCheat.CheatType.ToString()!);
                            MaintainHeight(activate);
                            return;
                        }
                    }
                    PeaceWalkerCheat.MaintainHeight = activeGameCheat;
                }
                else
                {
                    BaseActions.ReplaceWithInvalidCode(activeGameCheat.CodeLocation, HoldYPositionOffset,
                        HoldYPositionOffset.Length);
                }
            }
            else
            {
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }

        public static void NoClip(bool activate)
        {
            //Works as intended
            if (activate)
            {
                ActiveCheats.Add(PeaceWalkerCheat.NoClip);
                MaintainHeight(activate);
                WalkThroughWalls(activate);
            }
            else
            {
                ActiveCheats.Remove(PeaceWalkerCheat.NoClip);
                WalkThroughWalls(activate);
                MaintainHeight(activate);
            }
        }
    }

    public static class PeaceWalkerCheat
    {
        public static GameCheat UnlimitedLife { get; internal set; } = new(CheatActions.ToggleUnlimitedLife,
            OriginalLifeBytes, Constants.Cheat.UnlimitedLife);
        public static GameCheat Invulnerable { get; internal set; } = new(CheatActions.ToggleInvulnerable,
            OriginalVulnerableBytes, Constants.Cheat.Invulnerable);
        public static GameCheat NoReload { get; internal set; } = new(CheatActions.ToggleNoReload,
            OriginalReloadBytes, Constants.Cheat.NoReload);
        public static GameCheat UnlimitedAmmo { get; internal set; } = new(CheatActions.ToggleUnlimitedAmmo,
            OriginalAmmoBytes, Constants.Cheat.UnlimitedAmmo);
        public static GameCheat InfiniteSuppressor { get; internal set; } = new(CheatActions.ToggleInfiniteSuppressor,
            OriginalSuppressorBytes, Constants.Cheat.InfiniteSuppressor);
        public static GameCheat UnlimitedPsyche { get; internal set; } = new(CheatActions.ToggleUnlimitedPsyche,
            OriginalPsycheBytes, Constants.Cheat.UnlimitedPsyche);
        public static GameCheat FreezeAi { get; internal set; } = new(CheatActions.ToggleFreezeAi,
            OriginalAiBytes, Constants.Cheat.FreezeAi);
        public static GameCheat InvisibleToAi { get; internal set; } = new(CheatActions.ToggleInvisibleToAi,
            OriginalVisibleToAiBytes, Constants.Cheat.InvisibleToAi);
        public static GameCheat MaxCamo1 { get; internal set; } = new(CheatActions.ToggleMaxCamoSub1,
            OriginalCamo1Bytes, null);
        public static GameCheat MaxCamo2 { get; internal set; } = new(CheatActions.ToggleMaxCamoSub2,
            OriginalCamo2Bytes, null);
        public static GameCheat MaxCamo { get; internal set; } = new(CheatActions.ToggleMaxCamo,
            null, Constants.Cheat.MaxCamo);
        public static GameCheat UnlimitedEquipment { get; internal set; } = new(CheatActions.ToggleUnlimitedEquipment,
            OriginalEquipmentBytes, Constants.Cheat.UnlimitedEquipment);
        public static GameCheat NoTimeLimit { get; internal set; } = new(CheatActions.ToggleNoTimeLimit,
            OriginalTimeLimitBytes, Constants.Cheat.NoTimeLimit);
        public static GameCheat MaxStockOnPickup { get; internal set; } = new(CheatActions.ToggleMaxStockOnPickup,
            OriginalStockOnPickupBytes, Constants.Cheat.MaxStockOnPickup);
        public static GameCheat FreezeMissionTime { get; internal set; } = new(CheatActions.ToggleFreezeMissionTime,
            OriginalMissionTimeBytes, Constants.Cheat.FreezeMissionTime);
        public static GameCheat FreezeMissionStats { get; internal set; } = new(CheatActions.ToggleFreezeMissionStats,
            OriginalMissionStatsBytes, Constants.Cheat.FreezeMissionStats);
        public static GameCheat ForceVehicleCommander { get; internal set; } = new(CheatActions.ForceVehicleCommander,
            null, Constants.Cheat.ForceVehicleCommander);
        public static GameCheat WalkThroughWalls { get; internal set; } =
            new(CheatActions.WalkThroughWalls, OriginalClipBytes, null);
        public static GameCheat MaintainHeight { get; internal set; } =
            new(CheatActions.MaintainHeight, OriginalYPositionBytes, null);
        public static GameCheat NoClip { get; internal set; } =
            new(CheatActions.NoClip, null, Constants.Cheat.NoClip);

        public static readonly List<GameCheat> CheatList =
        [
            UnlimitedLife, Invulnerable, NoReload,UnlimitedAmmo, InfiniteSuppressor, UnlimitedPsyche,
            FreezeAi, InvisibleToAi, MaxCamo, UnlimitedEquipment, NoTimeLimit, MaxStockOnPickup,
            FreezeMissionTime, FreezeMissionStats, ForceVehicleCommander, NoClip
        ];
    }
}