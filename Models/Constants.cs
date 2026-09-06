using System.Collections.Generic;

namespace MGSPW_MC_Cheat_Trainer.Models;

public static class Constants
{
    public const int MillisecondsInSecond = 1000;
    
    public interface IPwObject
    {
        public string Name { get; set; }
        public string Shorthand { get; set; }
        public int Index { get; set; }
    }

    public enum Cheat
    {
        UnlimitedLife, Invulnerable, NoReload, UnlimitedAmmo, InfiniteSuppressor, UnlimitedPsyche, FreezeAi,
        InvisibleToAi, MaxCamo, UnlimitedEquipment, NoTimeLimit, MaxStockOnPickup, FreezeMissionTime, FreezeMissionStats,
        ForceVehicleCommander, NoClip
    }

    public enum WeaponMemory
    {
        Id = 0x0, //uint
        Research = 0x4, //uint
        Development = 0x8, //uint
        Stock = 0xC, //uint
        CurrentUsage = 0x14, //short
        UsageLevel = 0x16 //byte
    }
    
    public class Weapon(string name, string shorthand, int index, int[]? upgradeIndices = null) : IPwObject
    {
        public string Name { get; set; } = name;
        public string Shorthand { get; set; } = shorthand;
        public int Index { get; set; } = index;
        public int[]? UpgradeIndices { get; set; } = upgradeIndices;
    }

    public static readonly List<Weapon> WeaponsList =
    [
        new Weapon("Mk.22", "mk22", 0x02, [0x03, 0x04, 0x05, 0x06]),
        new Weapon("EZ Gun(LR)", "ezgunlr", 0x07, [0x08, 0x09, 0x0A]),
        new Weapon("EZ Gun(PR)", "ezgunpr", 0xB, [0x0C, 0x0D, 0x0E]),
        new Weapon("C96", "c96", 0x17, [0x18, 0x19]),
        new Weapon("K.Pistol", "kpistol", 0x1D, [0x1E]),
        new Weapon("M19", "m19", 0x1A, [0x1B,0x1C]),
        new Weapon("M1911A1", "m1911a1", 0x12, [0x13]),
        new Weapon("M1911A1(CT)", "m1911a1custom", 0x14, [0x15,0x16]),
        new Weapon("PB/6P9","pb6p9", 0x10, [0x11]),
        new Weapon("PM", "pm", 0x0F),
        new Weapon("Supply Mk.", "supplymarker", 0x1F, [0x20,0x21]),
        new Weapon("Strike Mk.", "strikemarker", 0x22, [0x23,0x24]),
        new Weapon("Banana", "banana", 0x25),
        new Weapon("CAW","caw", 0x37),
        new Weapon("M37","m37", 0x2E, [0x2F, 0x30, 0x31]),
        new Weapon("M37(LB/ACM)","m37acm", 0x32, [0x33]),
        new Weapon("SPAS-12","spas12", 0x34, [0x35, 0x36]),
        new Weapon("Twin Barrel","twinbarrel", 0x26, [0x27, 0x28, 0x29, 0x2A]),
        new Weapon("Twin Barrel(RB)","twinbarrelrubber", 0x2B, [0x2C, 0x2D]),
        new Weapon("ADM63","adm63", 0x49),
        new Weapon("ADM65","adm65", 0x4A, [0x4B]),
        new Weapon("G11","g11", 0x53),
        new Weapon("FAL","fal", 0x4F, [0x50, 0x51]),
        new Weapon("M16A1","m16a1", 0x38, [0x39,0x3A, 0x3B, 0x3C]),
        //new Weapon("M16A1 (Shotgun)","m16a1shotgun", ),
        new Weapon("M16A1(GL)","m16a1grenade", 0x3E, [0x3F]),
        new Weapon("M16A1(SGL)","m16a1smoke", 0x40, [0x41]),
        new Weapon("M653","m653", 0x42, [0x43, 0x44]),
        //new Weapon("M653 (Grenade Launcher)","m653grenade"),
        new Weapon("M653(STG)","m653shotgun", 0x45),
        new Weapon("M653(SGL)","m653smoke", 0x46),
        new Weapon("RK47","rk47", 0x47, [0x48]),
        //new Weapon("RK-47 (Grenade Launcher)","rk47grenade"),
        new Weapon("RK47(SGL)","rk47smoke", 0x4C),
        new Weapon("RPK","rpk", 0x4D, [0x4E]),
        new Weapon("SUG","sug", 0x52),
        new Weapon("Musket","tanegashima", 0x54),
        new Weapon("Patriot","patriot", 0x3D),
        new Weapon("M10","m10", 0x55, [0x56]),
        new Weapon("M10(BJ)","m10barreljacket", 0x57),
        new Weapon("MP5A2","mp5a2", 0x5B, [0x5C]),
        new Weapon("MP5SD2","mp5sd2", 0x5D),
        new Weapon("M1928A1","m1928a1", 0x5E, [0x5F, 0x60]),
        new Weapon("Uz61","uz61", 0x58, [0x59, 0x5A]),
        new Weapon("Mosin-Nagant","mosinnagant", 0x6D, [0x6E, 0x6F, 0x70, 0x71]),
        new Weapon("M1C","m1c", 0x61, [0x62]),
        new Weapon("M1C(Psyche)", "m1cpsyche", 0x63, [0x64]),
        new Weapon("M21","m21", 0x65, [0x66, 0x67]),
        new Weapon("M700","m700", 0x68, [0x69, 0x6A]),
        new Weapon("M700(Life Rec.)", "m700life", 0x6B, [0x6C]),
        new Weapon("PTRD1941","ptrd1941", 0x79, [0x7A]),
        new Weapon("PTRS1941","ptrs1941", 0x7B),
        new Weapon("Railgun","railgun", 0x7F, [0x80, 0x81]),
        new Weapon("Railgun Dynamo","railgundynamo", 0x82),
        new Weapon("SVD","svd", 0x72, [0x73, 0x74]),
        new Weapon("SVD(NV)","svdnv", 0x75),
        new Weapon("Stealth Gun","stealthgun", 0x7C, [0x7D, 0x7E]),
        new Weapon("WA2000","wa2000", 0x76, [0x77, 0x78]),
        new Weapon("M60","m60", 0x83),
        new Weapon("M60(AP/SB)", "m60armor", 0x84, [0x85, 0x86]),
        new Weapon("M63A1","m63a1", 0x87, [0x88, 0x89]),
        new Weapon("PKM","pkm", 0x8A, [0x8B, 0x8C, 0x8D, 0x8E]),
        new Weapon("MG3","mg3", 0x8F, [0x90, 0x91, 0x92]),
        new Weapon("EM Wave Gun","emwgun", 0x94),
        new Weapon("M134","m134", 0x93),
        new Weapon("LAW","law", 0x95, [0x96, 0x97, 0x98]),
        new Weapon("M202A1","m202a1", 0x99, [0x9A, 0x9B]),
        new Weapon("M47","m47", 0xAD),
        new Weapon("RPG2","rpg2", 0x9C),
        new Weapon("RPG7","rpg7", 0x9D, [0x9E, 0x9F]),
        new Weapon("C. Gustav","carlgustav", 0xA3, [0xA4, 0xA5, 0xA6]),
        new Weapon("C. Gustav(MP)","carlgustavmulti", 0xA7, [0xA8, 0xA9]),
        new Weapon("C. Gustav(FR)","carlgustavfulton", 0xAA, [0xAB, 0xAC]),
        new Weapon("FIM-43","fim43", 0xA0),
        new Weapon("XFIM-92A","xfim9a2", 0xA1, [0xA2]),
        new Weapon("Sling Post","slingpost", 0xAE),
        new Weapon("Sling Band","slingband", 0xAF, [0xB0, 0xB1]),
        new Weapon("Chaff Grenade","chaff", 0xC1, [0xC2, 0xC3, 0xC4, 0xC5]),
        new Weapon("Empty Magazine","emptymag", 0xC6),
        new Weapon("EM Net","emnet", 0xCE),
        new Weapon("Grenade","fraggrenade", 0xB2, [0xB3, 0xB4, 0xB5, 0xB6]),
        new Weapon("Sleep Gas Grenade","sleepgasgrenade", 0xCF, [0xD0, 0xD1, 0xD2, 0xD3]),
        new Weapon("Smoke Grenade","smokegrenade", 0xBC, [0xBD, 0xBE, 0xBF, 0xC0]),
        new Weapon("Smoke Grenade(Colored)","coloredsmokegrenade", 0xC7, [0xC8, 0xC9, 0xCA, 0xCB]),
        new Weapon("Stun Grenade","stungrenade", 0xB7, [0xB8, 0xB9, 0xBA, 0xBB]),
        new Weapon("Supply Mk.(thrown)","supportsupplymarker", 0xCC),
        new Weapon("Strike Mk.(thrown)","supportstrikemarker", 0xCD),
        new Weapon("AT Mine","antitankmine", 0xE7, [0xE8, 0xE9, 0xEA, 0xEB]),
        new Weapon("Aerial Mine","aerialmine", 0xF1, [0xF2, 0xF3, 0xF4, 0xF5]),
        new Weapon("C4","c4", 0xD4, [0xD5, 0xD6, 0xD7, 0xD8]),
        new Weapon("Claymore","claymore", 0xD9, [0xDA, 0xDB, 0xDC, 0xDD]),
        new Weapon("Decoy","decoy", 0xEC, [0xED, 0xEE, 0xEF, 0xF0]),
        new Weapon("Fulton Mine","fultonmine", 0xF6),
        new Weapon("Magazine","mgsbook", 0xDE, [0xDF, 0xE0, 0xE1, 0xE2]),
        new Weapon("Solid Mag.","solidbook", 0xE3),
        new Weapon("Liquid Mag.","liquidbook", 0xE4),
        new Weapon("Solidus Mag.","solidusbook", 0xE5),
        new Weapon("Super Mag.","superbook", 0xE6),
        new Weapon("Cookbook","cookbook", 0xF7, [0xF8, 0xF9, 0xFA, 0xFB]),
        new Weapon("Supply Mk.(placed)","placedsupportsupplymarker", 0xFC),
        new Weapon("Strike Mk.(placed)","placedsupportstrikemarker", 0xFD),
    ];
}