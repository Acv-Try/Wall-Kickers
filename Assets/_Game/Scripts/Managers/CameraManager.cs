using System.Collections;
using UnityEngine;
public enum CameraState
{
    Frozen,
    Following,
    Returning
}
public class CameraManager : MonoBehaviour
{
    [SerializeField] private CameraController controller;
    private Vector3 startCenter;
    private Vector3 initialCenter;
    private float initialLeftOffset;
    private float initialRightOffset;
    private Coroutine coroutine;

    public CameraState State { get; private set; } = CameraState.Frozen;
    #region Singleton
    private static CameraManager _instance;
    public static CameraManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<CameraManager>();
                if (_instance == null)
                {
                    Debug.LogWarning($"CameraManager is not found in the scene!");
                }
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion

    public void Initialize(Vector3 startCenter, float leftOffset, float rightOffset, Transform playerTransform)
    {
        CameraBox.Instance.SetPlayer(playerTransform);
        CameraBox.Instance.SetCenter(startCenter);
        CameraBox.Instance.SetOffsets(leftOffset, rightOffset);
        controller.SetInitial(startCenter);

        initialCenter = startCenter;
        initialLeftOffset = leftOffset;
        initialRightOffset = rightOffset;

        GameManager.Instance.OnPlayerDeath -= HandleDeath;
        GameManager.Instance.OnPlayerDeath += HandleDeath;
        GameManager.Instance.OnReplay -= HandleRespawn;
        GameManager.Instance.OnReplay += HandleRespawn;
    }
    public void SetState(CameraState newState) => State = newState;
    private void HandleDeath()
    {
        SetState(CameraState.Frozen);
        controller.HideDeadline();
        controller.Shake();
    }
    private void HandleRespawn(Transform player)
    {
        Debug.Log("respawn");
        CameraBox.Instance.SetPlayer(player);
        CameraBox.Instance.SetCenter(initialCenter);
        CameraBox.Instance.SetOffsets(initialLeftOffset, initialRightOffset);
        coroutine = StartCoroutine(RespawnSequence());
    }
    private IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(1.2f);
        controller.ResumeMovement();
        while (!controller.HasReachedTarget)
            yield return null;
        controller.ShowDeadline();
        coroutine = null;
    }
}