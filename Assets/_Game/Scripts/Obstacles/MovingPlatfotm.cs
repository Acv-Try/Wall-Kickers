public class MovingPlatform : BaseWall
{
    public override void Touched(PlayerController player)
    {
        base.Touched(player);
        player.transform.SetParent(transform);
    }

    public override void Left(PlayerController player)
    {
        base.Left(player);
        player.transform.SetParent(null);
    }
}