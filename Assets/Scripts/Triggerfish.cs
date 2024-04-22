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
    public float probabilityAdjustment = 0.1f;
    public float stateChangeThreshold = 0.5f;
    private float baseStateChangeProbability;


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
    private List<GameObject> inNest;
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
        //setup of initial parameters
        state = updateState();
        circlingAngle = 0;
        currentlyChased = null;
        
        //initialisation of the territory components
        nestMeshCollider = nest.GetComponent<MeshCollider>();
        patrolAreaCollider = patrolArea.GetComponent<MeshCollider>();
        nestManager = nest.GetComponent<NestManager>();


        //sets the lowest point that the fish can go to 
        worldFloor = GameObject.Find("Ocean Floor");
        floorBoundary = worldFloor.GetComponent<Transform>().position.y;

        //inital value of the time since last state change
        timeSinceLastStateChange = 0;
        baseStateChangeProbability = Random.Range(0, 1f);
    }

    // Update is called once per frame
    void Update()
    {

        //gets the objects in nest and the nearest of these
        inNest = nestManager.getObjectsInNest();
        closest = getNearestObj(inNest);

        //if the chasing conditions are met
        if (inNest.Count != 0 && checkInVision(closest)) //&& currentlyChased == null)
        {
            //set state to chasing and set currently chased to closest
            state = State.Chasing;
            currentlyChased = closest;
        }

        //if state isn't in chasing (ie passive state)
        else if (state != State.Chasing) 
        {
            //resets the distance chased and currently chased - not in chasing any more
            distanceChased = 0;
            currentlyChased = null;

            //checks if triggerfish is within 
            if(!patrolAreaCollider.bounds.Contains(gameObject.transform.position))
            {
                //sets state to returning
                state = State.Returning;
                //generates new goal position back in the patrol area
                goalPos = generateGoalPos(patrolAreaCollider.bounds, floorBoundary, goalPos);
            }

            //TODO - adjust so that higher probability not just given threshold
            float adjustedProbability = baseStateChangeProbability * (timeSinceLastStateChange * probabilityAdjustment);
            Debug.Log("adjusted probabilty = " + adjustedProbability);
            if (adjustedProbability > stateChangeThreshold || 
                (patrolAreaCollider.bounds.Contains(gameObject.transform.position) && lastState == State.Returning))
            {
                //updates the state to randomly be circling or patrolling
                state = updateState();
            }

            //checks if fish is patrolling
            if (state == State.Patrolling)
            {
                //if fish is near the goal position or was previously in a different state, adjust goal position
                if (Vector3.Distance(gameObject.transform.position, goalPos) < 1 || lastState != State.Patrolling)
                {
                    goalPos = generateGoalPos(patrolAreaCollider.bounds, floorBoundary, goalPos);
                }
            }

            //checks if fish is cirlcing
            if (state == State.Circling)
            {
                //if not previously circling, sets the radius of the circular path
                if (lastState != State.Circling)
                {
                    circlingRadius = Mathf.Abs(getCirclingRadius());
                }
                //updates the cureent angle based on the current speed and adjusts this to the time change
                circlingAngle += (lastState == state) ? speed * Time.deltaTime : 0;
                goalPos = updateCircleGoalPos(circlingAngle, circlingRadius);
            }
        }

        //checks if state is chasing
        if(state == State.Chasing)
        {
            //if chased for 15m, has reached the position or chased object is deleted
            float distnceFromChased = Vector3.Distance(gameObject.transform.position, goalPos);
            if (distanceChased > 15 || 
                distanceChased <= gameObject.GetComponent<MeshCollider>().bounds.extents.z || 
                !currentlyChased.activeInHierarchy)
            {
                //updates the state to a passive state
                state = updateState();
            }
            //updates the distance chased (from center of nest)
            distanceChased = Vector3.Distance(new Vector3(
                                                          nest.transform.position.x, 
                                                          gameObject.transform.position.y, 
                                                          nest.transform.position.z),
                                              gameObject.transform.position);

            //updates the goal position to the new position of chased object
            goalPos = currentlyChased.transform.position;
 
        }

        //applies the movement rules and updates current state values
        movement();
        timeSinceLastStateChange += Time.deltaTime;
        lastState = state;
    }

    /// <summary>
    ///     Updates the current state to be one of the passive states with a 0.5 probability
    /// </summary>
    /// <returns>
    ///     The state picked off of the random change 
    /// </returns>
    private State updateState()
    {
        //generates a random number between 0 and 1
        float stateProbability = Random.Range(0, 1f);
        Debug.Log("probability for state change = " + stateProbability);

        //changes state value depending on the value above
        State newState = (stateProbability <= 0.5f) ? State.Patrolling : State.Circling;
        //Randomly change speed on passive state change if state changes
        speed = (Random.Range(minSpeed, maxSpeed * passiveLimiter));

        baseStateChangeProbability = Random.Range(0, 1f);
        timeSinceLastStateChange = 0;

        return newState;
    }

    /// <summary>
    ///     Used to generate a random position from bounds that are parsed into it
    /// </summary>
    /// <param name="bounds">
    ///     The bounds for which the point is to be generated within
    /// </param>
    /// <returns>
    ///     A Vector3 representing the goal postion generated from the bounds
    /// </returns>
    private Vector3 generateGoalPos(Bounds bounds, float floorBoundary, Vector3 previousGoal)
    {
        //sets the bounds to generate the position within
        Vector3 upperBound = bounds.max;
        Vector3 lowerBound = bounds.min;
        float xVal;
        float yVal;
        float zVal;

        //if previously in different state or starting state is set to patrolling
        if (previousGoal == Vector3.zero || lastState != State.Patrolling)
        {
            //generate new goal position 
            xVal = Random.Range(upperBound.x, lowerBound.x);
            yVal = Random.Range(upperBound.y - 2, floorBoundary);
            zVal = Random.Range(upperBound.z, lowerBound.z);
        }
        else
        {
            //otherwise makes small adjustments to the goal position
            xVal = previousGoal.x + Random.value * goalAdjustment;
            yVal = previousGoal.y + Random.value * goalAdjustment;
            zVal = previousGoal.z + Random.value * goalAdjustment;
        }

        return new Vector3(xVal, yVal, zVal);
    }

    /// <summary>
    ///     Updates the goal position from the circling state so that the position follows a circular path
    /// </summary>
    /// <param name="theta">
    ///     The current angle for the point to (based on starting angle)
    /// </param>
    /// <param name="radius">
    ///     The radius of the circle that the point is circling around
    /// </param>
    /// <returns>
    ///     The new position as a Vector3
    /// </returns>
    private Vector3 updateCircleGoalPos(float theta,  float radius)
    {
        //calculates the new positon based of the parametric equation of a circle
        float xVal = nest.transform.position.x + (radius * Mathf.Cos(theta)  * Time.deltaTime);
        float yVal = gameObject.transform.position.y;
        float zVal = nest.transform.position.z + (radius * Mathf.Sin(theta)) * Time.deltaTime; 

        return new Vector3(xVal, yVal, zVal);
    }

    /// <summary>
    ///     Apply the movement rules
    /// </summary>
    private void movement()
    {
        //gets the direction between the Triggerfish and the goal position
        Vector3 direction = goalPos - gameObject.transform.position;
        //rotates the Triggerfish object to face the goal position
        gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation,
                                                         Quaternion.LookRotation(direction),
                                                         turningSpeed * Time.deltaTime);

        //cretes variable for acceleration adjustments
        float adjustment = speed * accelerationFactor * Time.deltaTime;

        if (state == State.Chasing)
        {
            //adjusts speed based on standard max speed
            //speed = (speed < maxSpeed) ? speed + adjustment :
            //            (speed >= maxSpeed) ? speed - adjustment : speed;
            if (speed < maxSpeed) { speed += adjustment; }
            else { speed -= adjustment; }
        }
        else
        {
            //sets the passive limit for maxSpeed reachable
            float passiveMax = maxSpeed * passiveLimiter;

            //adjusts speed based on passive limited speed
            if (speed < passiveMax) { speed += adjustment; }
            else { speed -= adjustment; }
            //speed = (speed < passiveMax) ? speed + adjustment :
            //            (speed >= passiveMax) ? speed - adjustment : speed;
        }
        //adjusts the position of the fish model in the scene
        gameObject.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    private float getCirclingRadius()
    {
        //definitions of the points to create the vector to rotate around

        float height = nest.GetComponent<MeshRenderer>().bounds.extents.y;

        float xPos = nest.transform.position.x;
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
