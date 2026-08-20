using UnityEngine;
using AudioSystem;
using System;
using System.Collections.Generic;
public class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundBank soundBank;
    [SerializeField] private SourceBank sourceBank;
    private SoundPool pool;

    SoundEmitter _a_emitter;
    private Dictionary<Enum, SoundEmitter> activeEmitters;
    #region
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<AudioManager>();
                if (_instance == null)
                {
                    Debug.LogWarning($"Audio Manager is not found in the scene!");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion
    void Start()
    {
        soundBank.Initialize();
        pool = GetComponent<SoundPool>();
        activeEmitters = new Dictionary<Enum, SoundEmitter>();
    }
    public SoundData GetSoundData(EType_SourceDataType type)
    {
        if (sourceBank == null)
        {
            Debug.LogWarning($"{name} source bank is null.");
            return null;
        }
        return sourceBank.GetData(type);
    }

    public void Play<T>(SoundData data, T type) where T : Enum
    {
        if (data == null) return;
        data.clip = soundBank.GetClip(type);
        pool.CreateBuilder().WithPosition(transform.position).Play(data);
    }
    public void PlayAndTrack<T>(SoundData data, T type) where T : Enum
    {
        if (data == null) return;
        data.clip = soundBank.GetClip(type);
        _a_emitter = pool.CreateBuilder().WithPosition(transform.position).Play(data);
        activeEmitters[type] = _a_emitter;
    }
    public void Stop<T>(T type) where T : Enum
    {
        if (activeEmitters.TryGetValue((Enum)(object)type, out var emitter))
        {
            emitter?.Stop();
        }
    }
}

public enum EType_Gameplay_SFX
{
    C_Monkey_Jump,
    C_Monkey_JumpEffect,
    C_Monkey_Death,
    C_Monkey_Death_Explosion,
    Checkpoint_Reach,
    Checkpoint_Flag_Rise,// Still empty
    Coin_Collect,
    Land_Wall_Wood,
    Land_Wall_Crumble,
    Land_Wall_Bounce,
    Obstacle_Electrical,
}
public enum EType_UI_SFX
{
    Start_Button,
    Icons,
    PanelSlide,
    Audio_Button_On,
    Audio_Button_Off
}
public enum EType_Music
{
    Gameplay,
    CharacterAndMap_Choose_Menu
}
//BG is background
public enum EType_SourceDataType
{
    Music,
    Character,
    Gameplay,
    UI
}
