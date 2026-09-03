using UnityEngine;
using AudioSystem;
using System.Collections.Generic;

public class SoundPool : PersistentSingleton<SoundPool>
{
    [SerializeField] SoundEmitter soundEmitterPrefab;
    [SerializeField] int poolSize = 10;
    readonly Stack<SoundEmitter> available = new();
    readonly LinkedList<SoundEmitter> frequentActive = new();

    protected override void Awake()
    {
        base.Awake();
        InitializePool();
    }
    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            SoundEmitter emitter = Instantiate(soundEmitterPrefab, transform);
            emitter.Initialize(this);
            emitter.gameObject.SetActive(false);
            available.Push(emitter);
        }
    }
    public SoundEmitter Get()
    {
        if(available.Count == 0)
        {
            Debug.LogWarning("SoundPool: no emitters available. Consider increasing pool size.");
            return null;
        }

        SoundEmitter emitter = available.Pop();
        emitter.gameObject.SetActive(true);
        return emitter;
    }

    public void Return(SoundEmitter emitter)
    {
        if(emitter.Node != null)
        {
            frequentActive.Remove(emitter.Node);
            emitter.Node = null;
        }

        emitter.gameObject.SetActive(false);
        available.Push(emitter);
    }

    public bool CanPlaySound(SoundData data)
    {
        if (!data.frequentSound) return true;
        if (frequentActive.Count >= data.maxInstances)
        {
            frequentActive.First.Value.Stop();
        }
        return true;
    }

    public void TrackFrequent(SoundEmitter emitter)
    {
        emitter.Node = frequentActive.AddLast(emitter);
    }

    public void StopAll()
    {
        foreach (var emitter in new List<SoundEmitter>(frequentActive))
        {
            emitter.Stop();
        }
        foreach (var emitter in new Stack<SoundEmitter>(available))
        {
            emitter.gameObject.SetActive(false);
        }
    }
    public SoundBuilder CreateBuilder() => new SoundBuilder(this);  
}
