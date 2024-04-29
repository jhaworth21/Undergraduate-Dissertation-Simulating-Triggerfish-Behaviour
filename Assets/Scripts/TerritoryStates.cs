using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalStates : MonoBehaviour
{

    List<string> objectsInTerritory;

    private void Start()
    {
        objectsInTerritory = new List<string>();
    }

    void addToTerritory(GameObject obj)
    {
        objectsInTerritory.Append(obj.name);
    }

    void removeFromTerritory(GameObject obj)
    {
        objectsInTerritory.Remove(obj.name);
    }

    List<string> getObjectsInTerritory()
    {
        return objectsInTerritory;
    }
}
