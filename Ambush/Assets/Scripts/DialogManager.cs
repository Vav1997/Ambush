using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogManager : MonoBehaviour
{

    public static DialogManager instance;
    private AudioSource audioSource;
    [SerializeField] private Coroutine currentPlayingCoroutine;
    [SerializeField] private Dialog currentPlayingDialog;
    [SerializeField] private List<Dialog> currentPlayingDialogList = new List<Dialog>();
    [SerializeField] private int currentDialogAudioIndex;
    private bool isSkipping;
    private int tapCount;
    private int i;
    [SerializeField] private int dialogSkipTapCount;
    [SerializeField] private float tappingTime;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        audioSource = GetComponent<AudioSource>();
    }  


    private void Update()
    {

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            tapCount += 1;
            StartCoroutine(Countdown());    
        }
       
        if (tapCount == dialogSkipTapCount)
        {    
            tapCount = 0;
            StopCoroutine(Countdown());
            
            //SkipDialogAudio();

        }
    }

    private IEnumerator Countdown()
    { 
        yield return new WaitForSeconds(tappingTime);
        tapCount = 0;  
    }

    

    public void StartDialog(List<Dialog> dialog , Mission mission, bool returnCallback)
    {
        //If something is playing right now
        if(currentPlayingCoroutine != null)
        {
            //if the interruption level is more than the one that is playing right now
            if(dialog[0].interruptionLevel >= currentPlayingDialog.interruptionLevel)
            {
                StopCoroutine(currentPlayingCoroutine);
                currentPlayingCoroutine = StartCoroutine(playAudioSequentially(dialog, mission, returnCallback));
                currentPlayingDialogList = dialog;
                currentPlayingDialog = dialog[0];
            }
        }
        else
        {
            currentPlayingDialogList = dialog;
            currentPlayingDialog = dialog[0];
            if(currentPlayingCoroutine != null)
            {
                StopCoroutine(currentPlayingCoroutine);
            }
            currentPlayingCoroutine = StartCoroutine(playAudioSequentially(dialog, mission, returnCallback));
        }
    }
    
    public bool isNowPlaying()
    {
        return audioSource.isPlaying;
    }

    public bool isNowPlayingTheSameDialog(Dialog checkDialog)
    {
        foreach (AudioClip checkAudioClip in checkDialog.AudioDialog)
        {
            if(checkAudioClip == audioSource.clip)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator playAudioSequentially(List<Dialog> dialog, Mission mission, bool returnCallback)
    {
        yield return null;

        
        //1.Loop through each AudioClip
        for (i = 0; i < dialog.Count; i++)
        {
            currentDialogAudioIndex = i;
            //2.Assign current AudioClip to audiosource
            if(dialog[i].AudioDialog.Length != 0)
            {
                audioSource.clip = dialog[i].AudioDialog[Random.Range(0, dialog[i].AudioDialog.Length)];

                //3.Play Audio
                audioSource.Play();
            }
            

            //4.Wait for it to finish playing
            while (audioSource.isPlaying && !isSkipping)
            {
                yield return null;
            }
            
            
            yield return new WaitForSeconds(dialog[i].DelayBetweenNextAudio);

            
            //5. Go back to #2 and play the next audio in the adClips array
        }

       
        currentDialogAudioIndex = 0;
        currentPlayingCoroutine = null;
        currentPlayingDialogList = null;
        currentPlayingDialog = null;
        audioSource.clip = null;
        
        if(!isSkipping)
        {
            if(returnCallback)
            {
                mission.OnDialogFinished();
            }
        }
        
    }


    public void SkipDialogAudio()
    {
        if(currentPlayingDialogList != null)
        {
            isSkipping = true;
            audioSource.Stop();
            if(i < currentPlayingDialogList.Count -1 )
            {
                i++;
                audioSource.clip = currentPlayingDialogList[i].AudioDialog[Random.Range(0, currentPlayingDialogList[i].AudioDialog.Length)];
                audioSource.Play();
            }
    
            isSkipping = false;
        }
        
    }



    public void VoiceNumbersByScore(List<NumberVoice> numberVoices, Mission mission, int score)
    {
        List<NumberVoice> scoreNumberVoiceList = new List<NumberVoice>();
        string numberString = score.ToString();

        for (int i = 0; i < numberString.Length; i++) {
            char digitChar = numberString[i];
            int digitInt = int.Parse(digitChar.ToString());
            // do something with digitInt
            for (int x = 0; x < numberVoices.Count; x++)
            {
                if(numberVoices[x].number == digitInt)
                {
                    scoreNumberVoiceList.Add(numberVoices[x]);
                    break;
                }
            }
        }
        StartCoroutine(PlayNumberSoundsInsequence(scoreNumberVoiceList, mission));
    }

    public IEnumerator PlayNumberSoundsInsequence(List<NumberVoice> numberVoices, Mission mission)
    {
        yield return null;
        
        //1.Loop through each AudioClip
        for (int i = 0; i < numberVoices.Count; i++)
        {
            //2.Assign current AudioClip to audiosource
            if(numberVoices[i].numberAudio)
            {
                audioSource.clip = numberVoices[i].numberAudio;

                //3.Play Audio
                audioSource.Play();
            }
            

            //4.Wait for it to finish playing
            while (audioSource.isPlaying)
            {
                yield return null;
            }
            Debug.Log("Current delay is " + numberVoices[i].DelayBetweenNextAudio);
            yield return new WaitForSeconds(numberVoices[i].DelayBetweenNextAudio);

            
            //5. Go back to #2 and play the next audio in the adClips array
        }

        audioSource.clip = null;
        
        //mission.OnDialogFinished();
    }
}
