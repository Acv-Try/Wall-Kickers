using System.Collections;
using TMPro;
using UnityEngine;

public class ElectroWall :Wall
{

    public float TimeBetweenElectricStrike = 4f;
    public float TimeToStopElectro = 0.1f;
    SpriteRenderer sr;
    bool IsElectro;

    [SerializeField] TMP_Text TimerText;
    public override void Staying(Player player)
    {
        if(IsElectro)
        {
            player.Die();
        }
    }


    public void Start()
    {
        StartCoroutine(Timer());
        sr = GetComponent<SpriteRenderer>();
    }

    IEnumerator Timer()
    {
        while(true)
        {
        float Timer = TimeBetweenElectricStrike;
        TimerText.text = Timer.ToString();
        IsElectro = false;
            while(Timer > 0)
            {
                 yield return new WaitForSeconds(1);
                 Timer--;
                 TimerText.text = Timer.ToString();
            }
        IsElectro = true;
        sr.color = Color.blue;
        yield return new WaitForSeconds(TimeToStopElectro);
        sr.color = Color.white;
        }
    }
}
