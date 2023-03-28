using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIRocketController : MonoBehaviour
{
    public NeuralNetwork network = null;

    public RocketTarget rocket;

    public GravityTarget rocketGravity;
    public List<GravityTarget> obstacles;
    public GravityTarget destination;

    public float fitness;

    public bool alive = true;

    void Start()
    {
        obstacles.Add(GameObject.Find("Earth").GetComponent<GravityTarget>());
        destination = GameObject.Find("Moon").GetComponent<GravityTarget>();
    }

    void FixedUpdate()
    {
        if (alive)
        {
            // set inputs to network

            List<double> input = new List<double>();

            foreach (var obstacle in obstacles)
            {
                input.Add(obstacle.rigidBody.position.x);
                input.Add(obstacle.rigidBody.position.y);
            }

            input.Add(destination.rigidBody.position.x);
            input.Add(destination.rigidBody.position.y);

            input.Add(rocket.rigidBody.position.x);
            input.Add(rocket.rigidBody.position.y);

            input.Add(rocket.rigidBody.velocity.x);
            input.Add(rocket.rigidBody.velocity.y);

            // get outputs from network

            double[] output = network.Run(input);

            rocket.updateVelocityValue = ToUseValue((float)output[0], rocket.maxUpdateVelocityValue);
            rocket.moveVector = new Vector3((float)output[1], (float)output[2], 0);

            // get fitness and check rocket status

            CalculateFitness();

            if (rocket.stuck || rocket.away || rocket.idle || rocket.crushed)
            {
                Stop();
            }

        }
    }

    private void CalculateFitness()
    {
        float distance = Vector3.Distance(rocket.rigidBody.position, destination.rigidBody.position);

        float fitnessValue = 1 / distance;

        fitness = fitnessValue;
    }

    private float ToUseValue(float value, float max = 1)
    {
        return value * max;
    }

    private void Stop()
    {
        alive = false;

        rocket.handleVelocity = false;
        rocket.updateVelocityValue = 0;
        rocket.rigidBody.isKinematic = true;
        rocket.rigidBody.velocity = Vector3.zero;

        if (network.fitness == 0)
        {
            network.fitness = fitness;
        }
        else
        {
            network.fitness = (network.fitness + fitness) / 2;
        }
    }

    public void Reset()
    {
        alive = true;

        rocket.handleVelocity = true;
        rocket.updateVelocityValue = 0;
        rocket.rigidBody.isKinematic = false;
        
        rocketGravity.ResetPosition();
        rocket.ResetRocket();

        network.fitness = 0;

        foreach (var obstacle in obstacles)
        {
            obstacle.ResetPosition();
        }
        destination.ResetPosition();
    }

    public void ResetExp()
    {
        alive = true;

        rocket.handleVelocity = true;
        rocket.updateVelocityValue = 0;
        rocket.rigidBody.isKinematic = false;

        rocketGravity.ResetPosition();
        rocket.ResetRocket();
    }
}
