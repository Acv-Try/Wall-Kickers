using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEditor;

[Serializable]
public struct SoundEntry<T>
{
    public T Type;
    public AudioClip[] Clips;
}
public abstract class ClipsLibrary<T> : ScriptableObject, ISoundProvider
    where T : Enum
{
    [SerializeField] public List<SoundEntry<T>> entries;

    public Type SoundType => typeof(T);

    private Dictionary<T, AudioClip[]> dictionary;

    private void OnEnable() => Build();
    private void Build()
    {
        dictionary = new Dictionary<T, AudioClip[]>();
        var seen = new HashSet<T>();

        foreach (var entry in entries)
        {
            if (!seen.Add(entry.Type))
                Debug.LogWarning($"[{name}] Duplicate key '{entry.Type}' — overwriting.");

            if (entry.Clips == null || entry.Clips.Length == 0)
            {
                Debug.LogWarning($"[{name}] '{entry.Type}' has no clips — skipped.");
                continue;
            }

            for (int i = 0; i < entry.Clips.Length; i++)
            {
                if (entry.Clips[i] == null)
                    Debug.LogWarning($"[{name}] Null clip at index {i} in '{entry.Type}'.");
            }

            dictionary[entry.Type] = entry.Clips;
        }
    }
    private AudioClip LookUpClip(T type)
    {
        if (!dictionary.TryGetValue(type, out var clips))
        {
            Debug.LogError($"[{name}] Clip not found for type '{type}'.");
            return null;
        }
        return clips[Random.Range(0, clips.Length)];
    }
    public AudioClip GetClip(Enum type)
    {
        if (type is not T sType)
        {
            Debug.LogWarning($"[{name}] Invalid sound type '{type}'.");
            return null;
        }
        
        return LookUpClip(sType);
    }
}
