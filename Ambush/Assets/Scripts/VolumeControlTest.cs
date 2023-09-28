using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeControlTest : MonoBehaviour
{
    public Transform player;
    public Transform cameraObject;
    public float xRotation;
    public float yRotation;
    public float targetYRotation;
    public float targetXRotation;

    public float volume;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        xRotation = player.localRotation.eulerAngles.y;
        yRotation = cameraObject.localRotation.eulerAngles.x;
        if(yRotation < targetYRotation)
        {
            volume = yRotation / targetYRotation * 5;
        }
        else
        {
            volume = targetYRotation / yRotation * 5;
        }
        
    }
}
