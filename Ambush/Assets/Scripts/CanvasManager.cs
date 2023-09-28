using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject scope;

    [SerializeField] private GameObject blackLines;


    void Start()
    {
        if(CutsceneController.instance != null)
        {
            CutsceneController.instance.OnCutsceneStarted += cutsceneController_OnCutsceneStarted;
            CutsceneController.instance.OnCutsceneEnded += cutsceneController_OnCutsceneEnded;
        }
        
        FirstPersonController.instance.OnChangedToFirstPerson += FirstPersonController_OnChangedToFirstPerson;
    }

    private void FirstPersonController_OnChangedToFirstPerson(object sender, EventArgs e)
    {
        scope.SetActive(true);
    }

    private void cutsceneController_OnCutsceneEnded(object sender, EventArgs e)
    {
        blackLines.SetActive(false);
        scope.SetActive(false);
    }

    private void cutsceneController_OnCutsceneStarted(object sender, EventArgs e)
    {
        blackLines.SetActive(true);
        scope.SetActive(false);
    }
}
