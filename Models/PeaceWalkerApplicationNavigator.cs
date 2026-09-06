using System;

namespace MGSPW_MC_Cheat_Trainer.Models;

public class PeaceWalkerApplicationNavigator
{
    internal struct PeaceWalkerAoB
    {
        #region Cheats
        #region Unlimited HP
        internal static string UnlimitedLifeAoB = "0F BF AB BE 11 00 00 44 8B C0 2B E8 80 3D";
        internal static byte[] OriginalLifeBytes =
            [0x0F, 0xBF, 0xAB, 0xBE, 0x11, 0x00, 0x00, 0x44, 0x8B, 0xC0, 0x2B, 0xE8, 0x80, 0x3D];
        internal static MemoryOffset UnlimitedLifeOffset = new(0x0A, 0x0B);
        #endregion
        
        #region Invulnerable
        internal static string InvulnerableAoB = "32 D2 66 83 BF BE 11 00 00 00 4C 8B E8";
        internal static byte[] OriginalVulnerableBytes =
            [0x32, 0xD2, 0x66, 0x83, 0xBF, 0xBE, 0x11, 0x00, 0x00, 0x00, 0x4C, 0x8B, 0xE8];
        internal static byte[] InvulnerableBytes = [0x39, 0xC0, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90];
        internal static MemoryOffset InvulnerableOffset = new(0x02, 0x09);
        #endregion
        
        #region No Reload
        internal static string NoReloadAoB = "EB 62 44 28 73 05 B9 00 00 00 00";
        internal static byte[] OriginalReloadBytes = [0xEB, 0x62, 0x44, 0x28, 0x73, 0x05, 0xB9, 0x00, 0x00, 0x00, 0x00];
        internal static MemoryOffset NoReloadOffset = new(0x02, 0x05);
        #endregion
        
        #region Unlimited Ammo
        internal static string UnlimitedAmmoAoB = "75 08 66 41 2B C6 66 89 43 0A";
        internal static byte[] OriginalAmmoBytes = [0x75, 0x08, 0x66, 0x41, 0x2B, 0xC6, 0x66, 0x89, 0x43, 0x0A];
        internal static MemoryOffset UnlimitedAmmoOffset = new(0x02, 0x05);
        #endregion
        
        #region Infinite Suppressor
        internal static string InfiniteSuppressorAoB = "66 44 29 73 08 0F B7 43 08 66 85 C0";
        internal static byte[] OriginalSuppressorBytes =
            [0x66, 0x44, 0x29, 0x73, 0x08, 0x0F, 0xB7, 0x43, 0x08, 0x66, 0x85, 0xC0];
        internal static MemoryOffset InfiniteSuppressorOffset = new(0x00, 0x04);
        #endregion
        
        #region Unlimited Psyche
        internal static string UnlimitedPsycheAoB = "8B 05 DF 04 2A 01 2B C2 33 D2";
        internal static byte[] OriginalPsycheBytes = [0x8B, 0x05, 0xDF, 0x04, 0x2A, 0x01, 0x2B, 0xC2, 0x33, 0xD2];
        internal static MemoryOffset UnlimitedPsycheOffset = new(0x06, 0x07);
        #endregion
        
        #region Freeze AI
        internal static string FreezeAiAoB = "48 89 45 37 80 B9 9C 41 00 00 00 48 8B D9 0F 84";
        internal static byte[] OriginalAiBytes =
            [0x48, 0x89, 0x45, 0x37, 0x80, 0xB9, 0x9C, 0x41, 0x00, 0x00, 0x00, 0x48, 0x8B, 0xD9, 0x0F, 0x84];
        internal static byte[] FreezeAiBytes = [0x48, 0x31, 0xC0, 0x90, 0x90, 0x90, 0x90];
        internal static MemoryOffset FreezeAiOffset = new(0x04, 0x0A);
        #endregion
        
        #region Invisible to AI
        internal static string InvisibleToAiAoB =
            "48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 48 89 7C 24 20 41 54 41 56 41 57 48 83 EC 30 49 8B F1 49 8B F8";
        internal static byte[] OriginalVisibleToAiBytes =
        [
            0x48, 0x89, 0x5C, 0x24, 0x08, 0x48, 0x89, 0x6C, 0x24, 0x10, 0x48, 0x89, 0x74, 0x24, 0x18, 0x48, 0x89, 0x7C,
            0x24, 0x20, 0x41, 0x54, 0x41, 0x56, 0x41, 0x57, 0x48, 0x83, 0xEC, 0x30, 0x49, 0x8B, 0xF1, 0x49, 0x8B, 0xF8
        ];
        internal static byte[] InvisibleToAiBytes = [0x31, 0xC0, 0xC3];
        internal static MemoryOffset InvisibleToAiOffset = new(0x00, 0x02);
        #endregion
        
        #region Max Camo
        internal static string MaxCamo1AoB = "B8 18 FC FF FF 41 BE B6 03 00 00 41 3B DE 41 8B EE 48 8B CF 0F 4C EB";
        internal static byte[] OriginalCamo1Bytes =
        [
            0xB8, 0x18, 0xFC, 0xFF, 0xFF, 0x41, 0xBE, 0xB6, 0x03, 0x00, 0x00, 0x41, 0x3B, 0xDE, 0x41, 0x8B, 0xEE, 0x48,
            0x8B, 0xCF, 0x0F, 0x4C, 0xEB
        ];
        internal static MemoryOffset MaxCamo1Offset = new(0x14, 0x16);

        internal static string MaxCamo2AoB = "BD E8 03 00 00 48 85 08 41 0F 45 EE";
        internal static byte[] OriginalCamo2Bytes =
            [0xBD, 0xE8, 0x03, 0x00, 0x00, 0x48, 0x85, 0x08, 0x41, 0x0F, 0x45, 0xEE];
        internal static MemoryOffset MaxCamo2Offset = new(0x08, 0x0B);
        #endregion
        
        #region Unlimited Equipment
        internal static string UnlimitedEquipmentAoB = "66 41 3B F6 66 41 0F 4F F6 66 44 2B F6 66 44 89 77 0A";
        internal static byte[] OriginalEquipmentBytes =
        [
            0x66, 0x41, 0x3B, 0xF6, 0x66, 0x41, 0x0F, 0x4F, 0xF6, 0x66, 0x44, 0x2B, 0xF6, 0x66, 0x44, 0x89, 0x77, 0x0A
        ];
        internal static MemoryOffset UnlimitedEquipmentOffset = new(0x0D, 0x11);
        #endregion
        
        #region No Time Limit
        internal static string NoTimeLimitAoB = "40 53 48 83 EC 20 44 8B 81 90 00 00 00 48 8B D9 45 85 C0";
        internal static byte[] OriginalTimeLimitBytes =
        [
            0x40, 0x53, 0x48, 0x83, 0xEC, 0x20, 0x44, 0x8B, 0x81, 0x90, 0x00, 0x00, 0x00, 0x48, 0x8B, 0xD9, 0x45, 0x85,
            0xC0
        ];
        internal static byte[] NoTimeLimitBytes = [0x45, 0x31, 0xC0, 0x90, 0x90, 0x90, 0x90];
        internal static MemoryOffset NoTimeLimitOffset = new(0x06, 0x0C);
        #endregion
        
        #region Max Stock on Pickup
        internal static string MaxStockOnPickupAoB = "66 44 03 C6 66 2B EE 66 44 89 43 0A";
        internal static byte[] OriginalStockOnPickupBytes =
            [0x66, 0x44, 0x03, 0xC6, 0x66, 0x2B, 0xEE, 0x66, 0x44, 0x89, 0x43, 0x0A];
        internal static byte[] MaxStockOnPickupBytes = [0x66, 0x89, 0x43, 0x0A, 0x90];
        internal static MemoryOffset MaxStockOnPickupOffset = new(0x07, 0x0B);
        #endregion

        #region Freeze Mission Time
        internal static string FreezeMissionTimeAoB = "48 89 05 AC 5D 02 01 41 0F BA E1 19";
        internal static byte[] OriginalMissionTimeBytes =
            [0x48, 0x89, 0x05, 0xAC, 0x5D, 0x02, 0x01, 0x41, 0x0F, 0xBA, 0xE1, 0x19];
        internal static MemoryOffset FreezeMissionTimeOffset = new(0x00, 0x06);
        #endregion
        
        #region Freeze Mission Stats
        internal static string FreezeMissionStatsAoB = "49 8B C7 01 1C 81 48 8D 0C 85";
        internal static byte[] OriginalMissionStatsBytes = [0x49, 0x8B, 0xC7, 0x01, 0x1C, 0x81, 0x48, 0x8D, 0x0C, 0x85];
        internal static MemoryOffset FreezeMissionStatsOffset = new(0x03, 0x05);
        #endregion

        #region Force Vehicle Commander
        internal static string VehicleBossAoB =
            "0F 48 C3 89 1D ?? ?? ?? ?? 33 C9 89 05 ?? ?? ?? ?? 89 1D ?? ?? ?? ?? E8";
        internal static MemoryOffset VehicleBossOffset = new(0x05, 0x09);
        internal static MemoryOffset EscortCountOffset = new(0x09, 0xD);
        internal static byte[] ForceVehicleCommanderBytes = [0xFF, 0x00, 0x00, 0x00];
        #endregion

        #region No Clip
        internal static string HoldYPositionAoB = "41 0F 28 C0 F3 0F 11 53 24 F3 0F 5C 43 20";
        internal static byte[] OriginalYPositionBytes =
            [0x41, 0x0F, 0x28, 0xC0, 0xF3, 0x0F, 0x11, 0x53, 0x24, 0xF3, 0x0F, 0x5C, 0x43, 0x20];
        internal static MemoryOffset HoldYPositionOffset = new(0x04, 0x08);

        internal static string NoClipAoB = "0F BA E0 14 73 38 0F BA E0 0B 73 08";
        internal static byte[] OriginalClipBytes =
            [0x0F, 0xBA, 0xE0, 0x14, 0x73, 0x38, 0x0F, 0xBA, 0xE0, 0x0B, 0x73, 0x08];
        internal static MemoryOffset NoClipOffset = new(0x04, 05);
        internal static byte[] NoClipBytes = [0xEB, 0x38];
        #endregion
        #endregion
        
        #region Weapons, Items & Other R&D
        public static nint WeaponsPtrLocation = 0x014CC310;
        public static int WeaponsPtrOffset = 0x04;
        #endregion

        public static nint StagePtrLocation = 0x00EA4860;
        public static int StagePtrOffset = 0x54;

        public static nint MaxCoopPlayerCountLocation = 0x00FFA038;
        public static int[] MaxCoopPlayerCountOffsets = [0x28, 0x65];
    }
}