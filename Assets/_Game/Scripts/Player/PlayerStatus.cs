using System;
using UnityEngine;
public interface IPlayerStatus
{
    public Vector3 SpawnPos { get; }
    public int DeathCount { get; }
    public bool IsDead { get; }
    public event Action OnDeath;
    public event Action OnRespawn;
    public void Initialize(Vector3 spawnPosition);
}
public class PlayerStatus : MonoBehaviour, IPlayerStatus
{
    public bool IsDead { get; private set; }
    public int DeathCount { get; private set; }
    public Vector3 SpawnPos { get; private set; }

    public event Action OnDeath;
    public event Action OnRespawn;

    public void Initialize(Vector3 spawnPosition)
    {
        SpawnPos = spawnPosition;
        IsDead = false;
        DeathCount = 0;
        Respawn();
    }

    private void Die()
    {
        Debug.Log($"--1");
        if (IsDead) return;
        IsDead = true;
        DeathCount++;
        
        OnDeath?.Invoke();
        PlayerManager.Instance.RiseOnDeath();
    }

    public void Respawn()
    {
        Debug.Log($"--2");
        IsDead = false;
        transform.position = SpawnPos;
        
        OnRespawn?.Invoke();
        PlayerManager.Instance.RiseOnRespawn();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dead"))
            Die();
    }
}