using System;
using System.Collections.Generic;
using MGSPW_MC_Cheat_Trainer.Models;
using SimplifiedMemoryManager;
using static MGSPW_MC_Cheat_Trainer.Models.PeaceWalkerApplicationNavigator.PeaceWalkerAoB;

namespace MGSPW_MC_Cheat_Trainer;

public class GameCheat(Action<bool> action, byte[]? originalBytes, Constants.Cheat? cheatType)
{
    public Action<bool> CheatAction { get; private set; } = action;
    private byte[]? OriginalBytes { get; set; } = originalBytes;
    private IntPtr CodeLocation { get; set; } = IntPtr.Zero;
    public Constants.Cheat? CheatType { get; set; } = cheatType;

    private static class BaseActions
    {
        internal static void ReplaceWithOriginalCode(IntPtr memoryLocation, MemoryOffset offset, byte[] bytesToReplace, int startIndexToReplace = 0)
        {
            //TODO: this probably needs to be modified -- this old MGS2 code was essentially reliant on offset always being 0 I think...
            if (MgsPwMonitor.MgsPwProcess is null) throw new Exception("Not hooked into game");
            lock (MgsPwMonitor.MgsPwProcess)
            {
                bool successful = false;
                int retries = 5;
                do
                {
                    try
                    {
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess);
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
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess);
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
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess);
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
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess);
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
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess);
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
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess);
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
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess);
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
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess);
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
                        using SimpleProcessProxy spp = new SimpleProcessProxy(MgsPwMonitor.MgsPwProcess);
                        
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

    private static class CheatActions
    {
        public static void ToggleUnlimitedLife(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.UnlimitedLife;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                        UnlimitedLifeAoB,
                        UnlimitedLifeOffset,
                        UnlimitedLifeOffset.Length);
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
                BaseActions.ReplaceWithOriginalCode(activeGameCheat.CodeLocation, UnlimitedLifeOffset,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException());
            }
        }

        public static void ToggleInvulnerable(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.Invulnerable;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithSpecificCode(InvulnerableAoB,
                        InvulnerableBytes, InvulnerableOffset);
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
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleNoReload(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.NoReload;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                        NoReloadAoB,
                        NoReloadOffset,
                        NoReloadOffset.Length);
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
                BaseActions.ReplaceWithOriginalCode(activeGameCheat.CodeLocation, NoReloadOffset,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException());
            }
        }
        
        public static void ToggleUnlimitedAmmo(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.UnlimitedAmmo;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                        UnlimitedAmmoAoB,
                        UnlimitedAmmoOffset,
                        UnlimitedAmmoOffset.Length);
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
                BaseActions.ReplaceWithOriginalCode(activeGameCheat.CodeLocation, UnlimitedAmmoOffset,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException());
            }
        }
        
        public static void ToggleInfiniteSuppressor(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.InfiniteSuppressor;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                        InfiniteSuppressorAoB,
                        InfiniteSuppressorOffset,
                        InfiniteSuppressorOffset.Length);
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
                BaseActions.ReplaceWithOriginalCode(activeGameCheat.CodeLocation, InfiniteSuppressorOffset,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException());
            }
        }
        
        public static void ToggleUnlimitedPsyche(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.UnlimitedPsyche;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                        UnlimitedPsycheAoB,
                        UnlimitedPsycheOffset,
                        UnlimitedPsycheOffset.Length);
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
                BaseActions.ReplaceWithOriginalCode(activeGameCheat.CodeLocation, UnlimitedPsycheOffset,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException());
            }
        }
        
        public static void ToggleFreezeAi(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.FreezeAi;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithSpecificCode(FreezeAiAoB,
                        FreezeAiBytes, FreezeAiOffset);
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
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleInvisibleToAi(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.InvisibleToAi;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithSpecificCode(InvisibleToAiAoB,
                        InvisibleToAiBytes, InvisibleToAiOffset);
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
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }

        internal static void ToggleMaxCamoSub1(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.MaxCamo1;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                        MaxCamo1AoB,
                        MaxCamo1Offset,
                        MaxCamo1Offset.Length);
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
                BaseActions.ReplaceWithOriginalCode(activeGameCheat.CodeLocation, MaxCamo1Offset,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException());
            }
        }

        internal static void ToggleMaxCamoSub2(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.MaxCamo2;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                        MaxCamo2AoB,
                        MaxCamo2Offset,
                        MaxCamo2Offset.Length);
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
                BaseActions.ReplaceWithOriginalCode(activeGameCheat.CodeLocation, MaxCamo2Offset,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException());
            }
        }
        
        public static void ToggleMaxCamo(bool activate)
        {
            ToggleMaxCamoSub1(activate);
            ToggleMaxCamoSub2(activate);
        }
        
        public static void ToggleUnlimitedEquipment(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.UnlimitedEquipment;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                        UnlimitedEquipmentAoB,
                        UnlimitedEquipmentOffset,
                        UnlimitedEquipmentOffset.Length);
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
                BaseActions.ReplaceWithOriginalCode(activeGameCheat.CodeLocation, UnlimitedEquipmentOffset,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException());
            }
        }
        
        public static void ToggleNoTimeLimit(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.NoTimeLimit;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithSpecificCode(NoTimeLimitAoB,
                        NoTimeLimitBytes, NoTimeLimitOffset);
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
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleMaxStockOnPickup(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.MaxStockOnPickup;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithSpecificCode(MaxStockOnPickupAoB,
                        MaxStockOnPickupBytes, MaxStockOnPickupOffset);
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
                BaseActions.ReplaceWithSpecificCode(activeGameCheat.CodeLocation, activeGameCheat.OriginalBytes ?? throw new InvalidOperationException(), new MemoryOffset(0, activeGameCheat.OriginalBytes.Length));
            }
        }
        
        public static void ToggleFreezeMissionTime(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.FreezeMissionTime;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                        FreezeMissionTimeAoB,
                        FreezeMissionTimeOffset,
                        FreezeMissionTimeOffset.Length);
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
                BaseActions.ReplaceWithOriginalCode(activeGameCheat.CodeLocation, FreezeMissionTimeOffset,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException());
            }
        }
        
        public static void ToggleFreezeMissionStats(bool activate)
        {
            GameCheat activeGameCheat = PeaceWalkerCheat.FreezeMissionStats;
            if (activate)
            {
                if (activeGameCheat.CodeLocation == IntPtr.Zero)
                {
                    activeGameCheat.CodeLocation = BaseActions.ReplaceWithInvalidCode(
                        FreezeMissionStatsAoB,
                        FreezeMissionStatsOffset,
                        FreezeMissionStatsOffset.Length);
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
                BaseActions.ReplaceWithOriginalCode(activeGameCheat.CodeLocation, FreezeMissionStatsOffset,
                    activeGameCheat.OriginalBytes ?? throw new InvalidOperationException());
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
            OriginalCamo1Bytes, Constants.Cheat.MaxCamo);
        public static GameCheat MaxCamo2 { get; internal set; } = new(CheatActions.ToggleMaxCamoSub2,
            OriginalCamo2Bytes, Constants.Cheat.MaxCamo);
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

        public static readonly List<GameCheat> CheatList =
        [
            UnlimitedLife, Invulnerable, NoReload,UnlimitedAmmo, InfiniteSuppressor, UnlimitedPsyche,
            FreezeAi, InvisibleToAi, MaxCamo, UnlimitedEquipment, NoTimeLimit, MaxStockOnPickup,
            FreezeMissionTime, FreezeMissionStats
        ];
    }
}