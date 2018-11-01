using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snow : MonoBehaviour {

	// public variables
	public GameObject _prefabSnow;


	// private variables


	// unity methods


	// methods

	public void TakeSnow(GameObject parent){
		var snow = Instantiate (_prefabSnow, parent.transform.position, Quaternion.identity);
		snow.transform.SetParent (parent.transform);
	}

	// getter / setter

	/*
	 *  Überprüfung ob der Controller sich in einem Bestimmenten Bereich findet (touchObject with Tag)
	 *  Setze das neue Object so, dass es als Gegriffen zählt (isGrab = true)
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 * 
	 */
}
