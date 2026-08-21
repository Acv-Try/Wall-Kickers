using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        public SoundData Data { get; private set; }
        public LinkedListNode<SoundEmitter> Node { get; set; }

        AudioSource audioSource;
        Coroutine playingCoroutine;
        SoundPool pool;

        public void Initialize(SoundPool pool)
        {
            this.pool = pool;
            audioSource = gameObject.GetOrAdd<AudioSource>();

        }

        public void Play(SoundData data)
        {
            Data = data;
            ApplyData(data);

            if(playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
            }
            audioSource.Play();
            if (!data.loop)
            {
                playingCoroutine = StartCoroutine(WaitForSoundToEnd());
            }
        }

        public void Stop() 
        {
            if (pool == null) return;

            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            audioSource.Stop();
            pool.Return(this);
            pool = null;
        }
        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            audioSource.pitch += Random.Range(min, max);
        }

        IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            Stop();
        }

        void ApplyData(SoundData data)
        {
            audioSource.clip = data.clip;
            audioSource.outputAudioMixerGroup = data.mixerGroup;
            audioSource.loop = data.loop;

            audioSource.mute = data.mute;
            audioSource.bypassEffects = data.bypassEffects;
            audioSource.bypassListenerEffects = data.bypassListenerEffects;
            audioSource.bypassReverbZones = data.bypassReverbZones;

            audioSource.priority = data.priority;
            audioSource.volume = Mathf.Clamp01(data.volume);
            audioSource.pitch = Mathf.Clamp(data.pitch, -3f, 3f);
            audioSource.panStereo = data.panStereo;
            audioSource.spatialBlend = data.spatialBlend;
            audioSource.reverbZoneMix = data.reverbZoneMix;
            audioSource.dopplerLevel = data.dopplerLevel;
            audioSource.spread = data.spread;

            audioSource.minDistance = data.minDistance;
            audioSource.maxDistance = Mathf.Max(data.maxDistance, data.minDistance);

            audioSource.ignoreListenerVolume = data.ignoreListenerVolume;
            audioSource.ignoreListenerPause = data.ignoreListenerPause;

            audioSource.rolloffMode = data.rolloffMode;
        }
    }
}
