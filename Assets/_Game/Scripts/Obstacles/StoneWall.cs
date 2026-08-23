using UnityEngine;

public class StoneWall : BaseWall
{
    [SerializeField] private string crackAnimaName;
    [SerializeField] private string fallApartAnimaName;
    private void Start()
    {
        Initialize();
    }

    public override void Touched(PlayerController player)
    {
        _animator.SetTrigger(crackAnimaName);
    }

    public override void Left(PlayerController player)
    {
        _animator.SetTrigger(fallApartAnimaName);
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
