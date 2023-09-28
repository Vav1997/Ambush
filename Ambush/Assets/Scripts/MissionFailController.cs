using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionFailController : MonoBehaviour
{

    private int tapCount;


    private void Start()
    {
        Mission.OnMissionFail += Mission_OnMissionFail;
    }

    private void Mission_OnMissionFail(object sender, EventArgs e)
    {
        //StartCoroutine(CheckForDoubleTap());
    }


    private void Update()
    {
        //Detect Double tap

        
    }

    private IEnumerator Countdown()
    { 
        yield return new WaitForSeconds(0.3f);
        tapCount = 0;  
    }

    // private IEnumerator CheckForDoubleTap()
    // {
    //     while(true)
    //     {
    //         if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
    //         {
    //             tapCount += 1;
    //             StartCoroutine(Countdown());    
    //         }
        
    //         if (tapCount == 2)
    //         {    
    //             tapCount = 0;
    //             StopCoroutine(Countdown());
    //             SceneLoader.Instance.LoadLevel(SceneLoader.Scene.MainMenu);
    //         }
    //         yield return null;
    //     }
    // }
}
