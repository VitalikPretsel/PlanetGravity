using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GravityTarget : MonoBehaviour
{
    Rigidbody2D rigidBody;

    public bool IsAttractee
    {
        get
        {
            return isAttractee;
        }
        set
        {
            if (value == true)
            {
                if (!GravityHandler.attractees.Contains(this.GetComponent<Rigidbody2D>()))
                {
                    GravityHandler.attractees.Add(rigidBody);
                }

            }
            else if (value == false)
            {
                GravityHandler.attractees.Remove(rigidBody);
            }
            isAttractee = value;
        }
    }

    public bool IsAttractor
    {
        get
        {
            return isAttractor;
        }
        set
        {
            if (value == true)
            {
                if (!GravityHandler.attractors.Contains(this.GetComponent<Rigidbody2D>()))
                {
                    GravityHandler.attractors.Add(rigidBody);
                }
            }
            else if (value == false)
            {
                GravityHandler.attractors.Remove(rigidBody);
            }
            isAttractor = value;
        }
    }

    [SerializeField] bool isAttractee;
    [SerializeField] bool isAttractor;

    [SerializeField] public Vector3 initialVelocity;
    [SerializeField] public bool applyInitialVelocityOnStart;

    void Awake()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        //initialVelocity = new Vector3(Random.Range(-10,10), Random.Range(-10,10), 0);
        if (applyInitialVelocityOnStart)
        {
            ApplyVelocity(initialVelocity);
        }
    }

    void OnEnable()
    {
        IsAttractor = isAttractor;
        IsAttractee = isAttractee;
    }

    void OnDisable()
    {
        IsAttractor = false;
        IsAttractee = false;
    }

    void ApplyVelocity(Vector3 velocity)
    {
        rigidBody.AddForce(initialVelocity, ForceMode2D.Impulse);
    }
}
