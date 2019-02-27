using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;

public class UnetNetworkPlayer : NetworkBehaviour
{
    public BallDatabase ballDatabase;


    public GameObject snowBallPrefab;


    public GameObject vrPlayArea;
    public GameObject nonVrCamera;


    public GameObject _prefabSnowball;
    GameObject newBall;

    public GameObject playerAvatar;
    public GameObject leftHandAvatar;
    public GameObject rightHandAvatar;

    public GameObject leftHandVR;
    public GameObject rightHandVR;
    public GameObject head;

    public float healtReplenishMultiplier = 1.0f;

    void Start()
    {
        BallsConnectedToGround = new List<Snowball2>();


        if (isLocalPlayer)
        {
            ballDatabase.Reset();

            if (XRSettings.enabled)
            {
                vrPlayArea.SetActive(true);

                foreach (Transform child in playerAvatar.transform)
                {
                    MeshRenderer renderer = child.gameObject.GetComponent<MeshRenderer>();
                    if (renderer != null) {
                        renderer.enabled = false;
                    }
                }
                foreach (Transform child in leftHandAvatar.transform)
                {
                    SkinnedMeshRenderer renderer = child.gameObject.GetComponent<SkinnedMeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.enabled = false;
                    }
                }
                foreach (Transform child in rightHandAvatar.transform)
                {
                    SkinnedMeshRenderer renderer = child.gameObject.GetComponent<SkinnedMeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.enabled = false;
                    }
                }
            }
            else
            {
                nonVrCamera.SetActive(true);
            }
        }
    }



    void Update()
    {
        if (isLocalPlayer)
        {
            if (ballDatabase.Health > 0)
            {
                ballDatabase.Health += Time.deltaTime * healtReplenishMultiplier;
            }

            if (XRSettings.enabled)
            {
                HandleVrInput();
            }
            else
            {
                HandleMouseKeyboardInput();
            }

            HandleCollisions();
        }
    }

    private void HandleVrInput()
    {
        playerAvatar.transform.position = head.transform.position;
        playerAvatar.transform.rotation = Quaternion.Euler(0, head.transform.rotation.eulerAngles.y, 0);

        leftHandAvatar.transform.position = leftHandVR.transform.position;
        leftHandAvatar.transform.rotation = leftHandVR.transform.rotation;

        rightHandAvatar.transform.position = rightHandVR.transform.position;
        rightHandAvatar.transform.rotation = rightHandVR.transform.rotation;
    }


    #region Mouse Keyboard Input
    private float ballToThrowScale = 1f;
    private float ballToThrowVelocityMultiplier = 10f;


    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private void HandleMouseKeyboardInput()
    {
        float xAxisValue = Input.GetAxis("Horizontal");
        float zAxisValue = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(xAxisValue, 0.0f, zAxisValue));

        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        if (Input.GetMouseButtonDown(0))
        {

        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            ballToThrowScale -= 0.05f;
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            ballToThrowScale += 0.05f;
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            ballToThrowVelocityMultiplier -= 3f;
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            ballToThrowVelocityMultiplier += 3f;
        }
        if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            var go = GameObject.Instantiate(snowBallPrefab);
            go.transform.position = nonVrCamera.transform.position + nonVrCamera.transform.forward * 2;
            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            rigidbody.velocity = nonVrCamera.transform.forward * ballToThrowVelocityMultiplier;
            go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Snowball2 BallInHand = go.GetComponent<Snowball2>();
            BallInHand.IsLocalBall = true;
            BallInHand.Scale = ballToThrowScale;
            BallInHand.Health = ballToThrowScale;

            SpawnBallOnOpponentsClient(BallInHand);
        }
    }

    public void SpawnBallOnOpponentsClient(Snowball2 BallInHand)
    {
        BallInHand.Id = ballDatabase.SnowBallIndex++;
        ballDatabase.PlayersBalls.Add(BallInHand.Id, BallInHand);
        CmdSpawnSnowBall(BallInHand.Id, BallInHand.transform.position, BallInHand.Rigidbody.velocity, BallInHand.UseGravity, BallInHand.Scale, BallInHand.Health);
    }
    #endregion

    [Command]
    public void CmdSpawnSnowBall(int id, Vector3 position, Vector3 velocity, bool useGravity, float scale, float health)
    {
        RpcSpawnSnowBall(id, position, velocity, useGravity, scale, health);
    }
    [ClientRpc]
    public void RpcSpawnSnowBall(int id, Vector3 position, Vector3 velocity, bool useGravity, float scale, float health)
    {
        if (!isLocalPlayer)
        {
            var go = GameObject.Instantiate(snowBallPrefab);

            go.transform.position = position;
            go.GetComponent<Rigidbody>().velocity = velocity;
            Snowball2 ball = go.GetComponent<Snowball2>();
            ball.UseGravity = useGravity;
            ball.IsLocalBall = false;

            ball.Scale = scale;
            ball.Health = health;
            ball.Id = id;

            ballDatabase.OpponentsBalls.Add(id, ball);
        }
    }

    [Command]
    public void CmdUpdateMySnowBall(int id, Vector3 position, Vector3 velocity, bool useGravity, float scale, float health)
    {
        RpcUpdateMySnowBall(id, position, velocity, useGravity, scale, health);
    }
    [ClientRpc]
    public void RpcUpdateMySnowBall(int id, Vector3 position, Vector3 velocity, bool useGravity, float scale, float health)
    {
        if (!isLocalPlayer)
        {
            if (ballDatabase.OpponentsBalls.ContainsKey(id))
            {
                Snowball2 ball = ballDatabase.OpponentsBalls[id];
                ball.transform.position = position;
                ball.Rigidbody.velocity = velocity;
                ball.Scale = scale;
                ball.Health = health;
                ball.UseGravity = useGravity;
            }
            else
            {
                Debug.Log("RpcUpdateMySnowBall OpponentsBalls did not contain the Key");
            }

        }
    }

    [Command]
    public void CmdDecreaseHealthOfOpponentsSnowBall(int id)
    {
        RpcDecreaseHealthOfOpponentsSnowBall(id);
    }
    [ClientRpc]
    public void RpcDecreaseHealthOfOpponentsSnowBall(int id)
    {
        if (!isLocalPlayer)
        {


            if (ballDatabase.PlayersBalls.ContainsKey(id))
            {
                Snowball2 opponentsBall = ballDatabase.PlayersBalls[id];
                opponentsBall.Health--;
            }
            else
            {
                Debug.Log(string.Format(
                    "RpcDecreaseHealthOfOpponentsSnowBall PlayersBalls did not contain the Key {0}", id));
            }

        }
    }

    [Command]
    public void CmdDeleteOpponentsSnowBall(int id)
    {
        RpcDeleteOpponentsSnowBall(id);
    }
    [ClientRpc]
    public void RpcDeleteOpponentsSnowBall(int id)
    {
        if (!isLocalPlayer)
        {
            if (ballDatabase.PlayersBalls.ContainsKey(id))
            {
                Snowball2 ball = ballDatabase.PlayersBalls[id];
                GameObject.Destroy(ball.gameObject);
                ballDatabase.PlayersBalls.Remove(id);
            }
            else
            {
                Debug.Log(string.Format(
                    "RpcDeleteOpponentsSnowBall PlayersBalls did not contain the Key {0}", id));
            }
        }
    }

    [Command]
    public void CmdDeleteMySnowBall(int id)
    {
        RpcDeleteSnowBall(id);
    }
    [ClientRpc]
    public void RpcDeleteSnowBall(int id)
    {
        if (!isLocalPlayer)
        {


            if (ballDatabase.OpponentsBalls.ContainsKey(id))
            {
                Snowball2 ball = ballDatabase.OpponentsBalls[id];
                GameObject.Destroy(ball.gameObject);
                ballDatabase.OpponentsBalls.Remove(id);
            }
            else
            {
                Debug.Log(string.Format(
                    "RpcDeleteSnowBall OpponentsBalls did not contain the Key {0}", id));
            }


        }
    }

    #region Ball Handling
    public Dictionary<int, Snowball2> PlayersBalls { get { return ballDatabase.PlayersBalls; } }
    public Dictionary<int, Snowball2> OpponentsBalls { get { return ballDatabase.OpponentsBalls; } }



    public const float snowballMergeThresholdMultiplier = 0.2f;

    //public List<Snowball2> Balls { get; set; }
    public List<Snowball2> BallsConnectedToGround { get; set; }

    public bool BallsAreCloseToEachOther
    {
        get
        {
            if (LeftHand != null && RightHand != null && LeftHand.BallInHand != null && RightHand.BallInHand != null)
            {

                Vector3 distanceOfBothBalls = LeftHand.BallInHand.transform.position - RightHand.BallInHand.transform.position;
                float radiusSumOfSnowballs = LeftHand.BallInHand.Scale / 2 + RightHand.BallInHand.Scale / 2;
                float snowballMergeThreshold = HandWithSmalestBall.BallInHand.Scale * snowballMergeThresholdMultiplier;

                float mergeDistance = radiusSumOfSnowballs - snowballMergeThreshold;

                return distanceOfBothBalls.magnitude < mergeDistance;
            }
            else
            {
                return false;
            }
        }
    }

    public static bool checkIfBallsAreCloseToEachOther(Snowball2 ball1, Snowball2 ball2)
    {
        Vector3 distanceOfBothBalls = ball1.transform.position - ball2.transform.position;
        float radiusSumOfSnowballs = ball1.Scale / 2 + ball2.Scale / 2;

        Snowball2 smallestBall = getSmallestBall(ball1, ball2);

        float snowballMergeThreshold = smallestBall.Scale * snowballMergeThresholdMultiplier;

        float mergeDistance = radiusSumOfSnowballs - snowballMergeThreshold;

        return distanceOfBothBalls.magnitude < mergeDistance;
    }

    public static Snowball2 getSmallestBall(Snowball2 ball1, Snowball2 ball2)
    {
        if (ball1.Scale < ball2.Scale)
        {
            return ball1;
        }
        else
        {
            return ball2;
        }
    }

    public Hand LeftHand { get; set; }
    public Hand RightHand { get; set; }
    public bool OneBallInHandIsBigger
    {
        get
        {
            return LeftHand.BallInHand.Scale != RightHand.BallInHand.Scale;
        }
    }
    public Hand HandWithSmalestBall
    {
        get
        {
            if (LeftHand.BallInHand.Scale < RightHand.BallInHand.Scale)
            {
                return LeftHand;
            }
            else
            {
                return RightHand;
            }
        }
    }

    public Hand HandWithBiggestBall
    {
        get
        {
            if (LeftHand.BallInHand.Scale < RightHand.BallInHand.Scale)
            {
                return RightHand;
            }
            else
            {
                return LeftHand;
            }
        }
    }

    private void HandleCollisions()
    {

        var myBalls = PlayersBalls.Select(d => d.Value).ToList();
        var opponentsBalls = OpponentsBalls.Select(d => d.Value).ToList();
        var allBalls = myBalls.Concat(opponentsBalls).ToList();
                     
        BallsConnectedToGround.Clear();
        for (int i = 0; i < myBalls.Count; i++)
        {
            var ball = myBalls[i];

            ball.reset();

            float distanceOfCenterToGround = ball.transform.position.y - 0.2f;

            float ballRadius = ball.Scale / 2;

            float mergeWithFloorThreshold = ballRadius * snowballMergeThresholdMultiplier;

            float snowballMergeRadius = ballRadius - mergeWithFloorThreshold;
            float distanceOfMergeRadiusToGround = distanceOfCenterToGround - snowballMergeRadius;

            bool isCloseToFloor = distanceOfMergeRadiusToGround < 0;

            if (isCloseToFloor)
            {
                ball.IsConnectedToGround = true;
                BallsConnectedToGround.Add(ball);
            }
            else
            {
                ball.IsConnectedToGround = false;
            }

            for (int j = 0; j < allBalls.Count; j++)
            {
                var otherBall = allBalls[j];
                var thisBall = ball;

                var thisBallIsntOtherBall = thisBall != otherBall;

                if (thisBallIsntOtherBall)
                {

                    bool ballsAreNotHeldInHand = thisBall.IsNotHeldInHand && otherBall.IsNotHeldInHand;

                    if (ballsAreNotHeldInHand)
                    {
                        bool ballsAreCloseToEachOther = checkIfBallsAreCloseToEachOther(thisBall, otherBall);
                        if (ballsAreCloseToEachOther)
                        {
                            bool hitMyBall = otherBall.IsLocalBall;
                            bool hitOpponentsBall = !otherBall.IsLocalBall;

                            if (hitMyBall)
                            {
                                bool velocityIsHigh = thisBall.Rigidbody.velocity.magnitude > 6;
                                if (velocityIsHigh)
                                {

                                    otherBall.Health--;

                                    if (otherBall.Health <= 0)
                                    {
                                        PlayersBalls.Remove(otherBall.Id);
                                        CmdDeleteMySnowBall(otherBall.Id);
                                        Object.Destroy(otherBall.gameObject);
                                    }

                                    CmdDeleteMySnowBall(thisBall.Id);
                                    PlayersBalls.Remove(thisBall.Id);
                                    Object.Destroy(thisBall.gameObject);
                                }
                                else
                                {
                                    thisBall.ConnectedBalls.Add(otherBall);
                                }
                            }
                            else if (hitOpponentsBall)
                            {
                                bool thisBallWasTheMovingOne = thisBall.UseGravity;
                                if (thisBallWasTheMovingOne)
                                {
                                    bool otherBallWasCover = !otherBall.UseGravity;

                                    if (otherBallWasCover)
                                    {
                                        otherBall.Health--;
                                        CmdDecreaseHealthOfOpponentsSnowBall(otherBall.Id);
                                    }
                                    CmdDeleteMySnowBall(thisBall.Id);
                                    PlayersBalls.Remove(thisBall.Id);
                                    Object.Destroy(thisBall.gameObject);
                                }
                                //  OpponentsBalls.Remove(otherBall.Id);
                                // CmdDeleteMySnowBall(otherBall.Id);
                            }



                        }
                    }
                }
            }
        }

        for (int i = 0; i < BallsConnectedToGround.Count; i++)
        {
            var ball = BallsConnectedToGround[i];
            for (int j = 0; j < ball.ConnectedBalls.Count; j++)
            {
                var connectedBall = ball.ConnectedBalls[j];

                bool connectedBallNotYetChecket = !connectedBall.IsConnectedToGround;
                if (connectedBallNotYetChecket)
                {
                    connectedBall.IsConnectedToGround = true;
                    BallsConnectedToGround.Add(connectedBall);
                }
            }
        }

        for (int i = 0; i < myBalls.Count; i++)
        {
            var ball = myBalls[i];
            if (ball.IsConnectedToGround)
            {
                if (ball.UseGravity != false)
                {
                    ball.UseGravity = false;
                    CmdUpdateMySnowBall(ball.Id, ball.transform.position, Vector3.zero, ball.UseGravity, ball.Scale, ball.Health);
                }
            }
            else
            {
                if (ball.UseGravity != true)
                {
                    ball.UseGravity = true;
                    CmdUpdateMySnowBall(ball.Id, ball.transform.position, ball.Rigidbody.velocity, ball.UseGravity, ball.Scale, ball.Health);
                }
            }

            if (ball.Health < 0.1)
            {
                Debug.Log(string.Format("ball.Health < 0;  ID {0}", ball.Id));
                CmdDeleteMySnowBall(ball.Id);
                PlayersBalls.Remove(ball.Id);
                Object.Destroy(ball.gameObject);
            }
        }

        for (int j = 0; j < opponentsBalls.Count; j++)
        {
            var opponentsBall = opponentsBalls[j];

            if (opponentsBall.UseGravity) {
                Vector3 distanceBetweenBallAndHead = opponentsBall.transform.position - head.transform.position;
                if(distanceBetweenBallAndHead.magnitude < 0.5f)
                {
                    ballDatabase.Health -= 0.2f;
                    CmdDeleteOpponentsSnowBall(opponentsBall.Id);
                    OpponentsBalls.Remove(opponentsBall.Id);
                    Object.Destroy(opponentsBall.gameObject);
                }
            }
        }

    }

    #endregion


}
