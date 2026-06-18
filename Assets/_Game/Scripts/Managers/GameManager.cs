using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region
    private static GameManager instance;
    public static GameManager Instance => instance;

    private int currentCheckPoint;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion
    private void Start()
    {
        PlayerController.Instance.Initialize();
        CameraFollowing1.Instance.Initialize();
    }
    public void SetCheckPoint(int checkPoint)
    {
        currentCheckPoint = checkPoint;
    }

    public int GetCurrentCheckpoint()
    {
        return currentCheckPoint;
    }
}
