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
        if (checkInNest() && !nestManager.getObjectsInTerritory().Contains(gameObject))
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

    private bool checkInNest()
    {
        if (nestCollidder.bounds.Contains(gameObject.transform.position)){
            //Debug.Log("in bounds");
            return true;
        }
        else 
        { 
           return false; 
        }
    }
}
