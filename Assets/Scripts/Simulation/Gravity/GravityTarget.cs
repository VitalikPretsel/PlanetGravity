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

    public Vector3 initialPosition;
    public Vector3 initialVelocity;

    public bool handleCollidingVelocity;
    public bool handleCollidingGravity;
    public bool handleCollidingJoint;

    public bool isColliding;

    private bool clearTrail = false;

    void Awake()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        trailRenderer = this.transform.Find("MapIcon").gameObject.GetComponent<TrailRenderer>();
    }

    void Start()
    {
        ApplyVelocity(initialVelocity);
    }

    void FixedUpdate()
    {
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
        }
        if (handleCollidingVelocity)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = 0;
        }
        if (handleCollidingJoint)
        {
            if (joint == null)
            {
                // Tried to use this code to make joints more realistic
                //double massProduct = rigidBody.mass * collidedRigidbody.mass;
                //Vector3 difference = rigidBody.position - collidedRigidbody.position;
                //float distance = difference.magnitude;
                //double unScaledforceMagnitude = massProduct / Math.Pow(distance, 2);
                //double forceMagnitude = 1 * unScaledforceMagnitude;
                //var breakForce = (float)forceMagnitude;
                var breakForce = 0.1f;

                joint = gameObject.AddComponent<FixedJoint2D>();
                joint.connectedBody = collidedRigidbody;
                joint.breakForce = breakForce;
                joint.enableCollision = true;
                joint.autoConfigureConnectedAnchor = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }

    void ApplyVelocity(Vector3 velocity)
    {
        rigidBody.AddForce(initialVelocity, ForceMode2D.Impulse);
    }

    public void ResetPosition()
    {
        rigidBody.position = initialPosition;
        rigidBody.velocity = Vector3.zero;
        ApplyVelocity(initialVelocity);
        transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);

        joint = null;
        isColliding = false;

        trailRenderer.enabled = false;
        clearTrail = true;
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
