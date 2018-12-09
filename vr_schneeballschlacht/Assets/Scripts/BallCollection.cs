using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BallCollection
{
    #region singleton   
    private static readonly BallCollection instance = new BallCollection();

    static BallCollection()
    {
    }

    private BallCollection()
    {
        Balls = new List<Snowball2>();
        BallsConnectedToGround = new List<Snowball2>();
    }

    public static BallCollection Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    public const float snowballMergeThresholdMultiplier = 0.2f;

    public List<Snowball2> Balls { get; set; }
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
            return ball1;
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
        BallsConnectedToGround.Clear();
        for (int i = 0; i < Balls.Count; i++)
        {
            var ball = Balls[i];
            ball.reset();

            float distanceOfCenterToGround = ball.transform.position.y - 0.2f;

            float ballRadius = ball.Scale / 2;

            float mergeWithFloorThreshold = ballRadius * BallCollection.snowballMergeThresholdMultiplier;

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

            for (int j = 0; j < Balls.Count; j++)
            {
                var otherBall = Balls[j];
                var thisBall = ball;

                var thisBallIsntOtherBall = thisBall != otherBall;

                if (thisBallIsntOtherBall)
                {

                    bool ballsAreNotHeldInHand = thisBall.IsNotHeldInHand && otherBall.IsNotHeldInHand;

                    if (ballsAreNotHeldInHand)
                    {
                        bool ballsAreCloseToEachOther = BallCollection.checkIfBallsAreCloseToEachOther(thisBall, otherBall);
                        if (ballsAreCloseToEachOther)
                        {
                            bool velocityIsHigh = thisBall.Rigidbody.velocity.magnitude > 6;
                            if (velocityIsHigh)
                            {
                                Balls.Remove(otherBall);
                                Object.Destroy(otherBall.gameObject);
                                Balls.Remove(thisBall);
                                Object.Destroy(thisBall.gameObject);
                            }
                            else
                            {
                                thisBall.ConnectedBalls.Add(otherBall);

                                if (thisBall.IsBall || otherBall.IsCover)
                                {
                                    thisBall.IsCover = true;
                                }
                            }
                        }
                    }
                }
            }
        }


        for (int i = 0; i < BallsConnectedToGround.Count; i++)
        {
            var ball = Balls[i];
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

        for (int i = 0; i < Balls.Count; i++)
        {
            var ball = Balls[i];

            bool shouldFallDown = ball.IsCover && !ball.IsConnectedToGround;
            if (shouldFallDown)
            {
                ball.IsCover = false;
            }
        }



    }


}