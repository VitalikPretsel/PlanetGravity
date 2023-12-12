using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketDeath : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private GravityTarget gravityTarget;

    public float idleTime;
    public float oldTime;
    public float stuckTime;
    public float collideTime;
    public float awayDistance;
    public int crushCount;
    public Vector3 centerPosition;
    public GameObject destination;

    public bool handleIdle;
    public bool handleOld;
    public bool handleStuck;
    public bool handleCollide;
    public bool handleAway;
    public bool handleCrush;

    private bool idle;
    private bool old;
    private bool stuck;
    private bool collided;
    private bool away;
    private bool crushed;
    private bool hit;

    private float timeIdleLeft;
    private float timeOldLeft;
    private float timeStuckLeft;
    private float timeCollideLeft;
    private int crushCountLeft;
    private Vector2 previousPosition;

    void Awake()
    {
        gravityTarget = this.GetComponent<GravityTarget>();
        rigidBody = this.GetComponent<Rigidbody2D>();
        
        if (handleIdle)
        {
            previousPosition = rigidBody.position;
        }
    }

    void FixedUpdate()
    {
        if (handleIdle)
        {
            CheckForIdle();
        }
        if (handleOld)
        {
            CheckForOld();
        }
        if (handleStuck)
        {
            CheckForStuck();
        }
        if (handleCollide)
        {
            CheckForCollided();
        }
        if (handleAway)
        {
            CheckForAway();
        }
    }

    public bool IsDead()
    {
        return stuck || collided || away || idle || old || crushed || hit;
    }

    public bool IsDeadStupid()
    {
        return stuck || collided || idle || old || crushed;
    }

    public bool IsDeadSuccess()
    {
        return hit;
    }

    void CheckForIdle()
    {
        // Check to see if rocket hasn't moved
        if (!idle && Vector2.Distance(rigidBody.position, previousPosition) < 1f)
        {
            timeIdleLeft += Time.deltaTime;
            if (timeIdleLeft > idleTime)
            {
                // Been idle for too long
                //Debug.Log("Player Stopped Moving");
                idle = true;
            }
        }
        else
        {
            timeIdleLeft = 0;
            idle = false;
        }
        previousPosition = rigidBody.position;
    }

    void CheckForOld()
    {
        // Check to see if rocket lives too long
        if (!old)
        {
            timeOldLeft += Time.deltaTime;
            if (timeOldLeft > oldTime)
            {
                // Lives for too long
                //Debug.Log("Player is too Old");
                old = true;
            }
        }
    }

    void CheckForStuck()
    {
        // Check to see if rocket fixed with joint
        if (!stuck && gravityTarget.joint != null)
        {
            timeStuckLeft += Time.deltaTime;
            if (timeStuckLeft > stuckTime)
            {
                // Been stuck for too long
                //Debug.Log("Player Stuck");
                stuck = true;
            }
        }
        else
        {
            // In case it's unstuck just for a moment
            timeStuckLeft -= Time.deltaTime;
            if (timeStuckLeft < 0)
            {
                timeStuckLeft = 0;
                stuck = false;
            }
        }
    }

    void CheckForCollided()
    {
        if (!collided && gravityTarget.isColliding)
        {
            timeCollideLeft += Time.deltaTime;
            if (timeCollideLeft > collideTime)
            {
                // Was colliding for too long
                //Debug.Log("Player Collided");
                collided = true;
            }
        }
        else
        {
            // In case it's uncollided just for a moment
            timeCollideLeft -= Time.deltaTime / 5;
            if (timeCollideLeft < 0)
            {
                timeCollideLeft = 0;
                collided = false;
            }
        }
    }

    void CheckForAway()
    {
        // Check to see if rocket is too far away from center
        if (!away && Vector3.Distance(rigidBody.position, centerPosition) > awayDistance)
        {
            //Debug.Log("Player is far away");
            away = true;
        }
        else
        {
            away = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (handleCrush)
        {
            crushCountLeft += 1;
            if (crushCountLeft >= crushCount)
            {
                //Debug.Log("Player Crushed");
                crushed = true;
            }
        }

        if (destination != null)
        {
            if (collision.gameObject == destination)
            {
                //Debug.Log("Player Hit the destination");
                hit = true;
            }
        }
    }

    public void ResetRocketDeath()
    {
        timeIdleLeft = 0;
        timeOldLeft = 0;
        timeStuckLeft = 0;
        timeCollideLeft = 0;
        crushCountLeft = 0;

        idle = false;
        old = false;
        stuck = false;
        crushed = false;
        collided = false;
        away = false;
        hit = false;
    }
}
