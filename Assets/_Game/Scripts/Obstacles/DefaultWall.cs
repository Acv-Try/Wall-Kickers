using UnityEngine;

public class DefaultWall : BaseWall
{
    private void Start()
    {
        Initialize();
    }
    public override void Touched(PlayerController player)
    {
        base.Touched(player);
        PlayLandAudio(EType_Gameplay_SFX.Land_Wall_Wood);
        //PlayerManager.Instance.PlayLandAudio();
    }
    public override void Left(PlayerController player) { base.Left(player); }
}
