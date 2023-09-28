using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMineDragController : MonoBehaviour
{
    public GameObject FinderDragObj;
    public event EventHandler<OnTouchPositionChangedEventArgs> OnTouchPositionChanged;
    public class OnTouchPositionChangedEventArgs : EventArgs
    {
        public Vector3 touchPos;
    }
    private int fingerId;

    // Update is called once per frame

    private void Start()
    {
        fingerId = -1;
    }
    void Update()
    {

        if(Input.touchCount > 0)
        {  
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    fingerId = touch.fingerId;
                    break;
                case TouchPhase.Moved:
                    Vector3 touchPosition = touch.position;
                    touchPosition.z = 0;
                    FinderDragObj.transform.position = touchPosition;

                    if(MineController.instance.StartPointActivated())
                    {
                        OnTouchPositionChanged?.Invoke(this, new OnTouchPositionChangedEventArgs()
                        {
                            touchPos = touchPosition
                        });
                    }
                    break;
                case TouchPhase.Ended:
                fingerId = -1;
                if(MineController.instance.StartPointActivated() && !MineController.instance.isMineSolved())
                {
                    MineController.instance.OnMineDetonate();
                }
                break;
            }
            
        }
    }
}
