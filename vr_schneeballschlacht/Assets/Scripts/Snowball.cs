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
    private SphereCollider myCollider;
    public float rotateRay;

    // is this snowball actual a cover
    private bool isCover;
	// should a cover but is in air
	private bool checkSnowball;
	// the rigidbody from this snowball
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
        myCollider = GetComponent<SphereCollider>();
        rotateRay = 45;
    }
	
	// Update is called once per frame
	void Update () {
        if (refCenter == null && (refLeft == null || refRight == null))
        {
            ExpandCover();
        }
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
        float height = myCollider.radius + 0.1f;
		centerPosition = transform.position;
        Vector3 centerDirection = Vector3.down * height;
        Vector3 leftDirection = Quaternion.AngleAxis(-rotateRay, Vector3.forward) * centerDirection;
        Vector3 rightDirection = Quaternion.AngleAxis(rotateRay, Vector3.forward) * centerDirection;

        Debug.DrawRay(centerPosition, centerDirection, Color.blue);
        Debug.DrawRay(centerPosition, leftDirection, Color.red);
        Debug.DrawRay(centerPosition, rightDirection, Color.green);

		RaycastHit hit;
		if (Physics.Raycast (centerPosition, centerDirection, out hit, height)) {
			refCenter = hit.collider.gameObject;
			GroundCover ();
		} else if (Physics.Raycast (centerPosition, leftDirection, out hit, height)) {
			refLeft = hit.collider.gameObject;
			if (Physics.Raycast (centerPosition, rightDirection, out hit, height)) {
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
		rb.isKinematic = true;
		rb.constraints = RigidbodyConstraints.FreezeAll;
		isCover = true;
		checkSnowball = false;
	}

	void ExpandCover(){
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        isCover = false;
        checkSnowball = true; 
	}
}
