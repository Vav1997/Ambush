using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MineCollisionDetection : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(MineController.instance.StartPointActivated() && !MineController.instance.isMineSolved())
        {
            MineController.instance.OnEdgeCollided();
        }
    }
}
