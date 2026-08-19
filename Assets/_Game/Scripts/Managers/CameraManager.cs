using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CameraController controller;
    private Vector3 startCenter;
    private Vector3 initialCenter;
    private Coroutine coroutine;
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

    public void Initialize(Vector3 startCenter, Transform playerTransform)
    {
        controller.SetInitial(startCenter, playerTransform);
        initialCenter = startCenter;

        GameManager.Instance.OnPlayerDeath -= HandleDeath;
        GameManager.Instance.OnPlayerDeath += HandleDeath;
        GameManager.Instance.OnReplay -= HandleRespawn;
        GameManager.Instance.OnReplay += HandleRespawn;
    }

    private void HandleDeath()
    {
        Debug.Log($"-1-");
        controller.Freeze();
        controller.HideDeadline();
        controller.Shake();
    }
    private void HandleRespawn(Transform pos)
    {
        Debug.Log($"-2-");
        controller.SetInitial(initialCenter, pos);
        coroutine = StartCoroutine(ReturnAndReveal());
    }
    private IEnumerator ReturnAndReveal()
    {
        yield return StartCoroutine(controller.ReturnToCenter());
        if (controller.State == CameraState.Following)
            controller.ShowDeadline();
        coroutine = null;
    }
}
