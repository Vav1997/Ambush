using System;
using System.Collections;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    [SerializeField]
    private bool detectSwipeOnlyAfterRelease = false;

    [SerializeField]
    private float minDistanceForSwipe = 20f;

    public  event EventHandler<SwipeData> OnSwipe;

    public  event EventHandler OnDoubleTap;
    public  event EventHandler OnFingerHold;

    private int tapCount;
    [SerializeField] private float fingerHoldTime;

    private Coroutine fingerHoldCor;

    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }
        }

        //Detect Double tap

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            tapCount += 1;
            StartCoroutine(Countdown());    
        }
       
        if (tapCount == 2)
        {    
            tapCount = 0;
            StopCoroutine(Countdown());
            
            OnDoubleTap?.Invoke(this, EventArgs.Empty);
        }

        // detect 3 second hold

        if(Input.touchCount > 0)
        {
            switch(Input.GetTouch(0).phase)
            {
                case TouchPhase.Began:
                    fingerHoldCor = StartCoroutine(FingerHoldCountdown());
                    break;
                case TouchPhase.Ended:
                    StopCoroutine(fingerHoldCor);
                    break;
            }
        }

    }

    private IEnumerator FingerHoldCountdown()
    {
        yield return new WaitForSeconds(fingerHoldTime);
        OnFingerHold?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator Countdown()
    { 
        yield return new WaitForSeconds(0.3f);
        tapCount = 0;  
    }

    private void DetectSwipe()
    {
        if (SwipeDistanceCheckMet())
        {
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                SendSwipe(direction);
            }
            else
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                SendSwipe(direction);
            }
            fingerUpPosition = fingerDownPosition;
        }
    }

    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    private bool SwipeDistanceCheckMet()
    {
        return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }

     private void SendSwipe(SwipeDirection direction)
     {

         OnSwipe?.Invoke(this, new SwipeData()
         {
            Direction = direction
         });
     }
}

    public class SwipeData : EventArgs
    {
        public SwipeDirection Direction;
    }

public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}