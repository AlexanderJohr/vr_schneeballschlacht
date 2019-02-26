using System;
using System.Collections.Generic;
using UnityEngine;

public class Snowball2 : MonoBehaviour
{
    public Snowball2()
    {
        ConnectedBalls = new List<Snowball2>();
    }
    public int Id;
    public bool IsLocalBall;
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
    private Rigidbody _rigidbody;

    public Rigidbody Rigidbody
    {
        get
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            return _rigidbody;
        }
    }
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
            if (value < health)
            {
                Darken(0.1f);

            }
            health = value;
        }
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
        Scale += 0.15f;
    }

    public void reset()
    {
        IsConnectedToGround = false;
        ConnectedBalls.Clear();
    }
}
