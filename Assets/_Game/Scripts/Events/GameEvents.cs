using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static event Action OnGameStart;
    public static event Action OnGameEnd;
    public static event Action OnPauseButtonClick;
    public static event Action OnPlayButtonClick;

    public static void RaiseOnGameStart()
    {
        OnGameStart?.Invoke();
    }

    public static void RaiseOnGameEnd()
    {
        OnGameEnd?.Invoke();
    }

    public static void RaiseOnPauseButtonClick()
    {
        OnPauseButtonClick?.Invoke();
    }

    public static void RaiseOnPlayButtonClick()
    {
        OnPlayButtonClick?.Invoke();
    }
}
