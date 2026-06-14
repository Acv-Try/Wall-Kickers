using System;
using UnityEngine;

public interface ISoundProvider
{
    Type SoundType { get; }
    public AudioClip GetClip(Enum type);
}
