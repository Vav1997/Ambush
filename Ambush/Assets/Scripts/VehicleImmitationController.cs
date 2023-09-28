using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class VehicleImmitationController : MonoBehaviour
{
    public PathCreator pathCreator;
    [SerializeField] private float speed;
    private float distanceTraveled;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float reducedVolume;

    public Coroutine StartMovementCoroutine;



    public void ReduceVehicleVolume()
    {
        audioSource.volume = reducedVolume;
    }

    public void StartMovementCor()
    {
        StartMovementCoroutine = StartCoroutine(StartMovement());
    }

    public IEnumerator StartMovement()
    {
        while(distanceTraveled < pathCreator.path.length)
        {
            distanceTraveled += speed * Time.deltaTime;
            
            transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTraveled);
            yield return null;
        }

        VehicleImmitationsController.instance.VehicleImmitationControllerList.Remove(this);
        gameObject.SetActive(false);
    }


}
