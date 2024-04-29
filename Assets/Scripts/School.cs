using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class School : MonoBehaviour
{
    float speed;
    bool turning = false;

    void Start()
    {

        speed = Random.Range(SchoolManager.SM.minSpeed, SchoolManager.SM.maxSpeed);
    }


    void Update()
    {

        Bounds b = new Bounds(SchoolManager.SM.transform.position, SchoolManager.SM.schoolLimits * 2.0f);

        if (!b.Contains(transform.position))
        {

            turning = true;
        }
        else
        {

            turning = false;
        }

        if (turning)
        {

            Vector3 direction = SchoolManager.SM.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                speed * Time.deltaTime);
        }
        else
        {


            if (Random.Range(0, 100) < 10)
            {

                speed = Random.Range(SchoolManager.SM.minSpeed, SchoolManager.SM.maxSpeed);
            }


            if (Random.Range(0, 100) < 10)
            {
                ApplyRules();
            }
        }

        this.transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
    }

    private void ApplyRules()
    {

        GameObject[] gos;
        gos = SchoolManager.SM.allFish;

        Vector3 vCentre = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;

        float gSpeed = 0.01f;
        float mDistance;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {

            if (go != this.gameObject)
            {

                mDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (mDistance <= SchoolManager.SM.neighbourDistance)
                {

                    vCentre += go.transform.position;
                    groupSize++;

                    if (mDistance < 1.0f)
                    {

                        vAvoid = vAvoid + SchoolManager.SM.separationWeighting * (this.transform.position - go.transform.position);
                    }

                    School anotherSchool = go.GetComponent<School>();
                    gSpeed = gSpeed + anotherSchool.speed;
                }
            }
        }

        if (groupSize > 0)
        {

            vCentre = vCentre / groupSize + (SchoolManager.SM.goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            if (speed > SchoolManager.SM.maxSpeed)
            {

                speed = SchoolManager.SM.maxSpeed;
            }

            Vector3 direction = (vCentre + vAvoid) - transform.position;
            if (direction != Vector3.zero)
            {

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    SchoolManager.SM.rotationSpeed * Time.deltaTime);
            }
        }
    }
    //float speed;
    //bool turning;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    speed = Random.Range(SchoolManager.SM.minSpeed, SchoolManager.SM.maxSpeed);

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    Bounds b = new Bounds(SchoolManager.SM.transform.position, SchoolManager.SM.schoolLimit);

    //    if (!b.Contains(transform.position))
    //    {
    //        turning = true;
    //    }
    //    else
    //    {
    //        turning = false;
    //    }

    //    if (turning)
    //    {
    //        Vector3 direction = SchoolManager.SM.transform.position - transform.position;
    //        transform.rotation = Quaternion.Slerp(transform.rotation,
    //                                              Quaternion.LookRotation(direction),
    //                                              SchoolManager.SM.rotationSpeed * Time.deltaTime);
    //        Debug.Log("Transform Rotation = " +  transform.rotation);
    //    }
    //    else
    //    {
    //        if (Random.Range(0, 100) < 10)
    //        {
    //            speed = Random.Range(SchoolManager.SM.minSpeed, SchoolManager.SM.maxSpeed);
    //            ApplyRules();
    //        }
    //    }
    //    gameObject.transform.Translate(0, 0, speed * Time.deltaTime);
    //}

    ///// <summary>
    ///// Applies the boids model ruleset to the movement of the gameObject
    ///// </summary>
    //void ApplyRules()
    //{
    //    //gets all of the fish in the school
    //    GameObject[] allFish;
    //    allFish = SchoolManager.SM.allFish;

    //    //initialises the center and avoidance vectors
    //    Vector3 vcenter = Vector3.zero;
    //    Vector3 vavoid = Vector3.zero;

    //    //sets a base value for the group speed and size
    //    float gSpeed = 0.01f;
    //    int groupSize = 0;

    //    //creates the varaible representing neighbour distance
    //    float nDistance;

    //    //loops through all fish in the school
    //    foreach (GameObject fish in allFish)
    //    {
    //        //if not current fish
    //        if (fish != gameObject)
    //        {
    //            //gets the distance between current fish and currently fish at position in the loop
    //            nDistance = Vector3.Distance(fish.transform.position, gameObject.transform.position);
    //            //comparison against neighbour distance limit
    //            if (nDistance <= SchoolManager.SM.neighbourDistance)
    //            {
    //                //adds the position to the center vector based on weighting value
    //                vcenter += SchoolManager.SM.cohesionWeighting * fish.transform.position;
    //                groupSize++;

    //                //if too close
    //                if (nDistance < 1.0f)
    //                {
    //                    //adds the difference to avoidance vector with weighting applied
    //                    vavoid = vavoid * SchoolManager.SM.separationWeighting + (gameObject.transform.position - fish.transform.position);
    //                }

    //                //gets the other schools in the overall swarm
    //                School anotherSchool = fish.GetComponent<School>();
    //                gSpeed = gSpeed + anotherSchool.speed;
    //            }
    //        }
    //    }


    //    //if school has any fish
    //    if (groupSize > 0)
    //    {
    //        //center is the average position
    //        vcenter = vcenter / groupSize + (SchoolManager.SM.goalPos - gameObject.transform.position);
    //        //average the speed across the school
    //        speed = gSpeed / groupSize;
    //        //resets speed to max if max exceeded
    //        if (speed > SchoolManager.SM.maxSpeed)
    //        {
    //            speed = SchoolManager.SM.maxSpeed;
    //        }

    //        //creates the direction vector to control the forward movement of the fish
    //        Vector3 direction = (vcenter + vavoid) * SchoolManager.SM.alignmentWeighting - transform.position;
    //        if (direction != Vector3.zero)
    //            transform.rotation = Quaternion.Slerp(transform.rotation,
    //                                                  Quaternion.LookRotation(direction),
    //                                                  SchoolManager.SM.rotationSpeed * Time.deltaTime);
    //    }
    //}
}

