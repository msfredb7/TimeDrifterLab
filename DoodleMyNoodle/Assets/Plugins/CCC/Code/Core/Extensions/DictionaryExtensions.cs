﻿using System.Collections.Generic;

public static class DictionaryExtensions
{
    /// <summary>
    /// Returns the first key with the corresponding value
    /// </summary>
    public static TKey FindFirstKeyWithValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TValue value)
    {
        foreach (KeyValuePair<TKey, TValue> item in dict)
        {
            if(EqualityComparer<TValue>.Default.Equals(item.Value, value))
            {
                return item.Key;
            }
        }
        return default;
    }

    public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        where TValue : new()
    {
        if (!dict.TryGetValue(key, out TValue val))
        {
            val = new TValue();
            dict.Add(key, val);
        }

        return val;
    }

    public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
    {
        if (dict.TryGetValue(key, out TValue val))
        {
            return val;
        }
        else
        {
            return defaultValue;
        }
    }

    public static void SetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (dict.ContainsKey(key))
        {
            dict[key] = value;
        }
        else
        {
            dict.Add(key, value);
        }
    }
}