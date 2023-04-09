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
    private int numExperiment;

    public float fitness;
    public float bestFitness;

    public bool alive = true;

    public int hits = 0;

    void Start()
    {
        obstacles.Add(GameObject.Find("Earth").GetComponent<GravityTarget>());
        destination = GameObject.Find("Moon").GetComponent<GravityTarget>();
        rocket.destination = GameObject.Find("Moon");
        numExperiment = GameObject.Find("Academy").GetComponent<Academy>().numExperiment;
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


            if (rocket.hit)
            {
                hits += 1;
            }
            if (rocket.stuck || rocket.away || rocket.idle || rocket.crushed || rocket.hit)
            {
                Stop();
            }
        }
    }

    private void CalculateFitness()
    {
        float distance = Vector3.Distance(rocket.rigidBody.position, destination.rigidBody.position);

        // to prevent fitnessValue higher than 1
        if (distance < 1)
        {
            distance = 1;
        }

        float fitnessValue = 1 / distance;

        fitness = fitnessValue;
        if (fitness > bestFitness)
        {
            bestFitness = fitness;  // matters only value when rocket was closest to destination
        }
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

        network.fitness += bestFitness / numExperiment; // to get avarage for all experiments
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

        fitness = 0;
        bestFitness = 0;
        
        hits = 0;

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

        fitness = 0;
        bestFitness = 0;

        rocket.ResetRocket();
        rocketGravity.ResetPosition();
    }
}
