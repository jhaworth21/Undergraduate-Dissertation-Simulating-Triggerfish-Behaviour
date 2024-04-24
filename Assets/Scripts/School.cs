using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class School : MonoBehaviour
{
    float speed;
    bool turning;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(SchoolManager.SM.minSpeed, SchoolManager.SM.maxSpeed);

    }

    // Update is called once per frame
    void Update()
    {
        Bounds b = new Bounds(SchoolManager.SM.transform.position, SchoolManager.SM.schoolLimit * 2);

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
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  SchoolManager.SM.rotationSpeed * Time.deltaTime);
            Debug.Log("Transform Rotation = " +  transform.rotation);
        }
        else
        {
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(SchoolManager.SM.minSpeed, SchoolManager.SM.maxSpeed);
                ApplyRules();
            }
        }
        gameObject.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void ApplyRules()
    {
        GameObject[] gos;
        gos = SchoolManager.SM.allFish;

        Vector3 vcenter = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, gameObject.transform.position);
                if (nDistance <= SchoolManager.SM.neighbourDistance)
                {
                    vcenter += go.transform.position;
                    groupSize++;

                    if (nDistance < 1.0f)
                    {
                        vavoid = vavoid + (gameObject.transform.position - go.transform.position);
                    }

                    School anotherFlock = go.GetComponent<School>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }


        if (groupSize > 0)
        {
            vcenter = vcenter / groupSize + (SchoolManager.SM.goalPos - gameObject.transform.position);
            speed = gSpeed / groupSize;
            if (speed > SchoolManager.SM.maxSpeed)
            {
                speed = SchoolManager.SM.maxSpeed;
            }

            Vector3 direction = (vcenter + vavoid) - transform.position;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(direction),
                                                      SchoolManager.SM.rotationSpeed * Time.deltaTime);



        }
    }
}

