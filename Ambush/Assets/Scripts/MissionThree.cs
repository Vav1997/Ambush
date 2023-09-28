using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionThree : Mission
{
    private Route route;

    public enum MissionStage
    {
        MissionStart,
        ReachedMine,
        SolvingMine,
        MineSolved,
        MissionCompleted,
    }

    public MissionStage missionStage;
    public List<Dialog> MissionStartDialogList = new List<Dialog>();
    public List<Dialog> OnMineStartingPointActivatedDialogList = new List<Dialog>();
    public List<Dialog> OnMineEdgeCollidedDialogList = new List<Dialog>();
    public List<Dialog> OnMineDetonatedDialogList = new List<Dialog>();
    public List<Dialog> OnMineRouteToRightTriggerDialogList = new List<Dialog>();
    public List<Dialog> OnMineRouteToTopTriggerDialogList = new List<Dialog>();
    public List<Dialog> OnMineRouteToBotTriggerDialogList = new List<Dialog>();
    public List<Dialog> OnMineSolvedDialogList = new List<Dialog>();
    public List<Dialog> OnMineDestinationReachedDialogList = new List<Dialog>(); 



    [SerializeField] private SingleMusic onMineStartedMusic;
    


    private void Start()
    {
        
        if(MissionManager.instance.GetActiveMission() != this)
        {
            gameObject.SetActive(false);
        }
    }

    public override void OnMineDestinationReached()
    {
        DialogManager.instance.StartDialog(OnMineDestinationReachedDialogList, this, true); 
        missionStage = MissionStage.ReachedMine;
        FirstPersonController.instance.ChangeToNoMovement();
    }

    public override void OnMissionStarted()
    {
        missionStage = MissionStage.MissionStart;
        DialogManager.instance.StartDialog(MissionStartDialogList, this, true); 
    }

    public void StartMine()
    {
        MusicManager.instance.PlayMusic(onMineStartedMusic);
        MineController.instance.ActivateMine();
        MineController.instance.OnMineRouteTriggerEntered += MineController_OnMineRouteTriggerEntered;
        MineController.instance.OnMineEdgeTriggered += MineController_OnMineEdgeTriggered;
        MineController.instance.OnMineStartingPointActivated += MineController_OnMineStartingPointActivated;
        MineController.instance.OnMineSolved += MineController_OnMineSolved;
    }

    private void MineController_OnMineSolved(object sender, EventArgs e)
    {
        MusicManager.instance.StopMusic(true);
        DialogManager.instance.StartDialog(OnMineSolvedDialogList, this, true);
        missionStage = MissionStage.MineSolved;
    }

    private void MineController_OnMineStartingPointActivated(object sender, EventArgs e)
    {
        DialogManager.instance.StartDialog(OnMineStartingPointActivatedDialogList, this, true);
    }

    private void MineController_OnMineEdgeTriggered(object sender, MineController.OnMineEdgeTriggeredEventArgs e)
    {
        if(e.lives == 1)
        {
            DialogManager.instance.StartDialog(OnMineEdgeCollidedDialogList, this, true); 
        }
        else if(e.lives == 0)
        {
            OnMineDetonated();
        }
    }

    public override void OnMineDetonated()
    {
        MusicManager.instance.StopMusic(true);
        ChangeSceneDelay = 6.5f;
        DialogManager.instance.StartDialog(OnMineDetonatedDialogList, this, true); 
        OnMissionFailed();
    }

    private void MineController_OnMineRouteTriggerEntered(object sender, MineController.OnMineRouteTriggerEnteredEventAgrs e)
    {
        switch(e.Route)
        {
            case Route.ToRight:
                DialogManager.instance.StartDialog(OnMineRouteToRightTriggerDialogList, this, false);
                break;
            case Route.ToTop:
                DialogManager.instance.StartDialog(OnMineRouteToTopTriggerDialogList, this, false);
                break;
            case Route.ToBot:
                DialogManager.instance.StartDialog(OnMineRouteToBotTriggerDialogList, this, false);
                break;
        }
    }

    public override void OnMissionFailed()
    {
        base.OnMissionFailed();

    }

    public override void OnDialogFinished()
    {
        switch(missionStage)
        {
            case MissionStage.MissionStart:
                FirstPersonController.instance.ChangeToThirdPerson(FirstPersonController.AllowedMoveDirection.Left);
                break;
            case MissionStage.ReachedMine:
                StartMine();
                missionStage = MissionStage.SolvingMine;
                break;
            case MissionStage.MineSolved:
                MineController.instance.CloseMine();
                FirstPersonController.instance.ChangeToFirstPerson();
                LightController.instance.ChangeToScopeLight();
                missionStage = MissionStage.MissionCompleted;
                MissionManager.instance.LoadNextMission();
                break;
        }
    }
}
