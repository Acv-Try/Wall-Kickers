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
}
