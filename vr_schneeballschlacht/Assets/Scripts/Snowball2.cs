using UnityEngine;

public class Snowball2 : MonoBehaviour
{
    BallCollection ballCollection = BallCollection.Instance;



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
    public bool IsHoldedInHand { get; set; }
    public bool IsNotHoldedInHand { get { return !IsHoldedInHand; } }

    void Start()
    {
        IsHoldedInHand = false;
        ballCollection.Balls.Add(this);
        Rigidbody = GetComponent<Rigidbody>();
    }


    public void IncreaseScale()
    {
        Scale += 0.05f;
    }

    void Update()
    {


        HandleCollisionWithOtherBalls();

    }

    private void HandleCollisionWithOtherBalls()
    {
        if (!this.IsCover)
        {

            for (int i = 0; i < ballCollection.Balls.Count; i++)
            {
                var otherBall = ballCollection.Balls[i];
                var isntThisBall = this != otherBall;

                if (isntThisBall)
                {

                    bool ballsAreNotHoldedInHands = this.IsNotHoldedInHand && otherBall.IsNotHoldedInHand;
                    bool atLeastOneBallIsCover = this.IsCover || otherBall.IsCover;

                    if (ballsAreNotHoldedInHands && atLeastOneBallIsCover && otherBall.IsCover)
                    {
                        bool ballsAreCloseToEachOther = BallCollection.checkIfBallsAreCloseToEachOther(this, otherBall);
                        if (ballsAreCloseToEachOther)
                        {
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
                                this.IsCover = true;

                            }
                        }
                    }
                }
            }
        }


    }
}
