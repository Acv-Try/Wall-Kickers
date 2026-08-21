using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static event Action OnGameStart;
    public static event Action OnPouseButtonClick;

    public static void RaiseOnGameStart()
    {
        OnGameStart?.Invoke();
    }

    public static void RaiseOnPouseButtonClick()
    {
        OnPouseButtonClick?.Invoke();
    }
}
