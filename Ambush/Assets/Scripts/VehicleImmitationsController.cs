using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class VehicleImmitationsController : MonoBehaviour
{
    public static VehicleImmitationsController instance;
    [SerializeField] private PathCreator vehicleImmitationPath;
    [SerializeField] private GameObject vehicleImmitation;
    [SerializeField] private float minVehicleSpawnTime;
    [SerializeField] private float maxVehicleSpawnTime;
    [SerializeField] private int shelterVehiclesCount;
    [SerializeField] private float shelterVehiclesSpawnRate;

    public List<VehicleImmitationController> VehicleImmitationControllerList = new List<VehicleImmitationController>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void ReduceVehiclesVolume()
    {
        foreach (VehicleImmitationController vehicleImmitationController in VehicleImmitationControllerList)
        {
            vehicleImmitationController.ReduceVehicleVolume();
        }
    }

    public IEnumerator StartVehicleImmitations()
    {   
        while(true)
        {   
            float randomTime = Random.Range(minVehicleSpawnTime, maxVehicleSpawnTime);
            GameObject vehicleImmitationObj = Instantiate(vehicleImmitation, transform.position, Quaternion.identity);
            VehicleImmitationController vehicleImmitationController = vehicleImmitationObj.GetComponent<VehicleImmitationController>();
            vehicleImmitationController.pathCreator = vehicleImmitationPath;
            vehicleImmitationController.StartMovementCor();
            VehicleImmitationControllerList.Add(vehicleImmitationController);
            yield return new WaitForSeconds(randomTime);
        }
    }

    public IEnumerator StartAfterShelterVehicleImmitations()
    {
        while(shelterVehiclesCount > 0)
        {
            shelterVehiclesCount--;
            GameObject vehicleImmitationObj = Instantiate(vehicleImmitation, transform.position, Quaternion.identity);
            VehicleImmitationController vehicleImmitationController = vehicleImmitationObj.GetComponent<VehicleImmitationController>();
            vehicleImmitationController.pathCreator = vehicleImmitationPath;
            vehicleImmitationController.StartMovementCor();
            yield return new WaitForSeconds(shelterVehiclesSpawnRate);
        }

        yield return new WaitForSeconds(4);
        MissionManager.instance.GetActiveMission().OnVehiclesPassed();
    }
}
