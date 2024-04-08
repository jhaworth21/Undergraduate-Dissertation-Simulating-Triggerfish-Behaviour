using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NestManager : MonoBehaviour
{

    List<GameObject> objectsInTerritory;

    private void Start()
    {
        objectsInTerritory = new List<GameObject>();
    }

    public void addToTerritory(GameObject obj)
    {
        objectsInTerritory.Append(obj);
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
