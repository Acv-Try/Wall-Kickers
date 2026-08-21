using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SourceEntry<T>
{
    public T Type;
    public SoundData Data;
}

public abstract class SourceLibrary<T> : ScriptableObject
    where T : Enum
{
    [SerializeField] List<SourceEntry<T>> entries;

    public Type SourceType => typeof(T);
    private Dictionary<T, SoundData> dictionary = new();
    private void OnEnable() => Build();
    private void Build()
    {
        dictionary.Clear();
        var seen = new HashSet<T>();

        foreach (var entry in entries)
        {
            if (!seen.Add(entry.Type))
                Debug.LogWarning($"[{name}] Duplicate key '{entry.Type}' — overwriting.");

            if (entry.Data == null || entry.Data == null)
            {
                Debug.LogWarning($"[{name}] '{entry.Type}' has no source data — skipped.");
                continue;
            }

            dictionary[entry.Type] = entry.Data;
        }
    }
    public SoundData Get(T type)
    {
        if (!dictionary.TryGetValue(type, out var data))
        {
            Debug.LogError($"Source data for {type} not found.");
            return null;
        }
        SoundData a = data;
        return a;
    }
}
