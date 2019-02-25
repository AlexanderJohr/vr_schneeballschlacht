using System;
using System.Collections.Generic;
using UnityEngine;

public class Snowball2 : MonoBehaviour
{
    public Snowball2()
    {
        ConnectedBalls = new List<Snowball2>();
    }

    public BallCollection ballCollection;

    public bool isHeldInHand;
    public float health;

    public bool UseGravity
    {
        get
        {
            return Rigidbody.useGravity;
        }
        set
        {
            if (value == false)
            {
                Rigidbody.useGravity = false;
                this.Rigidbody.velocity = Vector3.zero;
                this.Rigidbody.angularVelocity = Vector3.zero;
            }
            else
            {
                Rigidbody.useGravity = true;
            }

        }
    }

    public Rigidbody Rigidbody { get; private set; }
    public float Scale
    {
        get
        {
            return this.transform.localScale.x;
        }
        set
        {
            this.transform.localScale = new Vector3(value, value, value);
        }
    }
    public bool IsHeldInHand
    {
        get
        {
            return isHeldInHand;
        }
        set
        {
            if (value == false)
            {
                print("");
            }
            isHeldInHand = value;
        }
    }
    public bool IsNotHeldInHand { get { return !IsHeldInHand; } }

    public bool IsConnectedToGround { get; set; }
    public List<Snowball2> ConnectedBalls { get; set; }
    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            if (value < health) {
                Darken(0.1f);

            }
            health = value;
        }
    }

    void Start()
    {
        //ballCollection = GameObject.Find("BallCollection").GetComponent<BallCollection>();
        ballCollection.Balls.Add(this);
        Rigidbody = GetComponent<Rigidbody>();
    }
    Material material;

    void Awake()
    {
        material = this.GetComponent<MeshRenderer>().material;
    }

    //invoke this function from anywhere outside or even from inside this particular script instance
    public void Darken(float percent)
    {
        percent = Mathf.Clamp01(percent);
        material.color = new Color(material.color.r * (1 - percent), material.color.g * (1 - percent), material.color.b * (1 - percent), material.color.a);
    }

    public void IncreaseScale()
    {
        Scale += 0.05f;
    }

    void Update()
    {

        //Rigidbody.velocity = new Vector3(10, 0, 10);
        //HandleCollisions();

    }
    //private void HandleCollisions()
    //{
    //    float distanceOfCenterToGround = this.transform.position.y - 0.2f;

    //    float ballRadius = this.Scale / 2;

    //    float mergeWithFloorThreshold = ballRadius * BallCollection.snowballMergeThresholdMultiplier;

    //    float snowballMergeRadius = ballRadius - mergeWithFloorThreshold;
    //    float distanceOfMergeRadiusToGround = distanceOfCenterToGround - snowballMergeRadius;

    //    bool isCloseToFloor = distanceOfMergeRadiusToGround < 0;

    //    if (this.IsBall && IsNotHeldInHand && isCloseToFloor)
    //    {
    //        this.IsCover = true;
    //        return;
    //    }

    //    bool isAtLeastCloseToOneOtherBall = false;
    //    for (int i = 0; i < ballCollection.Balls.Count; i++)
    //    {

    //        var otherBall = ballCollection.Balls[i];
    //        var isntThisBall = this != otherBall;

    //        if (isntThisBall)
    //        {

    //            bool ballsAreNotHeldInHand = this.IsNotHeldInHand && otherBall.IsNotHeldInHand;

    //            if (ballsAreNotHeldInHand)
    //            {
    //                bool ballsAreCloseToEachOther = BallCollection.checkIfBallsAreCloseToEachOther(this, otherBall);
    //                if (ballsAreCloseToEachOther)
    //                {
    //                    isAtLeastCloseToOneOtherBall = true;

    //                    bool velocityIsHigh = this.Rigidbody.velocity.magnitude > 6;
    //                    if (velocityIsHigh)
    //                    {
    //                        ballCollection.Balls.Remove(otherBall);
    //                        Object.Destroy(otherBall.gameObject);
    //                        ballCollection.Balls.Remove(this);
    //                        Object.Destroy(this.gameObject);
    //                    }
    //                    else
    //                    {
    //                        if (this.IsBall || otherBall.IsCover)
    //                        {
    //                            this.IsCover = true;
    //                            break;
    //                        }
    //                        else if (this.IsCover || otherBall.IsCover)
    //                        {
    //                            break;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    bool isNotCloseToAnyOtherBall = !isAtLeastCloseToOneOtherBall;
    //    bool isNotCloseToFloor = !isCloseToFloor;
    //    bool shouldFallDown = this.IsCover && isNotCloseToAnyOtherBall && isNotCloseToFloor;
    //    if (shouldFallDown) {
    //        this.IsCover = false;
    //    }
    //}

    public void reset()
    {
        IsConnectedToGround = false;
        ConnectedBalls.Clear();
    }
}
