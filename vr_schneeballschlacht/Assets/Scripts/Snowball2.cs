using UnityEngine;

public class Snowball2 : MonoBehaviour
{
    BallCollection ballCollection = BallCollection.Instance;

    public bool isHeldInHand;
    public bool IsBall { get { return !IsCover; } }
    public bool IsCover
    {
        get
        {
            if (Rigidbody.useGravity)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        set
        {
            if (value == true)
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

    void Start()
    {
        ballCollection.Balls.Add(this);
        Rigidbody = GetComponent<Rigidbody>();
    }


    public void IncreaseScale()
    {
        Scale += 0.05f;
    }

    void Update()
    {


        HandleCollisions();
     
    }
    private void HandleCollisions()
    {
        float distanceOfCenterToGround = this.transform.position.y - 0.2f;

        float ballRadius = this.Scale / 2;

        float mergeWithFloorThreshold = ballRadius * BallCollection.snowballMergeThresholdMultiplier;

        float snowballMergeRadius = ballRadius - mergeWithFloorThreshold;
        float distanceOfMergeRadiusToGround = distanceOfCenterToGround - snowballMergeRadius;

        bool isCloseToFloor = distanceOfMergeRadiusToGround < 0;

        if (this.IsBall && IsNotHeldInHand && isCloseToFloor)
        {
            this.IsCover = true;
            return;
        }

        bool isAtLeastCloseToOneOtherBall = false;
        for (int i = 0; i < ballCollection.Balls.Count; i++)
        {

            var otherBall = ballCollection.Balls[i];
            var isntThisBall = this != otherBall;

            if (isntThisBall)
            {

                bool ballsAreNotHeldInHand = this.IsNotHeldInHand && otherBall.IsNotHeldInHand;

                if (ballsAreNotHeldInHand)
                {
                    bool ballsAreCloseToEachOther = BallCollection.checkIfBallsAreCloseToEachOther(this, otherBall);
                    if (ballsAreCloseToEachOther)
                    {
                        isAtLeastCloseToOneOtherBall = true;

                        bool velocityIsHigh = this.Rigidbody.velocity.magnitude > 6;
                        if (velocityIsHigh)
                        {
                            ballCollection.Balls.Remove(otherBall);
                            Object.Destroy(otherBall.gameObject);
                            ballCollection.Balls.Remove(this);
                            Object.Destroy(this.gameObject);
                        }
                        else
                        {
                            if (this.IsBall || otherBall.IsCover)
                            {
                                this.IsCover = true;
                                break;
                            }
                            else if (this.IsCover || otherBall.IsCover)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        bool isNotCloseToAnyOtherBall = !isAtLeastCloseToOneOtherBall;
        bool isNotCloseToFloor = !isCloseToFloor;
        bool shouldFallDown = this.IsCover && isNotCloseToAnyOtherBall && isNotCloseToFloor;
        if (shouldFallDown) {
            this.IsCover = false;
        }
    }
}
