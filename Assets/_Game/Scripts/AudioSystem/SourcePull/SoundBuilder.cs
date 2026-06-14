using UnityEngine;
namespace AudioSystem
{
    public class SoundBuilder
    {
        readonly SoundPool pool;
        Vector3 position = Vector3.zero;
        bool randomPitch;

        public SoundBuilder(SoundPool pool) {
        this.pool = pool;
        }
        public SoundBuilder WithPosition(Vector3 position) 
        {
            this.position = position;
            return this;
        }
        public SoundBuilder WithRandomPitch()
        {
            randomPitch = true;
            return this;
        }

        public SoundEmitter Play(SoundData data)
        {
            if (data == null)
            {
                Debug.LogError("SoundBuilder: SoundData is null.");
                return null;
            }

            if (!pool.CanPlaySound(data)) return null;

            SoundEmitter emitter = pool.Get();

            if (emitter == null) return null;

            emitter.transform.position = position;

            if (randomPitch)
            {
                emitter.WithRandomPitch();
            }

            if (data.frequentSound)
            {
                pool.TrackFrequent(emitter);
            }

            emitter.Play(data);

            return emitter;
        }
    }
}
