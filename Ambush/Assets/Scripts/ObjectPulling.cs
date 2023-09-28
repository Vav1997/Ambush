using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PoolList
{
     
    public List<GameObject> pooledObjects = new List<GameObject>();
    public int amountToPull;
    public GameObject EnemyPrefab; 
}
public class ObjectPulling : MonoBehaviour
{
   public bool canExpand;
    public static ObjectPulling instance;
    public PoolList[] PoolingList;
   
    //private List<GameObject> pooledObjects = new List<GameObject>();
    

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {

        for (int i = 0; i < PoolingList.Length; i++)
        {
            
            for (int z = 0; z < PoolingList[i].amountToPull; z++)
            {
                
                GameObject Obj = Instantiate(PoolingList[i].EnemyPrefab);
                Obj.SetActive(false);
                PoolingList[i].pooledObjects.Add(Obj);
            }
        }

    }

     public GameObject GetPulledObject(int ListIndex)
    {
        for (int i = 0; i < PoolingList[ListIndex].pooledObjects.Count; i++)
        {
            if(!PoolingList[ListIndex].pooledObjects[i].activeInHierarchy)
            {
                return PoolingList[ListIndex].pooledObjects[i];
            }
        }

        if (canExpand)
		{
			//All pooled objects in use. Create a new one, add it to the pool, then return it. 
			GameObject MissingEnemy = Instantiate(PoolingList[ListIndex].EnemyPrefab);
			MissingEnemy.SetActive(false);
			PoolingList[ListIndex].pooledObjects.Add(MissingEnemy);
			return MissingEnemy;
		}
		else
		{
			return null;
		}

        
    }
}


  
