using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
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

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
    private void Start()
    {
        playerController = gameObject.GetComponent<IPlayerController>();
    }
    private void Update()
    {
        if (GameManager.Instance.CurrentState == CurrentState.Paused) return;
        // For Testing
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (firstClick == true)
        //    {
        //        firstClick = false;
        //        //OnFirstTouch?.Invoke();
        //        PlayerEvents.RaiseOnFirstTouch();
        //    }
        //    //OnTouchBegan?.Invoke();
        //    playerController.HandleTouchBegan();
        //}
        //if (Input.GetMouseButton(0))
        //{
        //    playerController.HandleTouchHeld();
        //    //OnTouchHeld?.Invoke();
        //}
        //End
        if (Touch.activeTouches.Count == 0) return;

        var touch = Touch.activeTouches[0];

        if (EventSystem.current.IsPointerOverGameObject(touch.touchId)) return;

        if (touch.phase == TouchPhase.Began)
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

        if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            playerController.HandleTouchHeld();

    }
}