using System.Collections.Generic;

namespace MGSPW_MC_Cheat_Trainer.Models;

public static class Constants
{
    public const int MillisecondsInSecond = 1000;
    
    public interface IPwObject
    {
        public string Name { get; set; }
        public string Shorthand { get; set; }
    }

    public enum Cheat
    {
        UnlimitedLife, Invulnerable, NoReload, UnlimitedAmmo, InfiniteSuppressor, UnlimitedPsyche, FreezeAi,
        InvisibleToAi, MaxCamo, UnlimitedEquipment, NoTimeLimit, MaxStockOnPickup, FreezeMissionTime, FreezeMissionStats,
        ForceVehicleCommander, NoClip
    }
    
    public class Weapon(string name, string shorthand) : IPwObject
    {
        public string Name { get; set; } = name;
        public string Shorthand { get; set; } = shorthand;
    }

    public static List<Weapon> WeaponsList =
    [
        new Weapon("Mk. 22", "mk22"),
        new Weapon("EZ Gun Life Recovery", "ezgunlr"),
        new Weapon("EZ Gun Psyche Recovery", "ezgunpr"),
        new Weapon("C96", "c96"),
        new Weapon("K. Pistol", "kpistol"),
        new Weapon("M19", "m19"),
        new Weapon("M1911A1", "m1911a1"),
        new Weapon("M1911A1 Custom", "m1911a1custom"),
        new Weapon("PB/6P9","pb6p9"),
        new Weapon("PM", "pm"),
        new Weapon("Supply Marker", "supplymarker"),
        new Weapon("Strike Marker", "strikemarker"),
        new Weapon("CAW","caw"),
        new Weapon("M37","m37"),
        new Weapon("M37 (ACM)","m37acm"),
        new Weapon("SPAS-12","spas12"),
        new Weapon("Twin Barrel","twinbarrel"),
        new Weapon("Twin Barrel (Rubber Slug)","twinbarrelrubber"),
        new Weapon("ADM63","adm63"),
        new Weapon("ADM65","adm65"),
        new Weapon("G11","g11"),
        new Weapon("FAL","fal"),
        new Weapon("M16A1","m16a1"),
        new Weapon("M16A1 (Shotgun)","m16a1shotgun"),
        new Weapon("M16A1 (Grenade Launcher)","m16a1grenade"),
        new Weapon("M16A1 (Smoke Grenade Launcher)","m16a1smoke"),
        new Weapon("M653","m653"),
        new Weapon("M653 (Grenade Launcher)","m653grenade"),
        new Weapon("M653 (Shotgun)","m653shotgun"),
        new Weapon("M653 (Smoke Grenade Launcher","m653smoke"),
        new Weapon("RK-47","rk47"),
        new Weapon("RK-47 (Grenade Launcher)","rk47grenade"),
        new Weapon("RK-47 (Smoke Grenade Launcher)","rk47smoke"),
        new Weapon("RPK","rpk"),
        new Weapon("SUG","sug"),
        new Weapon("Tanegashima","tanegashima"),
        new Weapon("Patriot","patriot"),
        new Weapon("MAC-10","mac10"),
        new Weapon("MAC-10 (Barrel Jacket)","mac10barreljacket"),
        new Weapon("MP5A2","mp5a2"),
        new Weapon("MP5SD2","mp5sd2"),
        new Weapon("M1928A1","m1928a1"),
        new Weapon("UZ61","uz61"),
        new Weapon("Mosin Nagant","mosinnagant"),
        new Weapon("M1C","m1c"),
        new Weapon("M1C (Psyche Recovery)", "m1cpsyche"),
        new Weapon("M21","m21"),
        new Weapon("M700","m700"),
        new Weapon("M700 (Life Recovery)", "m700life"),
        new Weapon("PTRD1941","ptrd1941"),
        new Weapon("PTRS1941","ptrs1941"),
        new Weapon("Rail Gun","railgun"),
        new Weapon("Rail Gun Dynamo","railgundynamo"),
        new Weapon("SVD","svd"),
        new Weapon("SVD (NV)","svdnv"),
        new Weapon("Stealth Gun","stealthgun"),
        new Weapon("WA2000","wa2000"),
        new Weapon("M60","m60"),
        new Weapon("M63A1","m63a1"),
        new Weapon("PKM","pkm"),
        new Weapon("MG3","mg3"),
        new Weapon("EMW Gun","emwgun"),
        new Weapon("M134","m134"),
        new Weapon("LAW","law"),
        new Weapon("M202A1","m202a1"),
        new Weapon("M47","m47"),
        new Weapon("RPG-2","rpg2"),
        new Weapon("RPG-7","rpg7"),
        new Weapon("Carl Gustav","carlgustav"),
        new Weapon("Carl Gustav (Multipurpose)","carlgustavmulti"),
        new Weapon("Card Gustav (Fulton Recovery)","carlgustavfulton"),
        new Weapon("FIM-43","fim43"),
        new Weapon("XFIM-92A","xfim9a2"),
        new Weapon("Sling Post","slingpost"),
        new Weapon("Sling Band","slingband"),
        new Weapon("Chaff Grenade","chaff"),
        new Weapon("Empty Magazines","emptymag"),
        new Weapon("EM Net","emnet"),
        new Weapon("Fragmentation Grenade","fraggrenade"),
        new Weapon("Sleep Gas Grenade","sleepgasgrenade"),
        new Weapon("Smoke Grenade","smokegrenade"),
        new Weapon("Colored Smoke Grenade","coloredsmokegrenade"),
        new Weapon("Stun Grenade","stungrenade"),
        new Weapon("Support Supply Marker (Thrown)","supportsupplymarker"),
        new Weapon("Support Strike Marker (Thrown)","supportstrikemarker"),
        new Weapon("Anti-Tank Mine","antitankmine"),
        new Weapon("Aerial Mine","aerialmine"),
        new Weapon("C4","c4"),
        new Weapon("Claymore","claymore"),
        new Weapon("Decoy","decoy"),
        new Weapon("Fulton Mine","fultonmine"),
        new Weapon("MGS Magazine","mgsbook"),
        new Weapon("Solid Magazine","solidbook"),
        new Weapon("Liquid Magazine","liquidbook"),
        new Weapon("Solidus Magazine","solidusbook"),
        new Weapon("Super Magazine","superbook"),
        new Weapon("Cookbook","cookbook"),
        new Weapon("Support Supply Marker (Placed)","placedsupportsupplymarker"),
        new Weapon("Support Strike Marker (Placed)","placedsupportstrikemarker"),
    ];
}