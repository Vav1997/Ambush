using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public enum RotationType {Single, Constant}
    public EnemyController enemyController;

    
    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
        
    }
    void Start()
    {
        if(enemyController.enemyType == EnemyController.EnemyType.WatchTower)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
        else
        {
            StartCoroutine(LookAtPlayer());
        }
        
    }

    public IEnumerator LookAtPlayer()
    {
        while(true)
        {
            
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            yield return null;
        }
    }
}
