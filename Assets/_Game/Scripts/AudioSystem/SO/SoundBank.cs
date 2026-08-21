using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SoundBank", menuName = "Scriptable Objects/Audio/SoundBank")]
public class SoundBank : ScriptableObject
{
    public LibraryGameplaySounds LibraryGameplay;
    public LibraryMusic LibraryMusic;
    public LibraryUI LibraryUI;

    Dictionary<Type, ISoundProvider> providerMap = new();

    public void Initialize()
    {
        Build();
    }
    void Build()
    {
        providerMap.Clear();
        Register(LibraryGameplay);
        Register(LibraryMusic);
        Register(LibraryUI);
    }
    void Register(ISoundProvider provider)
    {
        providerMap[provider.SoundType] = provider;
    }
     
    public AudioClip GetClip<T>(T type)
    where T : Enum
    {
        var Type = typeof(T);
        if (!providerMap.TryGetValue(Type, out var provider))
        {
            Debug.LogError($"No sound provider registered for {Type}");
            return null;
        }

        return provider.GetClip(type);
    }

}
