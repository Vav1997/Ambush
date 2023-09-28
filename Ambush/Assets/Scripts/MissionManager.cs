using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MissionManager : MonoBehaviour
{

    public static MissionManager instance;

    public List<Mission> MissionsList = new List<Mission>();

    public Mission ActiveMision;

   
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        
    }

    private void Start()
    {
        SetStartingMission();
    }



    public void SetStartingMission()
    {
        ActiveMision = MissionsList[0];
        ActiveMision.OnMissionStarted();
        
    }



    public void LoadNextMission()
    {
        ActiveMision.gameObject.SetActive(false);
        ActiveMision = MissionsList[MissionsList.IndexOf(ActiveMision) + 1];
        ActiveMision.gameObject.SetActive(true);
        ActiveMision.OnMissionStarted();
    }

    public Mission GetNextMission()
    {
        return MissionsList[MissionsList.IndexOf(ActiveMision) + 1]; 
    }

    public Mission GetActiveMission()
    {
        return ActiveMision;
    }


}
