using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Listener : MonoBehaviour
{
    public static Listener instance;
    public AudioListener audioListener;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }


    public void TurOffListener()
    {
        audioListener.enabled = false;
    }

    public void TurOnListener()
    {
        audioListener.enabled = true;
    }
}
