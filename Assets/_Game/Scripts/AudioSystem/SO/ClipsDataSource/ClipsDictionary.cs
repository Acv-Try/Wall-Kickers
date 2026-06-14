using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;



public class BuildResult
{
    private List<string> warnings = new List<string>();
    public BuildResult(List<string> _warnings)
    {
        warnings = _warnings;
    }
    public void LogWarnings()
    {
        foreach (var warning in warnings)
        {
            Debug.LogWarning(warning);
        }
    }
}

public class ClipsDictionary<T>
    where T : Enum
{
    private readonly Dictionary<T, AudioClip[]> dictionary = new();
    
    public BuildResult BuildDictionary(IEnumerable<SoundEntry<T>> entries, string libraryName = "")
    {
        dictionary.Clear();
        var seen = new HashSet<T>();
        var warnings = new List<string>();
        
        foreach ( var entry in entries)
        {
            if (!seen.Add(entry.Type))
                warnings.Add($"[{libraryName}] Duplicate key '{entry.Type}' — overwriting.");

            if (entry.Clips == null || entry.Clips.Length == 0)
            {
                warnings.Add($"[{libraryName}] '{entry.Type}' has no clips — skipped.");
                continue;
            }

            for (int i = 0; i < entry.Clips.Length; i++)
            {
                if (entry.Clips[i] == null)
                    warnings.Add($"[{libraryName}] Null clip at index {i} in '{entry.Type}'.");
            }

            dictionary[entry.Type] = entry.Clips;
        }
        return new BuildResult(warnings);
    }

    public AudioClip GetClip(T type)
    {
        if(!dictionary.TryGetValue(type, out var clips))
        {
            Debug.LogError($"Audio clip not found! type - {type}");
            return null;    
        }
        return clips[Random.Range(0, clips.Length)];
    }
}

