using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    public enum Scene {StoryMode, EndlessMode, MainMenu}

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }



    public void LoadLevel(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }


}
