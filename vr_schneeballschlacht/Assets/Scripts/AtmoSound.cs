using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmoSound : MonoBehaviour {

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
