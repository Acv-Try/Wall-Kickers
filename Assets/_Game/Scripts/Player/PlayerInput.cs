using System;
using UnityEngine;

public interface IPlayerInput
{
    public event Action OnTouchBegan;
    public event Action OnTouchHeld;
    public event Action OnFirstTouch;
}
public class PlayerInput : MonoBehaviour//, IPlayerInput
{
    bool firstClick = true;

    private IPlayerController playerController;
    private void Start()
    {
        playerController = gameObject.GetComponent<IPlayerController>();
    }
    private void Update()
    {
        if (GameManager.Instance.CurrentState == CurrentState.Paused) return;
        // For Testing
        if (Input.GetMouseButtonDown(0))
        {
            if (firstClick == true)
            {
                firstClick = false;
                //OnFirstTouch?.Invoke();
                PlayerEvents.RaiseOnFirstTouch();
            }
            //OnTouchBegan?.Invoke();
            playerController.HandleTouchBegan();
        }
        if (Input.GetMouseButton(0))
        {
            playerController.HandleTouchHeld();
            //OnTouchHeld?.Invoke();
        }
        //End

        //if (Input.touchCount == 0) return;

        //var touch = Input.GetTouch(0);

        //if (touch.phase == TouchPhase.Began)
        //    OnTouchBegan?.Invoke();

        //if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
        //    OnTouchHeld?.Invoke();

    }
}