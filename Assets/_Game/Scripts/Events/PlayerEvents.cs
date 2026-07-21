using System;
public static class PlayerEvents 
{
    public static event Action OnFirstTouch;

    public static void RaiseOnFirstTouch() => OnFirstTouch?.Invoke();
}
