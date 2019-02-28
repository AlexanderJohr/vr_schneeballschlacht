using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{

    public Transform[] targets;
    public float speed;
    public float roationSpeed;


    private int current;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != targets[current].position)
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, targets[current].position, speed*Time.deltaTime);
            GetComponent<Rigidbody>().MovePosition(pos);
            Quaternion rotation =  Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targets[current].position - transform.position, Vector3.up), roationSpeed * Time.deltaTime);
            GetComponent<Rigidbody>().MoveRotation(rotation);

        }
        else
        {
            current = (current + 1) % targets.Length;
        }
    }
}
