using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Objects/Audio/SoundData")]
public class SoundData : ScriptableObject
{
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;
    public bool loop;

    public bool frequentSound;
    public int maxInstances = 3;

    public bool mute;
    public bool bypassEffects;
    public bool bypassListenerEffects;
    public bool bypassReverbZones;

    [Range(0, 256)] public int priority = 128;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(-3f, 3f)] public float pitch = 1f;
    [Range(-1f, 1f)] public int panStereo;
    [Range(0f, 1f)] public float spatialBlend;
    [Range(0f, 1.1f)] public float reverbZoneMix = 1f;
    [Range(0f, 5f)] public float dopplerLevel = 1f;
    [Range(0f, 360f)] public float spread;

    public float minDistance = 1f;
    public float maxDistance = 500f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

    public bool ignoreListenerVolume;
    public bool ignoreListenerPause;
}
