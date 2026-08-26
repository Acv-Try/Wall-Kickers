using UnityEngine;
using System.Collections;
using TMPro;
public class DisappearingWall : BaseWall
{
   public float TimeToRespawn = 4f;
   [SerializeField] private string crackAnimaName;
    [SerializeField] private string fallApartAnimaName;

  Collider2D col;
    SpriteRenderer sr;
    void Start()
    {
         col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        Initialize();
    }
    public override void Left(PlayerController player)
    {
        StartCoroutine(Timer());
    }

     public override void Touched(PlayerController player)
    {
        _animator.SetTrigger(crackAnimaName);
    }
 
    void ChangeState(bool state)
    {
        col.enabled = state;
        sr.enabled = state;
    }

    public void Destroy()
    {
        ChangeState(false);
    }
    IEnumerator Timer()
    {
        _animator.SetTrigger(fallApartAnimaName);
         col.enabled = false;
        yield return new WaitForSeconds(TimeToRespawn);
        _animator.SetTrigger("SetIdle");
        ChangeState(true);
    }
}