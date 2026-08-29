using UnityEngine;

public class DefaultWall : BaseWall
{
    private void Start()
    {
        Initialize();
    }
    public override void Touched(PlayerController playe)
    {
        AudioManager.Instance.Play(soundData, EType_Gameplay_SFX.Land_Wall_Wood);
        //PlayerManager.Instance.PlayLandAudio();
    }
    public override void Left(PlayerController player) { }
}
