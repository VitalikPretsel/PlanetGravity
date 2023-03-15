using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RocketHandler : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private GameObject particle;
    private ParticleSystem particleSys;

    public float updateVelocityValue;
    public float maxUpdateVelocityValue;
    
    public bool handleVelocity;

    void Awake()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        particle = this.transform.Find("FlameParticles").gameObject;
        particleSys = particle.GetComponent<ParticleSystem>();
    }

    void FixedUpdate()
    {
        if (handleVelocity)
        {
            if (Input.GetKey(KeyCode.Q) && updateVelocityValue > 0)
                updateVelocityValue -= 0.0001f;
            else if (Input.GetKey(KeyCode.E) && updateVelocityValue < maxUpdateVelocityValue)
                updateVelocityValue += 0.0001f;

            float x = 0, y = 0;
            if (Input.GetKey(KeyCode.DownArrow))
                y = -1;
            if (Input.GetKey(KeyCode.UpArrow))
                y = 1;
            if (Input.GetKey(KeyCode.LeftArrow))
                x = -1;
            if (Input.GetKey(KeyCode.RightArrow))
                x = 1;

            Vector3 vector = new Vector3(x, y, 0);

            var main = particleSys.main;

            if (!vector.Equals(Vector3.zero))
            {
                main.startLifetime = (updateVelocityValue / maxUpdateVelocityValue) * 0.5f;
                rigidBody.AddForce(updateVelocityValue * vector, ForceMode2D.Impulse);

                float particleAngle;

                particleAngle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
                var quat1 = Quaternion.AngleAxis(particleAngle, Vector3.up);

                if (vector.y < 0)
                {
                    vector.y *= -1;
                }

                particleAngle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
                var quat2 = Quaternion.AngleAxis(particleAngle, Vector3.right);

                particle.transform.rotation = quat1 * quat2;
            }
            else
            {
                main.startLifetime = 0f;
            }

            Vector2 moveDirection = rigidBody.velocity;
            if (moveDirection != Vector2.zero)
            {
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }
}
