using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HumanRocketController : MonoBehaviour
{
    private RocketHandler rocketHandler;

    void Awake()
    {
        rocketHandler = this.GetComponent<RocketHandler>();
    }

    void FixedUpdate()
    {
        if (rocketHandler.handleVelocity)
        {
            if (Input.GetKey(KeyCode.Q) && rocketHandler.updateVelocityValue > 0)
                rocketHandler.updateVelocityValue -= 0.0001f;
            else if (Input.GetKey(KeyCode.E) && rocketHandler.updateVelocityValue < rocketHandler.maxUpdateVelocityValue)
                rocketHandler.updateVelocityValue += 0.0001f;

            float x = 0, y = 0;
            if (Input.GetKey(KeyCode.DownArrow))
                y = -1;
            if (Input.GetKey(KeyCode.UpArrow))
                y = 1;
            if (Input.GetKey(KeyCode.LeftArrow))
                x = -1;
            if (Input.GetKey(KeyCode.RightArrow))
                x = 1;

            rocketHandler.moveVector = new Vector3(x, y, 0);
        }
    }
}
