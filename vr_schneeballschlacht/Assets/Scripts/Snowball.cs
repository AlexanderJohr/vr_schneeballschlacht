using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Todo Referenzen und Stats beim erstellen setzen
*/

public class Snowball : Creature {

	[Header("Stats")]
	public float attack;
	private BodyShape bodyShape;

	[Header("References")]
	public GameObject player;
	public GameObject field;

	public GameObject refCenter;
	public GameObject refLeft;
	public GameObject refRight;

	// Calculate the touchpoints
	private Vector3 centerPosition;
	private Vector3 leftPosition;
	private Vector3 rightPosition;
	private float width;
	private float height;

	// is this snowball actual a cover
	private bool isCover;
	// should a cover but is in air
	private bool checkSnowball;
	// the rigidbody from this snowball
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		// Get the width of this snowball
		width = transform.localScale.x;
		// Get the hight of this snowball
		height =  (transform.localScale.y /2) * 1.1f ;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		if (checkSnowball) {
			switch (bodyShape) {
			case BodyShape.Sphere:
				LookForSphere();
				break;
			case BodyShape.Cuboid:
			//	LookForCuboid();
				break;
			default:
				Debug.LogError ("This body not exist");
				break;
			}
		}
	}

	// Check the object which hit the snowball
	void OnCollisionEnter(Collision collision){
		Debug.Log (collision.gameObject.name);
		// check is this snowball a cover or not
		if (!isCover) {
			switch (collision.gameObject.tag) {
				// The snowball hit a player
				case "Player":
					InterActWithPlayer (collision.gameObject);
					break;
				// The snowball hit a field
				case "Field":
					InterActWithField (collision.gameObject);
					break;
				// The snowball hit a other Snowball
				case "Snowball":
					InterActWithSnowball (collision.gameObject);
					break;
				default:
					Debug.LogError ("Something goes wrong");
					break;
				
			}
		}
	}

	void LookForSphere(){
		Vector3 directionX = new Vector3(width/2,0,0);
		Vector3 directionY = Vector3.down * height/2;
		centerPosition = transform.position;
		leftPosition = centerPosition - directionX + directionY;
		rightPosition = centerPosition + directionX + directionY;

		Debug.DrawRay(centerPosition,Vector3.down * height,Color.blue);
		Debug.DrawRay(centerPosition,leftPosition - centerPosition,Color.red);
		Debug.DrawRay(centerPosition,rightPosition - centerPosition,Color.green);

		RaycastHit hit;
		if (Physics.Raycast (centerPosition, Vector3.down * height, out hit, height)) {
			refCenter = hit.collider.gameObject;
			GroundCover ();
		} else if (Physics.Raycast (centerPosition, leftPosition - centerPosition, out hit, height)) {
			refLeft = hit.collider.gameObject;
			if (Physics.Raycast (centerPosition, rightPosition - centerPosition, out hit, height)) {
				refRight = hit.collider.gameObject;
				GroundCover ();
			} else {
				refLeft = null;
			}
		}
	}

	void InterActWithPlayer(GameObject other){
		if (!GameObject.ReferenceEquals (other, this.player)) {
			var otherPlayer = other.GetComponent<Player> ();
			otherPlayer.setHealthPoints (this.attack);
		} 
		Destroy (gameObject);
	}

	void InterActWithField(GameObject other){
		if (GameObject.ReferenceEquals (other, this.field)) {
			GroundCover ();
			refCenter = field;
		} else {
			Destroy (gameObject);
		}
	}

	void InterActWithSnowball(GameObject other){
		var otherSnowball= other.GetComponent<Snowball> ();
		if (GameObject.ReferenceEquals (otherSnowball.player, this.player)) {
			ExpandCover ();
		} else {
			otherSnowball.setHealthPoints (this.attack);
		}
	}

	void GroundCover(){
		rb.Sleep ();
		rb.isKinematic = true;
		rb.constraints = RigidbodyConstraints.FreezeAll;
		isCover = true;
		checkSnowball = false;
	}

	void ExpandCover(){
		checkSnowball = true; 
	}
}
