//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;
using Valve.VR;
using System.Linq;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class Hand : MonoBehaviour
{
    public GameObject prefab;
    public Rigidbody Rigidbody { get; private set; }

    private Snowball2 ballInHand;
    public Snowball2 BallInHand
    {
        get
        {
            return ballInHand;
        }
        set
        {
            if (value != null)
            {
                ballInHand = value;
                BallInHand.IsHeldInHand = true;
                ballInHand.gameObject.transform.position = attachPoint.transform.position;

                joint = ballInHand.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = attachPoint;
            }
            else
            {
                if (joint != null)
                {
                    Object.DestroyImmediate(joint);
                    joint = null;
                }
                BallInHand.IsHeldInHand = false;
                ballInHand = null;
            }

        }
    }

    public Rigidbody attachPoint;

    [SteamVR_DefaultAction("Interact")]
    public SteamVR_Action_Boolean spawn;

    protected SteamVR_Behaviour_Pose trackedObj;
    protected FixedJoint joint;

    public bool IsHoldingBall { get { return joint != null; } }
    public bool IsNotHoldingBall { get { return joint == null; } }

    public bool HoldInputPressed { get { return spawn.GetStateDown(trackedObj.inputSource); } }
    public bool HoldInputReleased { get { return spawn.GetStateUp(trackedObj.inputSource); } }

    public bool HandIsInSnow { get { return attachPoint.transform.position.y <= 0.2; } }

    public bool ThisIsTheLeftHand { get { return trackedObj.inputSource == SteamVR_Input_Sources.LeftHand; } }
    public bool ThisIsTheRightHand { get { return trackedObj.inputSource == SteamVR_Input_Sources.RightHand; } }

    public bool BallsAreCloseToEachOther { get { return player.BallsAreCloseToEachOther; } }


    public UnetNetworkPlayer player;

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_Behaviour_Pose>();
        if (ThisIsTheLeftHand)
        {
            player.LeftHand = this;
        }
        else if (ThisIsTheRightHand)
        {
            player.RightHand = this;
        }

    }

    private void FixedUpdate()
    {
        var balls = player.PlayersBalls.Select(d => d.Value).ToList();

        Snowball2 ballInGrabDistance = null;
        for (int i = 0; i < balls.Count; i++)
        {
            var ball = balls[i];
            Vector3 distanceToBallCenter = ball.transform.position - transform.position;
            float ballRadius = ball.Scale / 2;

            bool ballIsWithinGrabDistance = distanceToBallCenter.magnitude < ballRadius;

            if (ballIsWithinGrabDistance) {
                ballInGrabDistance = ball;
            }
        }


        if (IsNotHoldingBall && HoldInputPressed && HandIsInSnow)
        {
            CreateBallInHand();
        }
        else if (IsNotHoldingBall && HoldInputPressed && ballInGrabDistance != null)
        {
            GrabBall(ballInGrabDistance.gameObject);
        }
        else if (IsHoldingBall && HoldInputReleased)
        {
            ThrowBall();
        }
        if (BallsAreCloseToEachOther)
        {
            MergeBalls();
        }
    }
    public AudioSource[] snowballCrunchSounds;
    public AudioSource throwSound;


    private void MergeBalls()
    {
        Hand handWhereBallGoesTo;
        Hand handWhereBallWillBeDeleted;

        if (player.OneBallInHandIsBigger)
        {
            handWhereBallWillBeDeleted = player.HandWithSmalestBall;
            handWhereBallGoesTo = player.HandWithBiggestBall;
        }
        else
        {
            var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;

            float leftBallVelocity = origin.TransformVector(player.LeftHand.trackedObj.GetVelocity()).magnitude;
            float rightBallVelocity = origin.TransformVector(player.RightHand.trackedObj.GetVelocity()).magnitude;

            if (leftBallVelocity > rightBallVelocity)
            {
                handWhereBallWillBeDeleted = player.LeftHand;
                handWhereBallGoesTo = player.RightHand;
            }
            else
            {

                handWhereBallWillBeDeleted = player.RightHand;
                handWhereBallGoesTo = player.LeftHand;
            }
        }

        player.PlayersBalls.Remove(handWhereBallWillBeDeleted.BallInHand.Id);

        Object.Destroy(handWhereBallWillBeDeleted.BallInHand.gameObject);
        handWhereBallWillBeDeleted.BallInHand = null;
        handWhereBallGoesTo.BallInHand.IncreaseScale();
        handWhereBallGoesTo.BallInHand.Health += 0.4f;


        AudioSource snowballCrunchSound = snowballCrunchSounds[Random.Range(0, snowballCrunchSounds.Length)];
        snowballCrunchSound.Play();

    }

    private void ThrowBall()
    {
        throwSound.Play();
        var rigidbody = BallInHand.GetComponent<Rigidbody>();


        // We should probably apply the offset between trackedObj.transform.position
        // and device.transform.pos to insert into the physics sim at the correct
        // location, however, we would then want to predict ahead the visual representation
        // by the same amount we are predicting our render poses.
        
        var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
        if (origin != null)
        {
            rigidbody.velocity = origin.TransformVector(trackedObj.GetVelocity() / (BallInHand.Scale * 5));
            rigidbody.angularVelocity = origin.TransformVector(trackedObj.GetAngularVelocity());
        }
        else
        {
            rigidbody.velocity = trackedObj.GetVelocity() / (BallInHand.Scale * 5);
            rigidbody.angularVelocity = trackedObj.GetAngularVelocity();
        }

        rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;

       player.SpawnBallOnOpponentsClient(BallInHand);
        BallInHand = null;

    }

    private void CreateBallInHand()
    {
        var go = GameObject.Instantiate(prefab);
        go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        BallInHand = go.GetComponent<Snowball2>();
        BallInHand.IsLocalBall = true;
        BallInHand.Health = 1;
        BallInHand.IsHeldInHand = true;


        AudioSource snowballCrunchSound = snowballCrunchSounds[Random.Range(0, snowballCrunchSounds.Length)];
        snowballCrunchSound.Play();
    }

    private void GrabBall(GameObject ball)
    {
        BallInHand = ball.GetComponent<Snowball2>();
        BallInHand.IsHeldInHand = true;
        player.CmdDeleteMySnowBall(BallInHand.Id);

        AudioSource snowballCrunchSound = snowballCrunchSounds[Random.Range(0, snowballCrunchSounds.Length)];
        snowballCrunchSound.Play();
    }
}
