using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class MissionFourTimerTextUI : MonoBehaviour
{
    [SerializeField] private MissionFour missionFour;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        missionFour.OnTimeValueChanged += MissionFour_OnTimerValueChanged;
        missionFour.OnTimerFinished += MissionFour_OnTimerFinished;
    }

    private void MissionFour_OnTimerValueChanged(object sender, MissionFour.OnTimerValueChangedEventArgs e)
    {
        timerText.text = e.TimerValue.ToString();
    }

    private void MissionFour_OnTimerFinished(object sender, EventArgs e)
    {
        timerText.text = "";
    }
}
