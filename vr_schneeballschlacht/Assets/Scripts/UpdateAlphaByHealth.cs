using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateAlphaByHealth : MonoBehaviour {
    public BallDatabase ballDatabase;
    Renderer rend;

    void Start () {
        Renderer rend = GetComponent<Renderer>();
    }
	
	void Update () {
        rend.material.color = new Color(0f, 0f, 0f, 1f - ballDatabase.Health);
    }
}
