using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndlessMissionFailUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private float fadeDuration;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        Mission.OnMissionFail += Mission_OnMissionFail;
        Mission.OnFinalScoreCalculated += Mission_OnFinalScoreCalculated;
        Hide();
    }

    private void Mission_OnFinalScoreCalculated(object sender, Mission.OnFinalScoreCalculatedEventArgs e)
    {
        scoreText.text = e.FinalScore.ToString();
    }

    private void Mission_OnMissionFail(object sender, EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    
}
