using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NestManager : MonoBehaviour
{

    List<string> objectsInTerritory;

    private void Start()
    {
        objectsInTerritory = new List<string>();
    }

    public void addToTerritory(GameObject obj)
    {
        objectsInTerritory.Append(obj.name);
    }

    public void removeFromTerritory(GameObject obj)
    {
        objectsInTerritory.Remove(obj.name);
    }

    public List<string> getObjectsInTerritory()
    {
        return objectsInTerritory;
    }
}
