using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO - Fix issue to do with the objectsInTerritory being empty - either in this or triggerfish
// not sure yet

public class NestManager : MonoBehaviour
{

    List<GameObject> objectsInTerritory;

    void Start()
    {
        objectsInTerritory = new List<GameObject>();
    }

    public void addToTerritory(GameObject obj)
    {
        objectsInTerritory.Add(obj);
    }

    public void removeFromTerritory(GameObject obj)
    {
        objectsInTerritory.Remove(obj);
    }

    public List<GameObject> getObjectsInTerritory()
    {
        return objectsInTerritory;
    }
}
