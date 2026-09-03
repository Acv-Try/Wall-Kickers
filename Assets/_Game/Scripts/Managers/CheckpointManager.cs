using UnityEngine;
using System;
public class CheckpointManager : SingletonGame<CheckpointManager>
{
    #region Singleton
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    #endregion

    public event Action OnReplay;
    public void RaiseOnReplay() => OnReplay?.Invoke();

    public void ScoreCompute()
    {
        LevelManager.Instance.IncreaseCheckpoint();
        UIManager.Instance.SetScore(LevelManager.Instance.TotalCheckpoints.ToString());
    }

    public void CompleteSkippedCheckpoint(CheckpointWall wall)
    {
        if(wall == null) return;
        wall.CompleteViaSkip();
    }
}
