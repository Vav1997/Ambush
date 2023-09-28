using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class MissionTwo : Mission
{
    public enum MissionStage
    {
        MissionStart,
        AfterCutscene,
        AvoidingVehicles,
        DestinationReached,
        MissionCompleted
    }

    public MissionStage missionStage;
    //public List<Dialog> MissionStartDialogList = new List<Dialog>();
    public List<Dialog> MissionFailedDialogList = new List<Dialog>();
    public List<Dialog> AfterVehicleCutsceneDialog = new List<Dialog>();
    public List<Dialog> OnDestinationReachedDialog = new List<Dialog>();
    public List<Dialog> AfterVehiclesPassedDialog = new List<Dialog>();
    public List<VehichleController> VehichleControllersList = new List<VehichleController>();

    [SerializeField] private float vehicleStartDelay;
    [SerializeField] private Vector3 playerCrawlStartPos;
    [SerializeField] private Vector3 playerCrawlStartRotation;

    [SerializeField] private float MoveToNewPosTime;
    [SerializeField] private float playerNoticingDistance;

    [SerializeField] private SingleMusic onVehiclesStartMusic;
    

    private Coroutine vehiclesStartCor;
    private Coroutine StartVehiclesCor;
    
    void Start()
    {
        FirstPersonController.instance.OnProneForward += FirstPersonController_OnProneForward;
        if(MissionManager.instance.GetActiveMission() != this)
        {
            gameObject.SetActive(false);
        }
    }

    private void FirstPersonController_OnProneForward(object sender, EventArgs e)
    {
        foreach (VehicleImmitationController vehicleImmitationController in VehicleImmitationsController.instance.VehicleImmitationControllerList)
        {
           
            if(Vector3.Distance(vehicleImmitationController.transform.position, FirstPersonController.instance.gameObject.transform.position) < playerNoticingDistance)
            {
                OnMissionFailed();
                break;
            }
        }
    }

    public override void OnMissionStarted()
    {
        missionStage = MissionStage.MissionStart;
        StartVehiclesCor = StartCoroutine(StartVehicles());
        CutsceneController.instance.StartCutscene();
        FirstPersonController.instance.ChangeToNoMovement();
        //DialogManager.instance.StartDialog(MissionStartDialogList, this, true); 
    }

    public override void OnMissionFailed()
    {
        ChangeSceneDelay = MissionFailedDialogList[0].AudioDialog[0].length;
        base.OnMissionFailed();

        //reduce vehicles volume
        VehicleImmitationsController.instance.ReduceVehiclesVolume();
        VehicleImmitationsController.instance.StopAllCoroutines();

        //make all vehicles stop
        foreach (VehicleImmitationController singleVehicleImmitationController in VehicleImmitationsController.instance.VehicleImmitationControllerList)
        {
            singleVehicleImmitationController.StopAllCoroutines();
        }


        DialogManager.instance.StartDialog(MissionFailedDialogList, this, true); 
    }

    //is called from DialogManager script
    public override void OnDialogFinished()
    {
        switch(missionStage)
        {
            case MissionStage.MissionStart:
                break;
            case MissionStage.AfterCutscene:
                MusicManager.instance.PlayMusic(onVehiclesStartMusic); 
                vehiclesStartCor = StartCoroutine(VehicleImmitationsController.instance.StartVehicleImmitations());
                FirstPersonController.instance.ChangeToThirdPerson(FirstPersonController.AllowedMoveDirection.Forward);
                StartCoroutine(StartVehicleTimer());
                missionStage = MissionStage.AvoidingVehicles;
                break;
            case MissionStage.DestinationReached:
                StartCoroutine(VehicleImmitationsController.instance.StartAfterShelterVehicleImmitations());
                break;
            case MissionStage.MissionCompleted:
                MissionManager.instance.LoadNextMission();
                break;
        }
    }

    public override void OnCutsceneFinished()
    {
        missionStage = MissionStage.AfterCutscene;
        FirstPersonController.instance.transform.position = playerCrawlStartPos;
        FirstPersonController.instance.transform.rotation = Quaternion.Euler(playerCrawlStartRotation);
        FirstPersonController.instance.ChangeToThirdPerson(FirstPersonController.AllowedMoveDirection.NoMove);
        LightController.instance.ChangeToOutOfScopeLight();
        removeVehicles();
        DialogManager.instance.StartDialog(AfterVehicleCutsceneDialog, this, true);
    }

    public override void OnDestinationReached()
    {
        MusicManager.instance.StopMusic(false);
        VehicleImmitationsController.instance.ReduceVehiclesVolume();
        StopCoroutine(StartVehiclesCor);
        StopCoroutine(vehiclesStartCor);
        missionStage = MissionStage.DestinationReached;
        FirstPersonController.instance.ChangeToNoMovement();
        DialogManager.instance.StartDialog(OnDestinationReachedDialog, this, true);
    }

    public override void OnVehiclesPassed()
    {
        missionStage = MissionStage.MissionCompleted;
        DialogManager.instance.StartDialog(AfterVehiclesPassedDialog, this, true);
        
    }


    public void removeVehicles()
    {
        foreach (VehichleController vehicle in VehichleControllersList)
        {
            Destroy(vehicle.gameObject);
        }
    }

    public IEnumerator StartVehicles()
    {
        foreach (VehichleController vehicle in VehichleControllersList)
        {
            vehicle.StartVehicleMovement();
            yield return new WaitForSeconds(vehicleStartDelay);
        }
    }

    

    public IEnumerator StartVehicleTimer()
    {
        float moveToNewPosTimer = MoveToNewPosTime;
        while(moveToNewPosTimer >= 0)
        {
            moveToNewPosTimer -= Time.deltaTime;
            yield return null;
        }
        
        StopCoroutine(vehiclesStartCor); 
    }
}
