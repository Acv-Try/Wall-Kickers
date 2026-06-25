using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public bool IsDead { get; private set; }
    public int CheckPoint { get; private set; }
    public int CheckPointCount { get; private set; }
    public int DeathCount { get; private set; }
    public Vector3 SpawnPos { get; private set; }

    public event Action OnDied;
    public event Action OnRespawned;
    public event Action<int, int> OnCheckpointReached;
    public event Action<Vector3> OnCameraCheckpointReached;

    public void Initialize(Vector3 spawnPosition)
    {
        SpawnPos = spawnPosition;
        IsDead = false;
        DeathCount = 0;
        ResetStats();
        Respawn();
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        DeathCount++;
        //OnCameraShake?.Invoke();
        OnDied?.Invoke();
    }

    public void Respawn()
    {
        IsDead = false;
        transform.position = SpawnPos;
        OnRespawned?.Invoke();
    }

    public void ResetStats()
    {
        CheckPoint = 0;
        CheckPointCount = 0;
    }

    public void SetSpawnPos(Vector3 position)
    {
        SpawnPos = position;
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
        if (collision.CompareTag("Dead"))
            Die();
    }
}