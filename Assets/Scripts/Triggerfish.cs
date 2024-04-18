using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


//TODO - Comment code
//TODO - Fix issue where patrolling shows circling behaviour

public class Triggerfish : MonoBehaviour
{   
    //defines the possible states of the triggerfish
    private enum State { Chasing, Circling, Patrolling, Returning };

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
    [SerializeField]
    private float distanceChased;
    [SerializeField]
    private GameObject currentlyChased;
    [SerializeField]
    private float circlingAngle = 0f;
    [SerializeField]
    private float circlingRadius;
    
    //header section for the tunable parameters not relating to movement
    [Header("Tunable Parameters")]
    public float goalAdjustment = 1f;
    public float stateAdjustment = 5f;

    //header for the variables relating to the territory area and movement area
    [Header("Boundaries and Territory")]
    //variables for the floor of the world 
    public GameObject worldFloor;
    [SerializeField]
    private float floorBoundary;
    private NestManager nestManager;

    //variables for the nest and patrol areas (and mesh colliders)
    public GameObject nest;
    private MeshCollider nestMeshCollider;
    private GameObject closest;
    private List<GameObject> inTerritory;
    public GameObject patrolArea;
    private MeshCollider patrolAreaCollider;

    //header for the variables related to veclocity
    [Header("Velocity")]
    public float speed;
    public float turningSpeed;
    public float minSpeed;
    public float maxSpeed;
    public float accelerationFactor;
    public float passiveLimiter;

    //header for the variables related to vision
    [Header("Vision")]
    public float viewRange;
    public float viewAngle;


    // Start is called before the first frame update
    void Start()
    {
        state = updateState();
        circlingAngle = 0;
        currentlyChased = null;

        nestMeshCollider = nest.GetComponent<MeshCollider>();
        patrolAreaCollider = patrolArea.GetComponent<MeshCollider>();
        nestManager = nest.GetComponent<NestManager>();


        //sets the lowest point that the fish can go to 
        worldFloor = GameObject.Find("Ocean Floor");
        floorBoundary = worldFloor.GetComponent<Transform>().position.y;

        timeSinceLastStateChange = 0;
    }

    // Update is called once per frame
    void Update()
    {
        inTerritory = nestManager.getObjectsInTerritory();
        closest = getNearestObj(inTerritory);

        if (inTerritory.Count != 0 && checkInVision(closest) && currentlyChased == null)
        {
            state = State.Chasing;
            currentlyChased = closest;
        }
        else if (state != State.Chasing) 
        {
            distanceChased = 0;
            currentlyChased = null;
            if(!patrolAreaCollider.bounds.Contains(gameObject.transform.position))
            {
                state = State.Returning;
                goalPos = generateGoalPos(patrolAreaCollider.bounds, floorBoundary, goalPos);
            }
            if(timeSinceLastStateChange > stateAdjustment || 
                (patrolAreaCollider.bounds.Contains(gameObject.transform.position) && lastState == State.Returning))
            {
                state = updateState();
            }
            if (state == State.Patrolling)
            {
                if (Vector3.Distance(gameObject.transform.position, goalPos) < 1 || lastState != State.Patrolling)
                {
                    goalPos = generateGoalPos(patrolAreaCollider.bounds, floorBoundary, goalPos);
                }
            }
            if (state == State.Circling)
            {
                if (lastState != State.Circling)
                {
                    circlingRadius = Mathf.Abs(getCirclingRadius());
                }
                //circlingAngle  = (circlingAngle > 360) ? 0 : circlingAngle;
                circlingAngle += (lastState == state) ? speed * Time.deltaTime : 0;
                goalPos = updateCircleGoalPos(circlingAngle, circlingRadius);
            }
        }
        if(state == State.Chasing)
        {
            if (distanceChased > 15 || goalPos == gameObject.transform.position || !currentlyChased.activeInHierarchy)
            {
                state = updateState();
            }
            distanceChased = Vector3.Distance(new Vector3(
                                                        nest.transform.position.x, 
                                                        gameObject.transform.position.y, 
                                                        nest.transform.position.z),
                                              gameObject.transform.position);

            updateChasingGoalPos(currentlyChased);
 
        }
        movement();
        timeSinceLastStateChange += Time.deltaTime;
        lastState = state;
    }

    /// <summary>
    /// 
    /// </summary>
    private State updateState()
    {
        float stateProbability = Random.Range(0, 1f);
        State newState = (stateProbability < 0.5f) ? State.Patrolling : State.Circling;
        timeSinceLastStateChange = 0;

        return newState;
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

        if (previousGoal == Vector3.zero || lastState != State.Patrolling)
        {
            xVal = Random.Range(upperBound.x, lowerBound.x);
            yVal = Random.Range(upperBound.y - 2, floorBoundary);
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
    private Vector3 updateCircleGoalPos(float theta,  float radius)
    {
        float xVal = nest.transform.position.x + (radius * Mathf.Cos(theta)  * Time.deltaTime);
        float yVal = gameObject.transform.position.y;
        float zVal = nest.transform.position.z + (radius * Mathf.Sin(theta)) * Time.deltaTime; 

        return new Vector3(xVal, yVal, zVal);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="closest"></param>
    private void updateChasingGoalPos(GameObject closest)
    {
        if(gameObject.transform.position != closest.transform.position && closest != null)
        {
            goalPos = closest.transform.position;
        }
        else
        {
            updateState();
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
        if (state != State.Chasing)
        {
            //speed = (speed >= passiveLimiter * maxSpeed) ? speed - (speed * accelerationFactor) : Random.Range(minSpeed, passiveLimiter * maxSpeed);
            //speed = (speed < passiveLimiter * maxSpeed)  ? speed + (speed * accelerationFactor) : Random.Range(minSpeed, passiveLimiter * maxSpeed);
            speed = maxSpeed;
            this.transform.Translate(0, 0, passiveLimiter * speed * Time.deltaTime);
        }
        else 
        {
            speed = (speed >= maxSpeed) ? speed - (speed * accelerationFactor) : speed;
            speed = (speed < maxSpeed)  ? speed + (speed * accelerationFactor) : speed;
            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }

    private float getCirclingRadius()
    {
        //definitions of the points to create the vector to rotate around

        float height = nest.GetComponent<MeshRenderer>().bounds.extents.y;

        float xPos = nest.transform.position.x;
        //float xPos = ((height - nest.transform.position.y) / height) * 
        float yPos = gameObject.transform.position.y;
        float zPos = nest.transform.position.z;

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
            float angle = Vector3.Angle(gameObject.transform.forward, closestObj.transform.position);
            float distance = Vector3.Distance(gameObject.transform.position, closestObj.transform.position);
            if (angle <= viewAngle && distance <= viewRange)
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

        if(objects.Count > 0)
        {
            foreach (GameObject obj in objects)
            {
                float distance = Vector3.Distance(gameObject.transform.position, obj.transform.position);
                if (distance < closestDistance || closest == null)
                {
                    closestDistance = distance;
                    closest = obj;
                }
            }
            return closest;
        }
        return null;
    }
}
