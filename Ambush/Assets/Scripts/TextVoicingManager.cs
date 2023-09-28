using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextVoicingManager : MonoBehaviour
{
    public static TextVoicingManager Instance;

    [SerializeField] private AudioSource audioSource;

    private int currentInterruptionLevel;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

    }

    public void VoiceText(AudioClip clip, bool returnCallback, int interruptionLevel)
    {
        StartCoroutine(VoiceTextCor(clip, returnCallback, interruptionLevel));
    }


    public IEnumerator VoiceTextCor(AudioClip clip, bool returnCallback, int interruptionLevel)
    {
        if(interruptionLevel >= currentInterruptionLevel)
        {
            currentInterruptionLevel = interruptionLevel;
            if(audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }

            while(audioSource.isPlaying)
            {
                yield return null;
            }
            audioSource.clip = null;
            currentInterruptionLevel = 0;
            if(returnCallback)
            {
                MainMenuController.instance.OnTextVoicingFinished();
            }
        }

    }
}
