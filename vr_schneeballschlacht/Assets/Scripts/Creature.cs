using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

	[Header("Creature")]
	public float healthpoints;

	// Set a new health of the creature
	/* TODO Platzhalter Ende 
	 */
	public void setHealthPoints(float damage){
		Debug.Log ("I get here");
		this.healthpoints -= damage;
		if (this.healthpoints <= 0) {
			Debug.Log (this + "is Dead!");
			Destroy (this);
			Destroy (gameObject);
		}
	}

}
