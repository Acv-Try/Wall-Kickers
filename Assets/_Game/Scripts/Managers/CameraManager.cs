using System.Collections;
using UnityEngine;
public enum CameraState
{
    Frozen,
    Following,
    Returning
}
public class CameraManager : SingletonGame<CameraManager>
{
    [SerializeField] private CameraController controller;
    private Vector3 startCenter;
    private Vector3 initialCenter;
    private float initialLeftOffset;
    private float initialRightOffset;
    private Coroutine coroutine;
    private float respawnWaitTime = 1.2f;
    public CameraState State { get; private set; } = CameraState.Frozen;
    #region Singleton Override
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
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
        Debug.Log("handle death called");
        SetState(CameraState.Frozen);
        controller.HideDeadline();
        controller.Shake();
        controller.PauseMovement();
    }
    private void HandleRespawn(Transform player)
    {
        Debug.Log("respawn");
        CameraBox.Instance.SetPlayer(player);
        CameraBox.Instance.SetCenter(initialCenter);
        CameraBox.Instance.SetOffsets(initialLeftOffset, initialRightOffset);
        controller.ResetLookAhead();
        coroutine = StartCoroutine(RespawnSequence());
    }
    private IEnumerator RespawnSequence()
    {
        //yield return new WaitForSeconds(respawnWaitTime);
        controller.StartReturning();
        controller.ResumeMovement();
        while (!controller.HasReachedTarget)
            yield return null;
        controller.ShowDeadline();
        coroutine = null;
    }
}