//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;
using Valve.VR;

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

    public bool BallsAreCloseToEachOther { get { return ballCollection.BallsAreCloseToEachOther; } }


    public BallCollection ballCollection;

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_Behaviour_Pose>();
        if (ThisIsTheLeftHand)
        {
            ballCollection.LeftHand = this;
        }
        else if (ThisIsTheRightHand)
        {
            ballCollection.RightHand = this;
        }

    }

    private void FixedUpdate()
    {
        Snowball2 ballInGrabDistance = null;
        for (int i = 0; i < ballCollection.Balls.Count; i++)
        {
            var ball = ballCollection.Balls[i];
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

    private void MergeBalls()
    {
        Hand handWhereBallGoesTo;
        Hand handWhereBallWillBeDeleted;

        if (ballCollection.OneBallInHandIsBigger)
        {
            handWhereBallWillBeDeleted = ballCollection.HandWithSmalestBall;
            handWhereBallGoesTo = ballCollection.HandWithBiggestBall;
        }
        else
        {
            var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;

            float leftBallVelocity = origin.TransformVector(ballCollection.LeftHand.trackedObj.GetVelocity()).magnitude;
            float rightBallVelocity = origin.TransformVector(ballCollection.RightHand.trackedObj.GetVelocity()).magnitude;

            if (leftBallVelocity > rightBallVelocity)
            {
                handWhereBallWillBeDeleted = ballCollection.LeftHand;
                handWhereBallGoesTo = ballCollection.RightHand;
            }
            else
            {

                handWhereBallWillBeDeleted = ballCollection.RightHand;
                handWhereBallGoesTo = ballCollection.LeftHand;
            }
        }

        ballCollection.Balls.Remove(handWhereBallWillBeDeleted.BallInHand);
        Object.Destroy(handWhereBallWillBeDeleted.BallInHand.gameObject);
        handWhereBallWillBeDeleted.BallInHand = null;
        handWhereBallGoesTo.BallInHand.IncreaseScale();
        handWhereBallGoesTo.BallInHand.Health += 0.1f;

    }

    private void ThrowBall()
    {
        var rigidbody = BallInHand.GetComponent<Rigidbody>();

        BallInHand = null;

        // We should probably apply the offset between trackedObj.transform.position
        // and device.transform.pos to insert into the physics sim at the correct
        // location, however, we would then want to predict ahead the visual representation
        // by the same amount we are predicting our render poses.

        var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
        if (origin != null)
        {
            rigidbody.velocity = origin.TransformVector(trackedObj.GetVelocity());
            rigidbody.angularVelocity = origin.TransformVector(trackedObj.GetAngularVelocity());
        }
        else
        {
            rigidbody.velocity = trackedObj.GetVelocity();
            rigidbody.angularVelocity = trackedObj.GetAngularVelocity();
        }

        rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
    }

    private void CreateBallInHand()
    {
        var go = GameObject.Instantiate(prefab);
        go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        BallInHand = go.GetComponent<Snowball2>();
        BallInHand.Health = 1;
        ballInHand.ballCollection = ballCollection;
        BallInHand.IsHeldInHand = true;
    }

    private void GrabBall(GameObject ball)
    {
        BallInHand = ball.GetComponent<Snowball2>();
        BallInHand.IsHeldInHand = true;
    }
}
