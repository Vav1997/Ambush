using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public static LightController instance;
    [SerializeField] private Light directionalLight;
    [SerializeField] private Color directionalLightScopeColor;
    [SerializeField] private Color directionalLightNoScopeColor;
    [SerializeField] private GameObject postProcessing;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        //ChangeToOutOfScopeLight();
    }


    public void ChangeToOutOfScopeLight()
    {
        directionalLight.color = directionalLightNoScopeColor;
        postProcessing.SetActive(false);
    }

    public void ChangeToScopeLight()
    {
        directionalLight.color = directionalLightScopeColor;
        postProcessing.SetActive(true);
    }
}
