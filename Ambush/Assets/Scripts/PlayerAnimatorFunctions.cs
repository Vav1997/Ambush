using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorFunctions : MonoBehaviour
{
    

    public void Prone()
    {
        FirstPersonController.instance.Move();
    }
    
}
