using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePosTrigger : MonoBehaviour
{
    private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<FirstPersonController>() != null)
        {
            if(!isTriggered)
            {
                MissionManager.instance.GetActiveMission().OnDestinationReached();
                isTriggered = true;
            }
            
        }
    }
}
