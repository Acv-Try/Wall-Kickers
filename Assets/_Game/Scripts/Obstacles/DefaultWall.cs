using UnityEngine;

public class DefaultWall : BaseWall
{
    private void Start()
    {
        Initialize();
    }
    public DefaultWall()
    {
        
    }
    public override void Touched(PlayerController player)
    {
        AudioManager.Instance.Play(soundData, EType_Gameplay_SFX.Land_Wall_Wood);
        //PlayerManager.Instance.PlayLandAudio();
    }
}
