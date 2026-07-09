using System;
using UnityEngine;
public interface IPlayerStatus
{
    public event Action OnDeath;
    public event Action OnRespawn;
    public event Action<int, int> OnCheckpointReached;
    public event Action<Vector3> OnCameraCheckpointReached;
    public Vector3 SpawnPos { get; }
    public int DeathCount { get; }
    public int CheckPoint { get; }

    public int CheckPointCount { get; }
    public bool IsDead { get; }

    public void Initialize(Vector3 spawnPosition);
    public void TriggerCameraCheckpoint(Vector3 position);
    public void IncreaseCheckpoint();
}
public class PlayerStatus : MonoBehaviour, IPlayerStatus
{
    public bool IsDead { get; private set; }
    public int CheckPoint { get; private set; }
    public int CheckPointCount { get; private set; }
    public int DeathCount { get; private set; }
    public Vector3 SpawnPos { get; private set; }

    public event Action OnDeath;
    public event Action OnRespawn;
    public event Action<int, int> OnCheckpointReached;
    public event Action<Vector3> OnCameraCheckpointReached;
    public void Initialize(Vector3 spawnPosition)
    {
        SpawnPos = spawnPosition;
        IsDead = false;
        DeathCount = 0;
        Respawn();
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        DeathCount++;
        OnDeath?.Invoke();
    }

    public void Respawn()
    {
        IsDead = false;
        ResetStats();
        transform.position = SpawnPos;
        OnRespawn?.Invoke();
    }

    public void ResetStats()
    {
        CheckPoint = 0;
        CheckPointCount = 0;
    }

    public void IncreaseCheckpoint()
    {
        CheckPoint++;
        CheckPointCount++;
        if (CheckPointCount >= 10) CheckPointCount = 0;
        OnCheckpointReached?.Invoke(CheckPoint, CheckPointCount);
    }

    public void TriggerCameraCheckpoint(Vector3 position)
    {
        OnCameraCheckpointReached?.Invoke(position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dead"))
            Die();
    }
}