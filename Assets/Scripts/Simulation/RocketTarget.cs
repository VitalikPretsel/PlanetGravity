using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D))]
public class RocketTarget : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    public RocketDeath rocketDeath;

    private GameObject particle;
    private ParticleSystem particleSys;

    public float updateVelocityValue;
    public float maxUpdateVelocityValue;

    public bool handleVelocity;

    public Vector3 moveVector;

    void Awake()
    {
        rocketDeath = this.GetComponent<RocketDeath>();
        rigidBody = this.GetComponent<Rigidbody2D>();
        particle = this.transform.Find("FlameParticles").gameObject;
        particleSys = particle.GetComponent<ParticleSystem>();
    }

    void FixedUpdate()
    {
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

    public void UpdateRocketColor(Color color)
    {
        this.transform.Find("MapIcon").GetComponent<SpriteRenderer>().color = color;
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
