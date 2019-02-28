using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateGameOverScreen : MonoBehaviour {

    public BallDatabase ballDatabase;

    void Update()
    {
        if (ballDatabase.Health > 0.0f)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
