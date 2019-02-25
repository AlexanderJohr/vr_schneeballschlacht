using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class NetworkPlayer : Photon.MonoBehaviour {

    public GameObject _prefabSnowball;
    GameObject newBall;
    PhotonView photonView;

    public GameObject playerAvatar;
    public GameObject leftHandAvatar;
    public GameObject rightHandAvatar;

    GameObject leftHandVR;
    GameObject rightHandVR;
    GameObject head;

    // Use this for initialization
    void Start () {
        Debug.Log(_prefabSnowball);
        photonView = GetComponent<PhotonView>();
        leftHandVR = GameObject.Find("Player/Controller (left)");
        rightHandVR = GameObject.Find("Player/Controller (right)");
        head = GameObject.Find("Player/Camera");

        rightHandVR.transform.position = rightHandAvatar.transform.position;
        leftHandVR.transform.position = leftHandAvatar.transform.position;
        head.transform.position = playerAvatar.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if (newBall != null) {
            Debug.Log(newBall.GetComponent<Rigidbody>().velocity);
        }

        if (photonView.isMine)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("A was pressed");
                newBall = (GameObject)PhotonNetwork.Instantiate("Snowball2", new Vector3 (10,10,10), Quaternion.identity, 0);
                //newBall.GetComponent<Snowball2>().ballCollection = GameObject.Find("BallCollectionHi").GetComponent<BallCollection>();
            }


            playerAvatar.transform.position = head.transform.position;
            playerAvatar.transform.rotation = head.transform.rotation;

            leftHandAvatar.transform.position = leftHandVR.transform.position;
            leftHandAvatar.transform.rotation = leftHandVR.transform.rotation;

            rightHandAvatar.transform.position = rightHandVR.transform.position;
            rightHandAvatar.transform.rotation = rightHandVR.transform.rotation; 
        }
    }
}
