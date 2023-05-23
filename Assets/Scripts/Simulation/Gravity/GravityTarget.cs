using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GravityTarget : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    public FixedJoint2D joint;
    private TrailRenderer trailRenderer;

    public bool isAttractee;
    public bool isAttractor;

    public Vector3 startingPosition;
    public Vector3 initialVelocity;
    public bool applyInitialVelocityOnStart;

    public bool handleCollidingVelocity;
    public bool handleCollidingGravity;
    public bool handleCollidingJoint;

    public bool isColliding;

    private Vector3 savedVelocity;

    private bool clearTrail = false;

    void Awake()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        trailRenderer = this.transform.Find("MapIcon").gameObject.GetComponent<TrailRenderer>();
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
        savedVelocity = rigidBody.velocity;

        SetGravityState(!isColliding);
        if (joint != null)
        {
            joint.autoConfigureConnectedAnchor = false;
        }

        if (clearTrail)
        {
            clearTrail = false;
            trailRenderer.Clear();
            trailRenderer.enabled = true;
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
        var collidedRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

        if (handleCollidingGravity)
        {
            isColliding = true;
            SetAsAttractee(false);
        }
        if (handleCollidingVelocity)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = 0;

            collidedRigidbody.velocity = collision.gameObject.GetComponent<GravityTarget>().savedVelocity;
        }
        if (handleCollidingJoint)
        {
            if (joint == null)
            {
                joint = gameObject.AddComponent<FixedJoint2D>();
                joint.connectedBody = collidedRigidbody;
                joint.breakForce = 0.1f;
                joint.enableCollision = true;
                joint.autoConfigureConnectedAnchor = true;
            }

            // Tried to use this code to make joints more realistic, guess we don't need to use joints at all for realistic simulations
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
        isColliding = false;

        if (handleCollidingGravity)
        {
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
            if (!GravityHandler.attractees.Contains(this))
            {
                GravityHandler.attractees.Add(this);
            }
        }
        else
        {
            GravityHandler.attractees.Remove(this);
        }
    }

    public void ResetPosition()
    {
        rigidBody.position = startingPosition;

        joint = null;
        isColliding = false;
        SetAsAttractee(isAttractee);

        rigidBody.velocity = Vector3.zero;
        ApplyVelocity(initialVelocity);

        trailRenderer.enabled = false;
        clearTrail = true;
    }

    void SetAsAttractor(bool value)
    {
        if (value)
        {
            if (!GravityHandler.attractors.Contains(this))
            {
                GravityHandler.attractors.Add(this);
            }
        }
        else
        {
            GravityHandler.attractors.Remove(this);
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
