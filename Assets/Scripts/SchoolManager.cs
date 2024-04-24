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

    public Vector3 schoolLimit = new Vector3(10.0f, 10.0f, 10.0f);
    public Vector3 goalPos = new Vector3(0, 0, 0);

    public GameObject swimmingBounds;
    private MeshRenderer swimmingBoundsMesh;


    [Header("Fish Settings")]
    [Range(0.0f, 5.0f)] public float minSpeed;
    [Range(0.0f, 5.0f)] public float maxSpeed;
    [Range(1.0f, 10.0f)] public float neighbourDistance;
    [Range(1.0f, 5.0f)] public float rotationSpeed;

    [Header("Weighting Parameters")]
    [Range(0.0f, 1.0f)] public float cohesionWeighting;
    [Range(0.0f, 1.0f)] public float alignmentWeighting;
    [Range(0.0f, 1.0f)] public float separationWeighting;


    // Start is called before the first frame update
    void Start()
    {
        swimmingBoundsMesh = swimmingBounds.GetComponent<MeshRenderer>();

        allFish = new GameObject[numFish];
        for (int i = 0; i < numFish; ++i)
        {
            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-schoolLimit.x, schoolLimit.x),
                Random.Range(-schoolLimit.y, schoolLimit.y),
                Random.Range(-schoolLimit.z, schoolLimit.z));

            allFish[i] = Instantiate(fishPrefab, pos, Quaternion.identity);
        }

        SM = this;
        goalPos = this.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0, 100) < 10)
        {
            goalPos = new Vector3(
                Random.Range(-swimmingBoundsMesh.bounds.extents.x, swimmingBoundsMesh.bounds.extents.x),
                Random.Range(-swimmingBoundsMesh.bounds.extents.y, swimmingBoundsMesh.bounds.extents.y),
                Random.Range(-swimmingBoundsMesh.bounds.extents.z, swimmingBoundsMesh.bounds.extents.z));
        }
    }
}
