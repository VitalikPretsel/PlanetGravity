using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RocketTarget : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    private GameObject particle;
    private GameObject rocketIcon;
    private ParticleSystem particleSys;

    public float updateVelocityValue;
    public float maxUpdateVelocityValue;
    
    public bool handleVelocity;

    public Vector3 moveVector;

    public Vector3 centerPosition;
    public float idleTime;
    public float stuckTime;
    public float awayDistance;
    public bool handleCrush;

    public bool idle;
    public bool stuck;
    public bool away;
    public bool crushed;

    private float timeIdleLeft;
    private float timeStuckLeft;
    private Vector2 previousPosition;

    void Awake()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        rocketIcon = this.transform.Find("MapIcon").gameObject;
        particle = this.transform.Find("FlameParticles").gameObject;
        particleSys = particle.GetComponent<ParticleSystem>();
        previousPosition = rigidBody.position;
    }

    void FixedUpdate()
    {
        //CheckForIdle();
        CheckForStuck();
        CheckForAway();

        if (handleVelocity)
        {
            ChangeRocketVelocity();
            ChangeRocketDirection();
            //ChangeMapIconDirection();
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
        }
        previousPosition = rigidBody.position;
    }

    void CheckForStuck()
    {
        // Check to see if rocket fixed with joint
        var joint = gameObject.GetComponent<FixedJoint2D>();

        if (!stuck && joint != null)
        {
            timeStuckLeft += Time.deltaTime;
            if (timeStuckLeft > idleTime)
            {
                // Been stuck for too long
                Debug.Log("Player Stuck");
                stuck = true;
            }
        }
        else
        {
            timeStuckLeft = 0;
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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (handleCrush)
        {
            Debug.Log("Player Crushed");
            crushed = true;
        }
    }

    public void ResetRocket()
    {
        timeIdleLeft = 0;
        timeStuckLeft = 0;

        idle = false;
        stuck = false;
        away = false;
        crushed = false;

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
            //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            rigidBody.MoveRotation(Quaternion.AngleAxis(angle, Vector3.forward));
        }
    }
}
