using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinePointTrigger : MonoBehaviour
{
    public enum PointType {Start, End, WrongPoint}

    public PointType pointType;
    private bool isTriggered;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(pointType == PointType.Start && MineController.instance.StartPointActivated() == false)
        {
            MineController.instance.MineStartingPointActivated();
            Debug.Log("Start point activated");
        }
        else if(pointType == PointType.End && !isTriggered && MineController.instance.StartPointActivated() && !MineController.instance.IsDetonated())
        {
            isTriggered = true;
            MineController.instance.MineSolved();
        }
    }
}
