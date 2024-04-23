using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NestManager : MonoBehaviour
{

    List<GameObject> objectsInNest;

    void Start()
    {
        objectsInNest = new List<GameObject>();
    }

    private void Update()
    {
        if(objectsInNest.Count > 0)
        {
            try
            {
                foreach (GameObject go in objectsInNest)
                {
                    if (!go.activeInHierarchy)
                    {
                        objectsInNest.Remove(go);
                    }
                }
            }
            catch(System.Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }

    public void addToNest(GameObject obj)
    {
        objectsInNest.Add(obj);
    }

    public void removeFromNest(GameObject obj)
    {
        objectsInNest.Remove(obj);
    }

    public List<GameObject> getObjectsInNest()
    {
        return objectsInNest;
    }
}
