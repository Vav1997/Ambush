using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    public static MainMenuController instance;
    [SerializeField] private SwipeDetector swipeDetector;

    [SerializeField] private ButtonDialog storyModeButton;
    [SerializeField] private ButtonDialog endlessModeButton;
    private ButtonDialog selectedButton;
    [SerializeField] private AudioClip levelSelectedClip;
    [SerializeField] private AudioClip startDialogFirstTime;
    [SerializeField] private AudioClip startDialogInGame;
    [SerializeField] private SingleMusic mainMenuMusic;
    
    private Button activeButton;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        Navigation NewNav = new Navigation();
        NewNav.mode = Navigation.Mode.Explicit;

        storyModeButton.button.onClick.AddListener(() => StoryModeButtonClick());
        endlessModeButton.button.onClick.AddListener(() => EndlessModeButtonClick());
        NewNav.selectOnUp = storyModeButton.button;
        NewNav.selectOnDown = endlessModeButton.button;      
    }

    private void Start()
    {
        if(PlayerPrefs.GetInt("IS_FIRST_TIME", 1) == 1)
        {
            PlayerPrefs.SetInt("IS_FIRST_TIME", 0);
            TextVoicingManager.Instance.VoiceText(startDialogFirstTime, false, 3);
        }
        else
        {
            TextVoicingManager.Instance.VoiceText(startDialogInGame, false, 0);
        }

        MusicManager.instance.PlayMusic(mainMenuMusic);
        swipeDetector.OnSwipe += swipeDetector_OnSwipe;
        swipeDetector.OnDoubleTap += swipeDetector_OnDoubleTap;
        swipeDetector.OnFingerHold += swipeDetector_OnFingerHold;
    }

    private void swipeDetector_OnFingerHold(object sender, EventArgs e)
    {
        if(selectedButton != null)
        {
            TextVoicingManager.Instance.VoiceText(selectedButton.buttonDescriptionAudio, false, 1);
        }
    }

    private void swipeDetector_OnDoubleTap(object sender, EventArgs e)
    {
        if(activeButton != null)
        {
            TextVoicingManager.Instance.VoiceText(levelSelectedClip, true, 1);
        }
    }

    private void swipeDetector_OnSwipe(object sender, SwipeData e)
    {
        switch(e.Direction)
        {
            case SwipeDirection.Up:
            selectedButton = storyModeButton;
            SelectButton(storyModeButton);
            break;
            case SwipeDirection.Down:
            selectedButton = endlessModeButton;
            SelectButton(endlessModeButton);
            break;
        }
    }


    public void SelectButton(ButtonDialog selectedButton)
    {
        selectedButton.button.Select();
        SetActiveButton(selectedButton.button);
        TextVoicingManager.Instance.VoiceText(selectedButton.buttonAudio, false, selectedButton.interruptionLevel);
    }

    public void SetActiveButton(Button button)
    {
        activeButton = button;
    }

    public void StoryModeButtonClick()
    {
        SceneLoader.Instance.LoadLevel(SceneLoader.Scene.StoryMode);
    }

    public void EndlessModeButtonClick()
    {
        SceneLoader.Instance.LoadLevel(SceneLoader.Scene.EndlessMode);
    }

    public void OnTextVoicingFinished()
    {
        activeButton.onClick.Invoke();
    }
}

[System.Serializable]
public class ButtonDialog
{
    public Button button;
    public AudioClip buttonAudio;
    public AudioClip buttonDescriptionAudio;
    public int interruptionLevel;
}
