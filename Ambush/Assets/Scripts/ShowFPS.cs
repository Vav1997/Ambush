using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowFPS : MonoBehaviour
{
 public TextMeshProUGUI frameRateText;
    public float updateInterval = 0.5f;

    private float accum;
    private int frames;
    private float timeLeft;

    private void Start()
    {
        // Make the game run as fast as possible
        Application.targetFrameRate = 300;

        if (frameRateText == null)
        {
            Debug.LogWarning("FrameRateDisplay: No Text component assigned!");
            enabled = false;
            return;
        }

        timeLeft = updateInterval;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeLeft <= 0.0f)
        {
            float fps = accum / frames;
            frameRateText.text = string.Format("{0:F2} FPS", fps);
            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
}