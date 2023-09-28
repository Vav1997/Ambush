using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionFailUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private float fadeDuration = 2f;

    private void Start()
    {
        Mission.OnMissionFail += MissionManager_OnMissionFail;
        Mission.OnMissionComplete += Mission_OnMissionComplete;
    }

    private void Mission_OnMissionComplete(object sender, EventArgs e)
    {
        StartCoroutine(FadeImage());
    }

    private void OnDisable()
    {
         Mission.OnMissionFail -= MissionManager_OnMissionFail;
         Mission.OnMissionComplete -= Mission_OnMissionComplete;
    }

    private void MissionManager_OnMissionFail(object sender, EventArgs e)
    {
        
        StartCoroutine(FadeImage());
    }


    private IEnumerator FadeImage()
    {
        // Set the initial alpha value to 0
        float alpha = 0f;
        Color imageColor = backgroundImage.color;
        imageColor.a = alpha;
        backgroundImage.color = imageColor;

        // Gradually increase the alpha value over time
        while (alpha < 1f)
        {
            alpha += Time.deltaTime / fadeDuration;
            imageColor.a = alpha;
            backgroundImage.color = imageColor;
            yield return null;
        }
    }
}
