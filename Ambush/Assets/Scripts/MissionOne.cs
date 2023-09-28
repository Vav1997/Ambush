using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class MissionOne : Mission
{
    public enum MissionStage
    {
        EliminateFirstEnemy,
        EliminateSecondEnemy,
        SecondEnemyEliminated,
        EliminateThirdEnemy,
        ThirdEnemyEliminated,
        MissionFaled,
        MissionCompleted
    }

    public MissionStage missionStage;
    public List<Dialog> MissionStartDialog = new List<Dialog>();
    public List<Dialog> OnFirstEnemyEliminatedDialogList = new List<Dialog>();
    public List<Dialog> OnSecondEnemyEliminatedDialogList = new List<Dialog>();
    public List<Dialog> OnThirdEnemyEliminatedDialogList = new List<Dialog>();
    public List<Dialog> OnThirdEnemyKillTimeExpiringDialogList = new List<Dialog>();
    public List<Dialog> OnThirdEnemyKillTimeExpired = new List<Dialog>();



    public List<Dialog> OnMissionFailedDialogList = new List<Dialog>();


    [SerializeField] private GameObject firstEnemy;
    [SerializeField] private GameObject secondEnemy;
    [SerializeField] private GameObject thirdEnemy;

    [SerializeField] private int thirdEnemyKillTime;

    public event EventHandler<OnTimerValueChangedEventArgs> OnTimeValueChanged;
    public event EventHandler OnTimerFinished;

    public class OnTimerValueChangedEventArgs : EventArgs
    {
        public float TimerValue;
    }
    
    private Coroutine thirdEnemyKillTimerCor;

    [SerializeField] private SingleMusic onSecondEnemyKillMusic;
    void Start()
    {
        if(MissionManager.instance.GetActiveMission() != this)
        {
            gameObject.SetActive(false);
        }
        
    }



    public override void OnMissionStarted()
    {
        //MusicManager.instance.PlayMusic(onSecondEnemyKillMusic);
        //FirstPersonController.instance.enabled = false;
        base.OnMissionStarted();
        missionStage = MissionStage.EliminateFirstEnemy;
        DialogManager.instance.StartDialog(MissionStartDialog, this, true);
        firstEnemy.SetActive(true);
        secondEnemy.SetActive(false);
        thirdEnemy.SetActive(false);
        
    }

    


    //is called from DialogManager script
    public override void OnDialogFinished()
    {
        
        switch(missionStage)
        {
            case MissionStage.EliminateFirstEnemy:
                //FirstPersonController.instance.enabled = true;
                break;
            case MissionStage.EliminateThirdEnemy:
            
            thirdEnemyKillTimerCor = StartCoroutine(CountDownToKillThirdEnemy());
            break;
            case MissionStage.MissionCompleted:
            MissionManager.instance.LoadNextMission();
            break;
        }
    }

    public override void OnEnemyEliminated(EnemyController enemy)
    {
        switch(missionStage)
        {
            case MissionStage.EliminateFirstEnemy:
                DialogManager.instance.StartDialog(OnFirstEnemyEliminatedDialogList, this, true);
                missionStage = MissionStage.EliminateSecondEnemy;
                
                secondEnemy.SetActive(true);
                break;
            case MissionStage.EliminateSecondEnemy:
                DialogManager.instance.StartDialog(OnSecondEnemyEliminatedDialogList, this, true);
                thirdEnemy.SetActive(true);
                MusicManager.instance.PlayMusic(onSecondEnemyKillMusic);
                missionStage = MissionStage.EliminateThirdEnemy;
                break;
            case MissionStage.EliminateThirdEnemy:
                missionStage = MissionStage.ThirdEnemyEliminated;
                MusicManager.instance.StopMusic(false);
                DialogManager.instance.StartDialog(OnThirdEnemyEliminatedDialogList, this, true);
                OnTimerFinished?.Invoke(this, EventArgs.Empty);
                missionStage = MissionStage.MissionCompleted;
                if(thirdEnemyKillTimerCor != null)
                {
                    StopCoroutine(thirdEnemyKillTimerCor);
                }
                
                break;
        }
    }


    public override void OnMissionFailed()
    {
        base.OnMissionFailed();
    }

    public IEnumerator CountDownToKillThirdEnemy()
    {
        int KillTimer = thirdEnemyKillTime;

        while(KillTimer > 0 && missionStage == MissionStage.EliminateThirdEnemy)
        {
            OnTimeValueChanged?.Invoke(this, new OnTimerValueChangedEventArgs()
            {
                TimerValue = KillTimer
            });

            yield return new WaitForSeconds(1);

            KillTimer --;
            if(KillTimer == 10)
            {
                DialogManager.instance.StartDialog(OnThirdEnemyKillTimeExpiringDialogList, this, false);
            }

        }
        
        if(missionStage == MissionStage.EliminateThirdEnemy)
        {
            missionStage = MissionStage.MissionFaled;
            OnTimerFinished?.Invoke(this, EventArgs.Empty);
            ChangeSceneDelay = 9;
            OnMissionFailed();
            DialogManager.instance.StartDialog(OnThirdEnemyKillTimeExpired, this, true);
        }  
    }
}
