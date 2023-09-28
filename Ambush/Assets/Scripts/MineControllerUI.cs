using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MineControllerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mineTimerText;
    [SerializeField] private RectTransform playerDragImageRect;
    [SerializeField] private Image playerDragImage;

    private void Start()
    {
        MineController.instance.OnMineStarted += MineController_OnMineStarted;
        MineController.instance.OnMineClosed += MineController_OnMineClosed;
        MineController.instance.OnMineDetonationTimeChanged += MineController_OnMineDetonationTimeChanged;
        MineController.instance.OnDrapPositionChanged += MineController_OnDrapPositionChanged;
        Hide();
    }

    private void MineController_OnDrapPositionChanged(object sender, MineController.OnDrapPositionChangedEventArgs e)
    {
        playerDragImageRect.position = e.touchPosition;
    }

    private void MineController_OnMineClosed(object sender, EventArgs e)
    {
        mineTimerText.text = "";
        Hide();
    }

    private void MineController_OnMineDetonationTimeChanged(object sender, MineController.OnMineDetonationTimeChangedEventArgs e)
    {
        var ts = TimeSpan.FromSeconds(e.MineTimer);
        mineTimerText.text = string.Format("{0:00}:{1:00}", (int)ts.TotalMinutes, (int)ts.Seconds);
    }

    private void MineController_OnMineStarted(object sender, EventArgs e)
    {
        Debug.Log("mtela show");
        Show();
    }

    private void Show()
    {
        this.gameObject.SetActive(true);
    }

     private void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
