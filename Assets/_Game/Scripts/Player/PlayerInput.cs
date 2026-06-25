using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public event Action OnTouchBegan;
    public event Action OnTouchHeld;

    private void Update()
    {
        if (Input.touchCount == 0) return;

        var touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
            OnTouchBegan?.Invoke();

        if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            OnTouchHeld?.Invoke();

        if (Input.GetMouseButtonDown(0))
            OnTouchBegan?.Invoke();
        if (Input.GetMouseButton(0))
            OnTouchHeld?.Invoke();
    }
}
