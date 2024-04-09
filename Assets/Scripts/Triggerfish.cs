using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using Random = UnityEngine.Random;

//TODO - Fix issue with cirlcing code not circling around a point (circles in a small circle)
//TODO - Fix issue where inspector values aren't updating properly
public class Triggerfish : MonoBehaviour
{   
    //defines the possible states of the triggerfish
    private enum State { Chasing, Circling, Patrolling };

    //header section for the variables relating to the current state
    [Header("State Variables")]
    [SerializeField]
    private State state;
    [SerializeField]
    private State lastState;
    [SerializeField]
    private float timeSinceLastStateChange;
    [SerializeField]
    private Vector3 goalPos;
    private Vector3 lastPos;
    [SerializeField]
    private float distanceChased;
    [SerializeField]
    private GameObject currentlyChased;
    [SerializeField]
    private float circlingAngle;
    
    //header section for the tunable parameters not relating to movement
    [Header("Tunable Parameters")]
    public float goalAdjustment = 1f;
    public float stateAdjustment = 1f;

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
    [Header("Vision")]
    public float viewRange;
    public float viewAngle;


    // Start is called before the first frame update
    void Start()
    {
        state = State.Patrolling;
        circlingAngle = 0;
        lastPos = Vector3.zero;
        currentlyChased = null;

        nestMeshCollider = nest.GetComponent<MeshCollider>();
        patrolAreaCollider = patrolArea.GetComponent<MeshCollider>();

        //sets the lowest point that the fish can go to 
        worldFloor = GameObject.Find("Ocean Floor");
        floorBoundary = worldFloor.GetComponent<Transform>().position.y;

        //generates a goal 
        goalPos = generateGoalPos(patrolAreaCollider.bounds, floorBoundary, Vector3.zero);
        timeSinceLastStateChange = 0;
    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> inTerritory = nest.GetComponent<NestManager>().getObjectsInTerritory();
        GameObject closest;
        try
        {
            closest = getNearestObj(inTerritory);
        }
        catch(NullReferenceException)
        {
            closest = null;
        }

        //TODO - Check if this makes sense to switch the state into chasing
        if (inTerritory != null && checkInVision(closest) && currentlyChased == null)
        {
            state = State.Chasing;
            currentlyChased = closest;
        }
        else
        {
            distanceChased = 0;
            if(timeSinceLastStateChange > stateAdjustment)
            {
                updateState();
            }
            else if (state == State.Patrolling)
            {
                if (Vector3.Distance(gameObject.transform.position, goalPos) < 1)
                {
                    goalPos = generateGoalPos(patrolAreaCollider.bounds, floorBoundary, goalPos);
                }
            }
            else if (state == State.Circling)
            {
                circlingAngle  = (circlingAngle > 360) ? 0 : circlingAngle;
                circlingAngle += (lastState == state) ? speed : 0;

                updateCircleGoalPos(circlingAngle, getCirclingRadius());
            }
        }
        movement();

        if(state == State.Chasing)
        {
            updateChasingGoalPos(currentlyChased);
            distanceChased += Vector3.Distance(lastPos, gameObject.transform.position);

            if (distanceChased >= 15)
            {
                updateState();
            }
        }

        Debug.Log("State = " + state);

        timeSinceLastStateChange += Time.deltaTime;
        lastState = state;
        lastPos = gameObject.transform.position;
    }

    /// <summary>
    /// 
    /// </summary>
    private void updateState()
    {
        float stateProbability = Random.Range(0, 1);
        state = (stateProbability < 0.5f) ? State.Patrolling : State.Circling;
        currentlyChased = null;
        timeSinceLastStateChange = 0;
        Debug.Log("reset time since last change");
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

        if (previousGoal == Vector3.zero || !patrolAreaCollider.bounds.Contains(goalPos))
        {
            xVal = Random.Range(upperBound.x, lowerBound.x);
            yVal = Random.Range(upperBound.y - 5, floorBoundary);
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
    /// <param name="theta"></param>
    /// <param name="radius"></param>
    private void updateCircleGoalPos(float theta,  float radius)
    {
        float adjustmentVal = Random.Range(0, 0.25f);

        float xVal;
        float yVal = gameObject.transform.position.y;
        float zVal;

        if (adjustmentVal < 0.125f)
        {
            xVal = nest.transform.position.x + (radius * Mathf.Cos(theta)) - adjustmentVal;
            zVal = nest.transform.position.z + (radius * Mathf.Sin(theta)) - adjustmentVal;
        }
        else
        {
            xVal = nest.transform.position.x + (radius * Mathf.Cos(theta)) + adjustmentVal;
            zVal = nest.transform.position.z + (radius * Mathf.Sin(theta)) + adjustmentVal;
        }

        goalPos = new Vector3(xVal, yVal, zVal);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="closest"></param>
    private void updateChasingGoalPos(GameObject closest)
    {
        if(gameObject.transform.position != closest.transform.position)
        {
            goalPos = closest.transform.position;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="goalPos"></param>
    private void movement()
    {
        Vector3 direction = goalPos - gameObject.transform.position;
        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation,
                                                         Quaternion.LookRotation(direction),
                                                         turningSpeed * Time.deltaTime);

        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    private float getCirclingRadius()
    {
        //definitions of the points to create the vector to rotate around
        float xPos = nestMeshCollider.transform.position.x;
        float yPos = gameObject.transform.position.y;
        float zPos = nestMeshCollider.transform.position.z;

        Vector3 rotationPoint = new Vector3(xPos, yPos, zPos);

        return Vector3.Distance(gameObject.transform.position, rotationPoint);
    }



    /// <summary>
    ///     Checks the angle between the triggerfish and the closest object within 
    ///     the triggerfish's territory
    /// </summary>
    /// <param name="closestObj">
    ///     the closest object to triggerfish within the nest
    /// </param>
    /// <returns>
    ///     The angle between the closest object and th
    /// </returns>
    private bool checkInVision(GameObject closestObj)
    {
        if (closestObj != null)
        {
            float angle = Vector3.Angle(gameObject.transform.position, closestObj.transform.position);

            if (angle <= 30 || angle >= 330)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    ///     Checks through a list of objects that should be parsed in from the NestManager 
    ///     attached to the triggerfish's nest 
    /// </summary>
    /// <param name="objects">
    ///     The list of objects to check for the nearest objects in
    /// </param>
    /// <returns>
    ///     Returns the closest gameObject to the triggerfish
    /// </returns>
    private GameObject getNearestObj(List<GameObject> objects)
    {
        GameObject closest = null;
        float closestDistance = 0.0f;

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(gameObject.transform.position, obj.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closest = obj;
            }
        }
        return closest;
    }
}
