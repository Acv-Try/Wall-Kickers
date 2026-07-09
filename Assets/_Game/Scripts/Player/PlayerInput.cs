using System;
using UnityEngine;

public interface IPlayerInput
{
    public event Action OnTouchBegan;
    public event Action OnTouchHeld;
    public event Action OnFirstTouch;
}
public class PlayerInput : MonoBehaviour, IPlayerInput
{
    public event Action OnTouchBegan;
    public event Action OnTouchHeld;
    public event Action OnFirstTouch;
    bool firstClick = true;
    private void Update()
    {
        // For Testing
        if (Input.GetMouseButtonDown(0))
        {
            if (firstClick == true)
            {
                firstClick = false;
                OnFirstTouch?.Invoke();
            }
            OnTouchBegan?.Invoke();
        }
        if (Input.GetMouseButton(0))
            OnTouchHeld?.Invoke();
        //End

        //if (Input.touchCount == 0) return;

        //var touch = Input.GetTouch(0);

        //if (touch.phase == TouchPhase.Began)
        //    OnTouchBegan?.Invoke();

        //if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
        //    OnTouchHeld?.Invoke();

    }
}