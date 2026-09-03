using System;
using UnityEngine;

public static class UIEvents
{
    //UI
    public static event Action OnGameLaunch;
    public static event Action OnGameLose;
    public static event Action OnContinue;
    public static event Action OnRestart;
    public static event Action OnPause;
    //UI
    public static void RaiseOnGameLaunch() => OnGameLaunch?.Invoke();
    public static void RaiseOnGameLose() => OnGameLose?.Invoke();
    public static void RaiseOnContinue() => OnContinue?.Invoke();
    public static void RaiseOnRestart() => OnRestart?.Invoke(); 
    public static void RaiseOnPause() => OnPause?.Invoke(); 
}
