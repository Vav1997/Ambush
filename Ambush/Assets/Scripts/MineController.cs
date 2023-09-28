using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineController : MonoBehaviour
{
    private Route route;
    private float MineDetonationTime = 120;
    private bool mineDetonated;
    private bool mineSolved;
    private bool foundStartingPoint;

    public static MineController instance;

    public event EventHandler<OnMineEdgeTriggeredEventArgs> OnMineEdgeTriggered;
    public class OnMineEdgeTriggeredEventArgs : EventArgs
    {
        public int lives;
    }
    public event EventHandler OnMineStarted;
    public event EventHandler<OnMineRouteTriggerEnteredEventAgrs> OnMineRouteTriggerEntered;

    public class OnMineRouteTriggerEnteredEventAgrs : EventArgs
    {
        public Route Route;
    }

    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip normalTick;
    [SerializeField] private AudioClip DoubleTick;
    [SerializeField] private AudioClip MonotonTick;
    [SerializeField] private PlayerMineDragController playerMineDragController;
    
    [SerializeField] private int edgeCollisionLives;

    public event EventHandler<OnMineDetonationTimeChangedEventArgs> OnMineDetonationTimeChanged;
    public class OnMineDetonationTimeChangedEventArgs : EventArgs
    {
        public float MineTimer;
    }
    public event EventHandler OnMineStartingPointActivated;
    public event EventHandler OnMineSolved;
    public event EventHandler OnMineClosed;

    public event EventHandler<OnDrapPositionChangedEventArgs> OnDrapPositionChanged;
    public class OnDrapPositionChangedEventArgs : EventArgs 
    {
        public Vector3 touchPosition;
    }

    [SerializeField] private MineCollisionDetection mineCollisionDetection;
    private Coroutine mineTimerCor;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        playerMineDragController.OnTouchPositionChanged += playerMineDragController_OnTouchPositionChanged;
    }

    private void playerMineDragController_OnTouchPositionChanged(object sender, PlayerMineDragController.OnTouchPositionChangedEventArgs e)
    {
        OnDrapPositionChanged?.Invoke(this, new OnDrapPositionChangedEventArgs()
        {
            touchPosition = e.touchPos
        });
    }

    public void OnEdgeCollided()
    {
        if(edgeCollisionLives > 0)
        {
            edgeCollisionLives--;
            OnMineEdgeTriggered?.Invoke(this, new OnMineEdgeTriggeredEventArgs()
            {
                lives = edgeCollisionLives
            });
        }
        else
        {
            OnMineDetonate();
        }
    }

    public void OnMineDetonate()
    {
        mineDetonated = true;
        MissionManager.instance.GetActiveMission().OnMineDetonated();
        StopCoroutine(mineTimerCor);
        audioSource.Stop();
        playerMineDragController.gameObject.SetActive(false);
    }

    public void OnRouteTriggerEntered(Route route)
    {
        OnMineRouteTriggerEntered?.Invoke(this, new OnMineRouteTriggerEnteredEventAgrs()
        {
            Route = route
        });
    }

    public void ActivateMine()
    {
        OnMineStarted?.Invoke(this, EventArgs.Empty);
        mineTimerCor = StartCoroutine(MineDetonationTimer());
        playerMineDragController.gameObject.SetActive(true);
    }

    public void MineStartingPointActivated()
    {
        foundStartingPoint = true;
        OnMineStartingPointActivated?.Invoke(this, EventArgs.Empty);
    }

    public void MineSolved()
    {
        mineSolved = true;
        StopCoroutine(mineTimerCor);
        OnMineSolved?.Invoke(this, EventArgs.Empty);
        audioSource.Stop();
    }

    public bool IsDetonated()
    {
        return mineDetonated;
    }

    public bool isMineSolved()
    {
        return mineSolved;
    }

    public void CloseMine()
    {
        OnMineClosed?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }

    private IEnumerator MineDetonationTimer()
    {
        float MineDetonationTimer = MineDetonationTime;
        while(MineDetonationTimer >= 0)
        {
            OnMineDetonationTimeChanged?.Invoke(this, new OnMineDetonationTimeChangedEventArgs()
            {
                MineTimer = MineDetonationTimer
            });
            
            //this part needs optimisation. Code works, but the if, else statements must be changed
            if(MineDetonationTimer > 10)
            {
                audioSource.clip = normalTick;
            }
            else if(MineDetonationTimer <= 10 && MineDetonationTimer > 2)
            {
                audioSource.clip = DoubleTick;
            }
            else
            {
                audioSource.clip = MonotonTick;
            }
           
            if(MineDetonationTimer > 1)
            {
                audioSource.Play();
            }
            
            MineDetonationTimer--;
            yield return new WaitForSeconds(1);
        }

        OnMineDetonate();
    }

    public bool StartPointActivated()
    {
        return foundStartingPoint;
    }
}
