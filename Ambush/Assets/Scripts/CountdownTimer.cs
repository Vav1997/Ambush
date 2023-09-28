using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private MissionOne MissionOne;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        MissionOne.OnTimeValueChanged += MissionOne_OnTimerValueChanged;
        MissionOne.OnTimerFinished += MissionOne_OnTimerFinished;
    }

    private void MissionOne_OnTimerFinished(object sender, EventArgs e)
    {
        timerText.text = "";
    }

    private void MissionOne_OnTimerValueChanged(object sender, MissionOne.OnTimerValueChangedEventArgs e)
    {
        timerText.text = e.TimerValue.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
