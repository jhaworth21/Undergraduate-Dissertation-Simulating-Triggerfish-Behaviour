using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SchoolManager : MonoBehaviour
{
    public static SchoolManager SM;
    public GameObject fishPrefab;
    public int numFish = 20;
    public GameObject[] allFish;
    public Vector3 schoolLimits = new Vector3(5.0f, 5.0f, 5.0f);
    public Vector3 goalPos = Vector3.zero;

    [Header("Fish Settings")]
    [Range(0.0f, 5.0f)] public float minSpeed;
    [Range(0.0f, 5.0f)] public float maxSpeed;
    [Range(1.0f, 10.0f)] public float neighbourDistance;
    [Range(1.0f, 5.0f)] public float rotationSpeed;

    [Header("Weighting Settings")]
    public float cohesionWeighting;
    public float alignmentWeighting;
    public float separationWeighting;

    void Start()
    {

        allFish = new GameObject[numFish];

        for (int i = 0; i < numFish; ++i)
        {

            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-schoolLimits.x, schoolLimits.x),
                Random.Range(-schoolLimits.y, schoolLimits.y),
                Random.Range(-schoolLimits.z, schoolLimits.z));

            allFish[i] = Instantiate(fishPrefab, pos, Quaternion.identity);
        }

        SM = this;
        goalPos = this.transform.position;
    }


    void Update()
    {

        //if (Random.Range(0, 100) < 10)
        //{
            //goalPos = this.transform.position;
                //+ new Vector3(
                //Random.Range(-schoolLimits.x, schoolLimits.x),
                //Random.Range(-schoolLimits.y, schoolLimits.y),
                //Random.Range(-schoolLimits.z, schoolLimits.z));
        //}
    }
    //public static SchoolManager SM;
    //public GameObject fishPrefab;

    //public int schoolSize = 20;
    //public GameObject[] allFish;

    //public Vector3 schoolLimits = new Vector3(10.0f, 10.0f, 10.0f);
    //public Vector3 goalPos = new Vector3(0, 0, 0);

    //public GameObject swimmingBounds;
    //private MeshRenderer swimmingBoundsMesh;


    //[Header("Fish Settings")]
    //[Range(0.0f, 5.0f)] public float minSpeed;
    //[Range(0.0f, 5.0f)] public float maxSpeed;
    //[Range(1.0f, 10.0f)] public float neighbourDistance;
    //[Range(1.0f, 5.0f)] public float rotationSpeed;

    //[Header("Weighting Parameters")]
    //[Range(0.0f, 10.0f)] public float cohesionWeighting;
    //[Range(0.0f, 10.0f)] public float alignmentWeighting;
    //[Range(0.0f, 10.0f)] public float separationWeighting;


    //// Start is called before the first frame update
    //void Start()
    //{
    //    swimmingBoundsMesh = swimmingBounds.GetComponent<MeshRenderer>();

    //    allFish = new GameObject[schoolSize];
    //    for (int i = 0; i < schoolSize; ++i)
    //    {
    //        Vector3 pos = this.transform.position + new Vector3(
    //            Random.Range(-schoolLimits.x, schoolLimits.x),
    //            Random.Range(-schoolLimits.y, schoolLimits.y),
    //            Random.Range(-schoolLimits.z, schoolLimits.z));

    //        allFish[i] = Instantiate(fishPrefab, pos, Quaternion.identity);
    //    }

    //    SM = this;
    //    goalPos = this.transform.position;

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Random.Range(0, 100) < 10)
    //    {
    //        //sets the bounds to generate the position within
    //        Vector3 upperBound = swimmingBoundsMesh.bounds.max;
    //        Vector3 lowerBound = swimmingBoundsMesh.bounds.min;

    //        //generates a positon vector from the bounds provided
    //        float xVal = Random.Range(upperBound.x, lowerBound.x);
    //        float yVal = Random.Range(upperBound.y - 2, lowerBound.y);
    //        float zVal = Random.Range(upperBound.z, lowerBound.z);

    //        goalPos = new Vector3(xVal, yVal, zVal);

    //        if (swimmingBoundsMesh.bounds.Contains(goalPos)) { Debug.Log("in bounds"); }
    //        //float posXVal = swimmingBounds.transform.position.x + swimmingBoundsMesh.bounds.max.x;
    //        //float negXVal = swimmingBounds.transform.position.x - swimmingBoundsMesh.bounds.max.x;

    //        //float posYVal = swimmingBounds.transform.position.y + swimmingBoundsMesh.bounds.max.y;
    //        //float negYVal = swimmingBounds.transform.position.y - swimmingBoundsMesh.bounds.max.y;

    //        //float posZVal = swimmingBounds.transform.position.z + swimmingBoundsMesh.bounds.max.z;
    //        //float negZVal = swimmingBounds.transform.position.z - swimmingBoundsMesh.bounds.max.z;

    //        //goalPos = new Vector3(
    //        //    Random.Range(negXVal, posXVal),
    //        //    Random.Range(negYVal, posYVal),
    //        //    Random.Range(negZVal, posYVal));
    //    }
    //}
}
