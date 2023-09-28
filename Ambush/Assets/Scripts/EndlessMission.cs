using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum SpawnPosType {WatchTower, Ground}
[System.Serializable]
public class EnemyPosition
{
    public Transform SpawnPos;
    public float VolumeMaxDistance;

}

public enum NextEnemyPosition {Left, Right}

public enum GameState {WaitingForStart, Started, GameOver}

public class EndlessMission : Mission
{
    public List<Dialog> MissionStartDialog = new List<Dialog>();
    public List<Dialog> OnEnemyLeftSideDialog = new List<Dialog>();
    public List<Dialog> OnEnemyRightSideDialog = new List<Dialog>();

    public List<NumberVoice> NumberVoiceList = new List<NumberVoice>();
    private List<NumberVoice> scoreNumbersVoiceList = new List<NumberVoice>();
    private NextEnemyPosition nextEnemyPosition;

    [SerializeField] private int startingKillTime;
    [SerializeField] private int timeAddedAfterKill;
    private float timer;
    private int timerSeconds;
    [SerializeField] private float minYRotation;
    [SerializeField] private float maxYRotation;
    [SerializeField] private float minXRotation;
    [SerializeField] private float maxXRotation;

    [SerializeField] private GameObject watchTowerEnemyPrefab;

    private GameObject currentEnemy;
    private EnemyPosition lastEnemyPosition;

    private EnemyPosition enemyPosition;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip singleTic;
    [SerializeField] private AudioClip doubleTic;
    [SerializeField] private AudioClip monotonTic;


    [SerializeField] private List<EnemyPosition> enemyPositionsList = new List<EnemyPosition>();

    [SerializeField] private GameState gameState;


    public event EventHandler<OnEndlessTimerValueChangedEventArgs> OnEndlessTimerValueChanged;
    public class OnEndlessTimerValueChangedEventArgs : EventArgs 
    {
        public float TimerValue;
    }



    private void Start()
    {
        FirstPersonController.instance.ChangeScopeScrollValues(minXRotation, maxXRotation, minYRotation, maxYRotation);
        timer = startingKillTime;
        timerSeconds = startingKillTime;
        gameState = GameState.WaitingForStart;
    }

    private void Update()
    {
        switch(gameState)
        {
            case GameState.WaitingForStart:
                break;
            case GameState.Started:
                timer -= Time.deltaTime;
                OnEndlessTimerValueChanged?.Invoke(this, new OnEndlessTimerValueChangedEventArgs()
                {
                    TimerValue = timer
                });
                if(timer <= 0)
                {
                    gameState = GameState.GameOver;
                    OnMissionFailed();
                }
                break;
        }
    }


    public override void OnMissionStarted()
    {
        base.OnMissionStarted();
        DialogManager.instance.StartDialog(MissionStartDialog, this, true);
        
    }


    public override void OnEnemyEliminated(EnemyController enemy)
    {
        score++;
        StartCoroutine(SpawnEnemy());
        AddAdditionalTime();
        
    }

    private void AddAdditionalTime()
    {
        timer += timeAddedAfterKill;
        timerSeconds += timeAddedAfterKill;
        if(timerSeconds > startingKillTime)
        {
           timerSeconds = startingKillTime;
           timer = startingKillTime;
        }
    }

    public override void OnMissionFailed()
    {
        base.OnMissionFailed();
        DialogManager.instance.VoiceNumbersByScore(NumberVoiceList, this, score);

    }

    public override void OnDialogFinished()
    {
        switch(gameState)
        {
            case GameState.WaitingForStart:
            StartCoroutine(TimerInSeconds());
            StartCoroutine(SpawnEnemy());
            gameState = GameState.Started;
            break;
        }
    }

    private IEnumerator TimerInSeconds()
    {
        while(timerSeconds > 0)
        {   
            if(timerSeconds > 10)
            {
                audioSource.PlayOneShot(singleTic);
            }
            else if(timerSeconds < 10 && timerSeconds > 2)
            {
                audioSource.PlayOneShot(doubleTic);
            }
            else if(timerSeconds == 2)
            {
                audioSource.PlayOneShot(monotonTic);
            }
            yield return new WaitForSeconds(1);
            timerSeconds--;
        }
    }

    private IEnumerator SpawnEnemy()
    {
        
        bool indexFound = false;
  
        while(!indexFound)
        {
            int randomPosIndex = UnityEngine.Random.Range(0, enemyPositionsList.Count);

            if(lastEnemyPosition != enemyPositionsList[randomPosIndex])
            {
                indexFound = true;
                lastEnemyPosition = enemyPositionsList[randomPosIndex];
                enemyPosition = enemyPositionsList[randomPosIndex];
                GameObject newEnemy =  Instantiate(watchTowerEnemyPrefab,enemyPosition.SpawnPos.position, Quaternion.identity);
                newEnemy.GetComponent<EnemyController>().ChangeAudioSourceDistance(enemyPosition.VolumeMaxDistance);

                Vector3 pos = Listener.instance.transform.position;
                Vector3 dir = (newEnemy.transform.position - Listener.instance.transform.position).normalized;
                if(dir.x > 0)
                {
                    DialogManager.instance.StartDialog(OnEnemyRightSideDialog, this, false);
                }
                else if (dir.x < 0)
                {
                    DialogManager.instance.StartDialog(OnEnemyLeftSideDialog, this, false);
                }
            }
            yield return null;
        }
    }

    //returns -1 when to the left, 1 to the right, and 0 for forward/backward
    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);
 
        if (dir > 0.0f) {
            return 1.0f;
        } else if (dir < 0.0f) {
            return -1.0f;
        } else {
            return 0.0f;
        }
    }  
    private IEnumerator ConstantSpawn()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(SpawnEnemy());
        }
    }

    

    public float GetGamingTimerNormalised()
    {
        return 1 - (timer / startingKillTime);
    }
}
