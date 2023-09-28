using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class VehichleController : MonoBehaviour
{
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private float speed;
    [SerializeField] private GameObject[] lights;
    [SerializeField] private AudioSource myAudio;
    private float distanceTraveled;

    void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void StartVehicleMovement()
    {
        StartCoroutine(StartMovement());
    }

    public IEnumerator StartMovement()
    {
        myAudio.Play();
        foreach (GameObject light in lights)
        {
            light.SetActive(true);
        }
        while(distanceTraveled < pathCreator.path.length)
        {
            distanceTraveled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTraveled);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
