using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MGSPW_MC_Cheat_Trainer;

public static class AoBCacheManager
{
    private static readonly string AoBCacheLocation = Path.Combine(Environment.CurrentDirectory, "aobCache.json");

    private class Cache
    {
        public List<AoBInfo> CachedAoBInfos { get; set; } = new();
    }

    private class AoBInfo(string name, long location)
    {
        public string? Name { get; set; } = name;
        public long Location { get; set; } = location;
    }

    private static void InitializeCache()
    {
        File.WriteAllText(AoBCacheLocation, "{}");
    }

    private static Cache GetCache()
    {
        try
        {
            var cache = JsonSerializer.Deserialize<Cache>(File.ReadAllText(AoBCacheLocation));
            return cache ?? new Cache();
        }
        catch(FileNotFoundException)
        {
            InitializeCache();
            return new Cache();
        }
    }

    public static void SaveToCache(string key, nint value)
    {
        var cache = GetCache();
        var cachedItem = cache.CachedAoBInfos.FirstOrDefault(x => x.Name == key);
        if (cachedItem != null)
            cachedItem.Location = value;
        else
            cache.CachedAoBInfos.Add(new AoBInfo(key, value.ToInt64()));
        string serializedText = JsonSerializer.Serialize(cache);
        File.WriteAllText(AoBCacheLocation, serializedText);
    }

    public static void RemoveFromCache(string key)
    {
        var cache = GetCache();
        var cachedItem = cache.CachedAoBInfos.FirstOrDefault(x => x.Name == key);
        if (cachedItem == null) return;
        cache.CachedAoBInfos.Remove(cachedItem);
        File.WriteAllText(AoBCacheLocation,JsonSerializer.Serialize(cache));
    }

    public static long CheckCache(string key)
    {
        try
        {
            var cache = GetCache();

            return cache.CachedAoBInfos.Find(x => x.Name == key)?.Location ?? long.MinValue;
        }
        catch (Exception ex)
        {
            var baseMessage = "Failed to check AoB cache";
            LogManager.Logger?.Error($"{baseMessage}: {ex}");
            return long.MinValue;
        }
    }
}