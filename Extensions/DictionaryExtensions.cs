using System.Collections.Generic;
using System.Text.Json;

namespace Dorbit.Framework.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
    {
        if (dict != null && dict.TryGetValue(key, out var value)) return value;
        return default;
    }
    
    public static int GetInt32OrDefault<TKey>(this Dictionary<TKey, JsonElement> dict, TKey key)
    {
        if (dict != null && dict.TryGetValue(key, out var value)) return value.GetInt32();
        return 0;
    }
    
    public static double GetDoubleOrDefault<TKey>(this Dictionary<TKey, JsonElement> dict, TKey key)
    {
        if (dict != null && dict.TryGetValue(key, out var value)) return value.GetDouble();
        return 0;
    }
    
    public static string GetStringOrDefault<TKey>(this Dictionary<TKey, JsonElement> dict, TKey key)
    {
        if (dict != null && dict.TryGetValue(key, out var value)) return value.GetString();
        return null;
    }
    
}