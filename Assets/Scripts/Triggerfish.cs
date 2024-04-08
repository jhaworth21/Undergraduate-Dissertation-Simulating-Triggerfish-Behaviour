using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField]
    private float circlingAngle;
    
    //header section for the tunable parameters not relating to movement
    [Header("Tunable Parameters")]
    public float goalAdjustment = 1f;
    public float stateAdjustment = 0.5f;

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
        circlingAngle = 0;
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

        if (inTerritory != null && checkInVision(closest))
        {
            state = State.Chasing;
            chasingMovement(closest);
        }
        else
        {
            if(timeSinceLastStateChange > stateAdjustment)
            {
                float stateProbability = Random.Range(0, 1);
                state = (stateProbability < 0.5f) ? State.Patrolling : State.Circling;
            }
            else if (state == State.Patrolling)
            {
                if (Vector3.Distance(gameObject.transform.position, goalPos) < 1)
                {
                    goalPos = generateGoalPos(nestMeshCollider.bounds, floorBoundary, goalPos);
                }
                movement();
            }
            else if (state == State.Circling)
            {
                circlingAngle  = (circlingAngle > 360) ? 0 : circlingAngle;
                circlingAngle += (lastState == state) ? speed : 0;

                updateCircleGoalPos(circlingAngle, getCirclingRadius());
                movement();
            }
        }

        timeSinceLastStateChange += Time.deltaTime;
        lastState = state;
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
        float adjustmentVal = Random.Range(0, 1);

        float xVal;
        float yVal;
        float zVal;

        if (adjustmentVal < 0.5f)
        {
            xVal = radius * Mathf.Cos(theta) - adjustmentVal;
            yVal = this.transform.position.y - adjustmentVal;
            zVal = radius * Mathf.Sin(theta) - adjustmentVal;
        }
        else
        {
            xVal = radius * Mathf.Cos(theta) + adjustmentVal;
            yVal = this.transform.position.y + adjustmentVal;
            zVal = radius * Mathf.Sin(theta) + adjustmentVal;
        }

        goalPos = new Vector3(xVal, yVal, zVal);
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
    }

    // TODO - check to make sure this makes sense
    private void chasingMovement(GameObject closest)
    { 
        goalPos = closest.transform.position;

        Vector3 direction = goalPos - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                                Quaternion.LookRotation(direction),
                                                turningSpeed * Time.deltaTime);

        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    private float getCirclingRadius()
    {
        //definitions of the points to create the vector to rotate around
        float xPos = this.nestMeshCollider.transform.position.x;
        float yPos = this.transform.position.y;
        float zPos = this.nestMeshCollider.transform.position.z;

        Vector3 rotationPoint = new Vector3(xPos, yPos, zPos);

        return Vector3.Distance(this.transform.position, rotationPoint);
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
