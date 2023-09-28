using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject cutsceneCamera;

    public static CutsceneController instance;

    [SerializeField] private float cutsceneDuration;

    public event EventHandler OnCutsceneStarted;
    public event EventHandler OnCutsceneEnded;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartCutscene()
    {
        StartCoroutine(Cutscene());
    }

    public IEnumerator Cutscene()
    {
        OnCutsceneStarted?.Invoke(this, EventArgs.Empty);
        mainCamera.SetActive(false);
        cutsceneCamera.SetActive(true);
        yield return new WaitForSeconds(cutsceneDuration);

        OnCutsceneEnded?.Invoke(this, EventArgs.Empty);
        MissionManager.instance.GetActiveMission().OnCutsceneFinished();
        cutsceneCamera.SetActive(false);

        
    }
}
