using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RocketTarget : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    private GravityTarget gravityTarget;
    private GameObject particle;
    private ParticleSystem particleSys;

    public float updateVelocityValue;
    public float maxUpdateVelocityValue;

    public bool handleVelocity;

    public Vector3 moveVector;

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

    public bool idle;
    public bool old;
    public bool stuck;
    public bool collided;
    public bool away;
    public bool crushed;
    public bool hit;


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
        particle = this.transform.Find("FlameParticles").gameObject;
        particleSys = particle.GetComponent<ParticleSystem>();
        previousPosition = rigidBody.position;
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

        if (handleVelocity)
        {
            ChangeRocketVelocity();
            ChangeRocketDirection();
            ChangeFireParticle();
        }
    }

    void ChangeRocketVelocity()
    {
        if (!moveVector.Equals(Vector3.zero))
        {
            rigidBody.AddForce(updateVelocityValue * moveVector, ForceMode2D.Impulse);
        }
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
                Debug.Log("Player Stopped Moving");
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
                Debug.Log("Player is too Old");
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
                Debug.Log("Player Stuck");
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
                Debug.Log("Player Collided");
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
            Debug.Log("Player is far away");
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
                Debug.Log("Player Crushed");
                crushed = true;
            }
        }

        if (destination != null)
        {
            if (collision.gameObject == destination)
            {
                Debug.Log("Player Hit the destination");
                hit = true;
            }
        }
    }

    public void ResetRocket()
    {
        timeIdleLeft = 0;
        timeOldLeft = 0;
        timeStuckLeft = 0;
        crushCountLeft = 0;

        idle = false;
        old = false;
        stuck = false;
        collided = false;
        away = false;
        crushed = false;
        hit = false;

        transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
    }

    // cosmetic
    void ChangeFireParticle()
    {
        var main = particleSys.main;

        if (!moveVector.Equals(Vector3.zero))
        {
            main.startLifetime = (updateVelocityValue / maxUpdateVelocityValue) * 0.5f;

            float particleAngle;

            particleAngle = Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg;
            var quat1 = Quaternion.AngleAxis(particleAngle, Vector3.right);

            if (moveVector.y < 0)
            {
                moveVector.y *= -1;
            }

            particleAngle = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg;
            var quat2 = Quaternion.AngleAxis(particleAngle, Vector3.down);

            particle.transform.rotation = quat1 * quat2;
        }
        else
        {
            main.startLifetime = 0f;
        }
    }

    // cosmetic
    void ChangeRocketDirection()
    {
        Vector2 moveDirection = rigidBody.velocity;
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(-moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
            rigidBody.MoveRotation(Quaternion.AngleAxis(angle, Vector3.forward));
        }
    }
}
