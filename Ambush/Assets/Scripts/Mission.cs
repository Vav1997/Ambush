using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CollidedScopeSide { Top, Bot, Left, Right}

public class Mission : MonoBehaviour
{
    public string MissionName;

    //SideCollisionsDialogs
    [Header("SideCollision Dialogs")]
    public List<Dialog> OnScopeCollidedTopDialogList = new List<Dialog>();
    public List<Dialog> OnScopeCollidedBotDialogList = new List<Dialog>();
    public List<Dialog> OnScopeCollidedLeftDialogList = new List<Dialog>();
    public List<Dialog> OnScopeCollidedRightDialogList = new List<Dialog>();
    
    public List<Dialog> OnShotMissedDialogList = new List<Dialog>();

    public int OnTimerValueChanged { get; internal set; }
    protected float ChangeSceneDelay = 5f;

    protected int score;

    public static event EventHandler OnMissionFail;
    public static event EventHandler OnMissionComplete;
    public static event EventHandler<OnFinalScoreCalculatedEventArgs> OnFinalScoreCalculated;
        public class OnFinalScoreCalculatedEventArgs : EventArgs
    {
        public int FinalScore;
    }
    
    public virtual void OnMissionStarted()
    {
        FirstPersonController.instance.OnScopeSideCollided += FirstPersonController_OnScopeSideCollided;
        FirstPersonController.instance.checkPointer.OnShotMissed += checkPointer_OnShotMissed;
    }

    protected virtual void checkPointer_OnShotMissed(object sender, EventArgs e)
    {
        DialogManager.instance.StartDialog(OnShotMissedDialogList, this, false);
    }

    public virtual void OnMissionCompleted()
    {
        MusicManager.instance.StopMusic(false);
        FirstPersonController.instance.enabled = false;
        StartCoroutine(ChangeScene());
        OnMissionComplete.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnMissionFailed()
    {
        MusicManager.instance.StopMusic(false);
        FirstPersonController.instance.enabled = false;
        OnMissionFail?.Invoke(this, EventArgs.Empty);
        StartCoroutine(ChangeScene());
        OnFinalScoreCalculated?.Invoke(this, new OnFinalScoreCalculatedEventArgs()
        {
            FinalScore = score
        });
    }

    private IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(ChangeSceneDelay);
        SceneLoader.Instance.LoadLevel(SceneLoader.Scene.MainMenu);
    }
    
    public virtual void OnDialogFinished()
    {

    }

    public virtual void OnEnemyEliminated(EnemyController enemy)
    {

    }

    public virtual void OnCutsceneFinished()
    {
        
    }

    public virtual void OnDestinationReached()
    {
        
    }

    public virtual void OnMineDestinationReached()
    {
        
    }

    public virtual void OnMineDetonated()
    {

    }

    public virtual void OnVehiclesPassed()
    {
        
    }

    public virtual void OnMineRouteTriggerEntered(Route route)
    {

    }

    public virtual void OnShotMissed()
    {

    }

    private void FirstPersonController_OnScopeSideCollided(object sender, FirstPersonController.OnScopeSideCollidedEventArgs e)
    {
        
        switch(e.collidedSide)
        {
            case CollidedScopeSide.Top:
                if(!DialogManager.instance.isNowPlayingTheSameDialog(OnScopeCollidedTopDialogList[0]))
                {
                    DialogManager.instance.StartDialog(OnScopeCollidedTopDialogList, this, false);
                }
                break;
            case CollidedScopeSide.Bot:
                if(!DialogManager.instance.isNowPlayingTheSameDialog(OnScopeCollidedBotDialogList[0]))
                {
                    DialogManager.instance.StartDialog(OnScopeCollidedBotDialogList, this, false);
                }
                break;
            case CollidedScopeSide.Left:
                if(!DialogManager.instance.isNowPlayingTheSameDialog(OnScopeCollidedLeftDialogList[0]))
                {
                    DialogManager.instance.StartDialog(OnScopeCollidedLeftDialogList, this, false);
                }
                break;
            case CollidedScopeSide.Right:
                if(!DialogManager.instance.isNowPlayingTheSameDialog(OnScopeCollidedRightDialogList[0]))
                {
                    DialogManager.instance.StartDialog(OnScopeCollidedRightDialogList, this, false);
                }
                break;
        }
    }
}
