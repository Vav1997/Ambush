using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum Speaker {Partner, Player}

[System.Serializable]
public class Dialog 
{
    public Speaker speaker;
    public AudioClip[] AudioDialog;
    public float DelayBetweenNextAudio;
    public int interruptionLevel;
}
