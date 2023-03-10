using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GravityTarget : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private FixedJoint2D joint;

    public bool isAttractee;
    public bool isAttractor;

    public Vector3 initialVelocity;
    public bool applyInitialVelocityOnStart;
    
    public bool handleColliding;
    public bool isColliding;

    void Awake()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (applyInitialVelocityOnStart)
        {
            ApplyVelocity(initialVelocity);
        }
    }

    void FixedUpdate()
    {
        SetGravityState(!isColliding);
        if (joint != null)
        {
            joint.autoConfigureConnectedAnchor = false;
        }
    }

    void OnEnable()
    {
        SetGravityState(true);
    }

    void OnDisable()
    {
        SetGravityState(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (handleColliding)
        {
            isColliding = true;
            SetAsAttractee(false);

            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = 0;

            var collidedRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

            joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = collidedRigidbody;
            joint.breakForce = 0.1f;
            joint.enableCollision = true;
            joint.autoConfigureConnectedAnchor = true;

            //double massProduct = rigidBody.mass * collidedRigidbody.mass;
            //Vector3 difference = rigidBody.position - collidedRigidbody.position;
            //float distance = difference.magnitude;
            //double unScaledforceMagnitude = massProduct / Math.Pow(distance, 2);
            //double forceMagnitude = 1 * unScaledforceMagnitude;
            //joint.breakForce = (float)forceMagnitude;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (handleColliding)
        {
            isColliding = false;
            SetAsAttractee(isAttractee);
        }
    }

    void ApplyVelocity(Vector3 velocity)
    {
        rigidBody.AddForce(initialVelocity, ForceMode2D.Impulse);
    }

    void SetAsAttractee(bool value)
    {
        if (value)
        {
            if (!GravityHandler.attractees.Contains(this.GetComponent<Rigidbody2D>()))
            {
                GravityHandler.attractees.Add(rigidBody);
            }

        }
        else
        {
            GravityHandler.attractees.Remove(rigidBody);
        }
    }

    void SetAsAttractor(bool value)
    {
        if (value)
        {
            if (!GravityHandler.attractors.Contains(this.GetComponent<Rigidbody2D>()))
            {
                GravityHandler.attractors.Add(rigidBody);
            }
        }
        else
        {
            GravityHandler.attractors.Remove(rigidBody);
        }
    }

    void SetGravityState(bool isActive) 
    {
        if (isActive)
        {
            SetAsAttractor(isAttractor);
            SetAsAttractee(isAttractee);
        }
        else
        {
            SetAsAttractee(false);
            SetAsAttractor(false);
        }
    }
}
