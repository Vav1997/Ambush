using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MissionFour : Mission
{
    public enum MissionStage
    {
        MissionStart,
        EliminateFirstEnemy,
        EliminateSecondEnemy,
        EliminateGeneral,
        MissionCompleted,
        MissionFaled
    }
    public MissionStage missionStage;

    public List<Dialog> MissionStartDialogList = new List<Dialog>();
    public List<Dialog> OnFirstEnemyEliminatedDialogList = new List<Dialog>();
    public List<Dialog> OnSecondEnemyEliminatedDialogList = new List<Dialog>();
    public List<Dialog> OnThirdEnemyEliminatedDialogList = new List<Dialog>();
    public List<Dialog> MissionFailedOnSecondEnemyDialogList = new List<Dialog>();
    public List<Dialog> MissionFailedOnGeneralDialogList = new List<Dialog>();
    public List<Dialog> OnSecondEnemyKillTimeExpiringDialogList = new List<Dialog>();
    public List<Dialog> OnSecondEnemyKillTimeExpiredDialogList = new List<Dialog>();
    public List<Dialog> OnMissionCompletedDialogList = new List<Dialog>();
    public List<Dialog> NearGeneralShotMissedDialogList = new List<Dialog>();
    public List<Dialog> OnWrongTargetShotDialogList = new List<Dialog>();


    


    
    [SerializeField] private GameObject firstEnemy;
    [SerializeField] private GameObject secondEnemy;
    [SerializeField] private GameObject General;
    [SerializeField] private GameObject WrongTargetEnemy;
    [SerializeField] private float MissedShotDetectedDistance;
    [SerializeField] private int secondEnemyKillTime;
    [SerializeField] private Transform enemyRunPos;
    [SerializeField] private SingleMusic onFirstEnemyKillMusic;
    
    public event EventHandler<OnTimerValueChangedEventArgs> OnTimeValueChanged;
    public event EventHandler OnTimerFinished;

    public class OnTimerValueChangedEventArgs : EventArgs
    {
        public float TimerValue;
    }
    
    private void Start()
    {
        if(MissionManager.instance.GetActiveMission() != this)
        {
            gameObject.SetActive(false);
        }
    }



   
    public override void OnMissionStarted()
    {
        base.OnMissionStarted();
        firstEnemy.SetActive(true);
        missionStage = MissionStage.MissionStart;
        DialogManager.instance.StartDialog(MissionStartDialogList, this, true); 
        FirstPersonController.instance.ChangeScopeScrollValues(-33, -5, 0, 12);
    }

    public override void OnDialogFinished()
    {
        switch(missionStage)
        {
            case MissionStage.MissionStart:
                missionStage = MissionStage.EliminateFirstEnemy;
                break;
            case MissionStage.EliminateSecondEnemy:
                MusicManager.instance.PlayMusic(onFirstEnemyKillMusic);
                secondEnemy.SetActive(true);
                StartCoroutine(CountDownToKillSecondEnemy());
                break;
            case MissionStage.EliminateGeneral:
                General.SetActive(true);
                WrongTargetEnemy.SetActive(true);
                break;
        }   
    }
    public override void OnEnemyEliminated(EnemyController enemy)
    {
        switch(missionStage)
        {
            case MissionStage.EliminateFirstEnemy:
                missionStage = MissionStage.EliminateSecondEnemy;
                DialogManager.instance.StartDialog(OnFirstEnemyEliminatedDialogList, this, true); 
                break;
            case MissionStage.EliminateSecondEnemy:
                MusicManager.instance.StopMusic(false);
                OnTimerFinished?.Invoke(this, EventArgs.Empty);
                DialogManager.instance.StartDialog(OnSecondEnemyEliminatedDialogList, this, true);
                missionStage = MissionStage.EliminateGeneral;
                break;
            case MissionStage.EliminateGeneral:
                if(enemy.enemyType == EnemyController.EnemyType.General)
                {
                    OnMissionCompleted();
                    WrongTargetEnemy.GetComponent<EnemyController>().RunToDestination(enemyRunPos);
                }
                else if(enemy.enemyType == EnemyController.EnemyType.WrongTarget)
                {
                    DialogManager.instance.StartDialog(OnWrongTargetShotDialogList, this, true);
                    ChangeSceneDelay = OnWrongTargetShotDialogList[0].AudioDialog[0].length;
                    General.GetComponent<EnemyController>().RunToDestination(enemyRunPos);
                    OnMissionFailed();
                }
                break;
        }
    }



    public override void OnMissionCompleted()
    {
        ChangeSceneDelay = OnMissionCompletedDialogList[0].AudioDialog[0].length;
        base.OnMissionCompleted();
        
        DialogManager.instance.StartDialog(OnMissionCompletedDialogList, this, false);
    }

    public override void OnMissionFailed()
    {
        base.OnMissionFailed();
        switch(missionStage)
        {
            case MissionStage.EliminateSecondEnemy:
                DialogManager.instance.StartDialog(MissionFailedOnSecondEnemyDialogList, this, false);
                break;
        }
    }

    protected override void checkPointer_OnShotMissed(object sender, EventArgs e)
    {
        Debug.Log("overrides shot missed");
        if(missionStage != MissionStage.EliminateGeneral)
        {
            DialogManager.instance.StartDialog(OnShotMissedDialogList, this, false);
        }
        else
        {
            if(DistanceFromGeneral() < MissedShotDetectedDistance)
            {
                DialogManager.instance.StartDialog(NearGeneralShotMissedDialogList, this, false);
                ChangeSceneDelay = NearGeneralShotMissedDialogList[0].AudioDialog[0].length;
                WrongTargetEnemy.GetComponent<EnemyController>().RunToDestination(enemyRunPos);
                General.GetComponent<EnemyController>().RunToDestination(enemyRunPos);
                OnMissionFailed();
            }  
        }
    }


    public float DistanceFromGeneral()
    {
       return Vector3.Distance(Listener.instance.gameObject.transform.position, General.transform.position);
    }


    public IEnumerator CountDownToKillSecondEnemy()
    {
        int KillTimer = secondEnemyKillTime;

        while(KillTimer > 0 && missionStage == MissionStage.EliminateSecondEnemy)
        {
            OnTimeValueChanged?.Invoke(this, new OnTimerValueChangedEventArgs()
            {
                TimerValue = KillTimer
            });

            yield return new WaitForSeconds(1);

            KillTimer --;
            if(KillTimer == 10)
            {
                DialogManager.instance.StartDialog(OnSecondEnemyKillTimeExpiringDialogList, this, false);
            }
            
            
        }
        
        if(missionStage == MissionStage.EliminateSecondEnemy)
        {
            DialogManager.instance.StartDialog(OnSecondEnemyKillTimeExpiredDialogList, this, false);
            missionStage = MissionStage.MissionFaled;
            OnTimerFinished?.Invoke(this, EventArgs.Empty);
            ChangeSceneDelay = OnSecondEnemyKillTimeExpiredDialogList[0].AudioDialog[0].length;
            OnMissionFailed();
        }  
    }
}
