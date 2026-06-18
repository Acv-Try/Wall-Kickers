using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    private SoundData soundData;
    private void Start()
    {
        soundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.Gameplay);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Collect();
    }
    private void Collect()
    {
        EconomyManager.Instance.AddCoins(value, soundData);
        Destroy(this);
    }
}
