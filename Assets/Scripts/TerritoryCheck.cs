using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryCheck : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject nest;
    MeshCollider nestCollidder;
    NestManager nestManager;

    private void Start()
    {
        nest = GameObject.Find("Nest");
        nestCollidder = nest.GetComponent<MeshCollider>();
        nestManager = nest.GetComponent<NestManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (checkInNest(gameObject))
        {
            nestManager.addToTerritory(gameObject);
        }
        else
        {
            if (nestManager.getObjectsInTerritory().Contains(gameObject))
            {
                nestManager.removeFromTerritory(gameObject);
            }
        }
    }

    private bool checkInNest(GameObject obj)
    {
        if (nestCollidder.bounds.Contains(obj.transform.position)){
            return true;
        }
        else 
        { 
           return false; 
        }
    }
}
