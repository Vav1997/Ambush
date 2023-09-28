using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Route { ToRight, ToTop, ToBot }

public class MineRouteTrigger : MonoBehaviour
{
    public Route route;
    private bool isTriggered;



    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!isTriggered && MineController.instance.StartPointActivated() && !MineController.instance.isMineSolved())
        {
            MineController.instance.OnRouteTriggerEntered(route);
            isTriggered = true;
        }
    }
}
