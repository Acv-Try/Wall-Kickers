using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SoundData soundData;
    private void Start()
    {
        soundData = AudioManager.Instance.GetSoundData(EType_SourceDataType.Gameplay);    
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            AudioManager.Instance.
            GetComponent<Collider>().GetComponent<PlayerController>().OnCameraCheckPointChange?.Invoke(transform.position);
        }
    }
}
