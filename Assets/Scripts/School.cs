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
        Bounds b = new Bounds(SchoolManager.SM.transform.position, SchoolManager.SM.swimLimits * 2);

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
        }
        else
        {
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(SchoolManager.SM.minSpeed, SchoolManager.SM.maxSpeed);
                ApplyRules();
            }
        }
        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
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
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (nDistance <= SchoolManager.SM.neighbourDistance)
                {
                    vcenter += go.transform.position;
                    groupSize++;

                    if (nDistance < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    School anotherSchool = go.GetComponent<School>();
                    gSpeed = gSpeed + anotherSchool.speed;
                }
            }
        }


        if (groupSize > 0)
        {
            vcenter = vcenter / groupSize + (SchoolManager.SM.goalPos - this.transform.position);
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
