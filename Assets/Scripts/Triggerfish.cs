using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Triggerfish : MonoBehaviour
{   
    //defines the possible states of the triggerfish
    private enum State { Chasing, Circling, Patrolling };

    //header section for the variables relating to the current state
    [Header("State Variables")]
    [SerializeField]
    private State state;
    [SerializeField]
    private float timeSinceLastStateChange;
    [SerializeField]
    private Vector3 goalPos;
    [SerializeField]
    private Vector3 currentPos;
    
    //header section for the tunable parameters not relating to movement
    [Header("Tunable Parameters")]
    public float goalAdjustment = 0.3f;
    public float stateAdjustment = 0.3f;

    //header for the variables relating to the territory area and movement area
    [Header("Boundaries and Territory")]
    //variables for the floor of the world 
    public GameObject worldFloor;
    [SerializeField]
    private float floorBoundary;

    //variables for the nest and patrol areas (and mesh colliders)
    public GameObject nest;
    private MeshCollider nestMeshCollider;
    public GameObject patrolArea;
    private MeshCollider patrolAreaCollider;

    //header for the variables related to veclocity
    [Header("Velocity")]
    public float speed;
    public float turningSpeed;
    public float minSpeed;
    public float maxSpeed;
    public float accerlerationFactor;

    //header for the variables related to vision
    [Header("VIsion")]
    public float viewRange;
    public float viewAngle;



    // Start is called before the first frame update
    void Start()
    {
        state = State.Patrolling;
        nestMeshCollider = nest.GetComponent<MeshCollider>();

        //sets the lowest point that the fish can go to 
        worldFloor = GameObject.Find("Ocean Floor");
        floorBoundary = worldFloor.GetComponent<Transform>().position.y;

        //generates a goal 
        goalPos = generateGoalPos(nestMeshCollider.bounds, floorBoundary, Vector3.zero);
        timeSinceLastStateChange = 0;
    }

    // Update is called once per frame
    void Update()
    {
        updateState(timeSinceLastStateChange);
        List<string> inTerritory = nest.GetComponent<NestManager>().getObjectsInTerritory();

        if (inTerritory != null && state != State.Chasing)
        {
            state = State.Chasing;
        }

        switch (state)
        {
            case State.Patrolling:
                patrollingMovement(goalPos);
                break;
            case State.Circling:
                float rotationAngle = 0f;
                circlingMovement(nest, rotationAngle);
                break;
            case State.Chasing:
                break;
        }

        if(state == State.Patrolling)
        {
            patrollingMovement(goalPos);
        }

        timeSinceLastStateChange += Time.deltaTime;
    }

    /// <summary>
    /// Used to generate a random position from bounds that are parsed into it
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns>A vector generated from the bounds</returns>
    private Vector3 generateGoalPos(Bounds bounds, float floorBoundary, Vector3 previousGoal)
    {
        Vector3 upperBound = bounds.max;
        Vector3 lowerBound = bounds.min;
        float xVal;
        float yVal;
        float zVal;

        if (previousGoal == Vector3.zero)
        {
            xVal = Random.Range(upperBound.x, lowerBound.x);
            yVal = Random.Range(upperBound.y - 10, floorBoundary);
            zVal = Random.Range(upperBound.z, lowerBound.z);
        }
        else
        {
            xVal = previousGoal.x + Random.value * goalAdjustment;
            yVal = previousGoal.y + Random.value * goalAdjustment;
            zVal = previousGoal.z + Random.value * goalAdjustment;
        }

        return new Vector3(xVal, yVal, zVal);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeSinceStateChange"></param>
    private void updateState(float timeSinceStateChange)
    {
        if(timeSinceLastStateChange < 0.3)
        {

        }
        else
        {
            timeSinceLastStateChange += Time.deltaTime;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="goalPos"></param>
    private void patrollingMovement(Vector3 goalPos)
    {
        Vector3 direction = goalPos - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                                Quaternion.LookRotation(direction),
                                                turningSpeed * Time.deltaTime);

        this.transform.Translate(0, 0, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, goalPos) < 1)
        {
            goalPos = generateGoalPos(nestMeshCollider.bounds, floorBoundary, goalPos);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nest"></param>
    /// <param name="angle"></param>
    private void circlingMovement(GameObject nest, float angle)
    {
        //definitions of the points to create the vector to rotate around
        float xPos = this.nestMeshCollider.transform.position.x;
        float yPos = this.transform.position.y;
        float zPos = this.nestMeshCollider.transform.position.z;

        Vector3 rotationPoint = new Vector3(xPos, yPos, zPos);
        float radius = Vector3.Distance(this.transform.position, rotationPoint);

        xPos =  radius * Mathf.Cos(angle);
        zPos = radius * Mathf.Sin(angle);

        //this.transform
        //Vector3 direction = 
    }

    private void chasingMovement()
    {

    }

    // TODO - Implement check vision so that objects in the territory are looped over and 
    /// <summary>
    /// 
    /// </summary>
    /// <param name="withinRange"></param>
    private void checkInVision(GameObject[] withinRange)
    {

    }
    
    // TODO - check distance of objects in the territory to see if they are within the chasing range
    /// <summary>
    /// Used to check if the game objects within the territory are within the vision range of the 
    /// triggerfish model
    /// </summary>
    /// <param name="objects"> The objects within the territory</param>
    private void checkDistance(GameObject[] objects)
    {

    }
}
