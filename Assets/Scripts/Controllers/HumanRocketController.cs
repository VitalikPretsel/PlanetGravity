using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HumanRocketController : MonoBehaviour
{
    private RocketTarget rocketTarget;

    void Awake()
    {
        rocketTarget = this.GetComponent<RocketTarget>();
    }

    void FixedUpdate()
    {
        if (rocketTarget.handleVelocity)
        {
            if (Input.GetKey(KeyCode.Q) && rocketTarget.updateVelocityValue > 0)
                rocketTarget.updateVelocityValue -= 0.0001f;
            else if (Input.GetKey(KeyCode.E) && rocketTarget.updateVelocityValue < rocketTarget.maxUpdateVelocityValue)
                rocketTarget.updateVelocityValue += 0.0001f;

            float x = 0, y = 0;
            if (Input.GetKey(KeyCode.DownArrow))
                y = -1;
            if (Input.GetKey(KeyCode.UpArrow))
                y = 1;
            if (Input.GetKey(KeyCode.LeftArrow))
                x = -1;
            if (Input.GetKey(KeyCode.RightArrow))
                x = 1;

            rocketTarget.moveVector = new Vector3(x, y, 0);
        }
    }
}
