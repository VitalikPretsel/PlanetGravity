using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIRocketController : MonoBehaviour
{
    public INeuralNetwork network = null;

    public RocketTarget rocket;

    public GravityTarget rocketGravity;
    public List<GravityTarget> obstacles;
    public GravityTarget destination;
    
    public int numExperiment;

    public float fitness;
    public float bestFitness;
    public int hits = 0;

    public bool alive = true;

    void Start()
    {
        var moon = GameObject.Find("Moon");
        destination = moon.GetComponent<GravityTarget>();
        rocket.rocketDeath.destination = moon;

        // obstacles.Add(GameObject.Find("Earth").GetComponent<GravityTarget>());
    }

    void FixedUpdate()
    {
        if (alive)
        {
            UpdateRocket();

            // Get fitness and check rocket status
            
            CalculateFitness();
            
            if (rocket.rocketDeath.IsDead())
            {
                if (rocket.rocketDeath.IsDeadSuccess())
                {
                    hits += 1;
                }
                else if (rocket.rocketDeath.IsDeadStupid())
                {
                    bestFitness = 0;
                }

                Stop();
            }
        }
    }

    private void UpdateRocket()
    {
        // Set inputs to network

        List<double> input = new List<double>();

        // In case you need to add obstacles uncomment this
        //foreach (var obstacle in obstacles)
        //{
        //    input.Add(ToNormValue(obstacle.rigidBody.position.x, rocket.rocketDeath.awayDistance));
        //    input.Add(ToNormValue(obstacle.rigidBody.position.y, rocket.rocketDeath.awayDistance));
        //}

        var normDistCoef = rocket.rocketDeath.awayDistance;

        input.Add(Normalize(destination.rigidBody.position.x, -normDistCoef, normDistCoef, 0, -1, 1, 0));
        input.Add(Normalize(destination.rigidBody.position.y, -normDistCoef, normDistCoef, 0, -1, 1, 0));

        input.Add(Normalize(destination.rigidBody.velocity.x, 0, 20, 10, -1, 1, 0));
        input.Add(Normalize(destination.rigidBody.velocity.y, 0, 20, 10, -1, 1, 0));

        input.Add(Normalize(rocket.rigidBody.position.x, -normDistCoef, normDistCoef, 0, -1, 1, 0));
        input.Add(Normalize(rocket.rigidBody.position.y, -normDistCoef, normDistCoef, 0, -1, 1, 0));

        input.Add(Normalize(rocket.rigidBody.velocity.x, 0, 1000, 500, -1, 1, 0));
        input.Add(Normalize(rocket.rigidBody.velocity.y, 0, 1000, 500, -1, 1, 0));

        // Get outputs from network

        double[] output = network.Run(input);

        rocket.updateVelocityValue = Normalize((float)output[0], -1, 1, 0, 0, rocket.maxUpdateVelocityValue, rocket.maxUpdateVelocityValue / 2);
        rocket.moveVector = new Vector3(
            Normalize((float)output[1], -1, 1, 0, -1, 1, 0),
            Normalize((float)output[2], -1, 1, 0, -1, 1, 0),
            0);
    }

    private void CalculateFitness()
    {
        float distance = Vector3.Distance(rocket.rigidBody.position, destination.rigidBody.position);
        float fitnessValue = 1 / distance;
        
        fitness = fitnessValue;
        if (fitness > bestFitness)
        {
            bestFitness = fitness;  // matters only value when rocket was closest to destination
        }
    }

    float Normalize(float value, float oldMin, float oldMax, float oldMid, float newMin, float newMax, float newMid)
    {
        if (value < oldMid)
            return Interpolate(value, oldMin, oldMid, newMin, newMid);
        else if (value > oldMid)
            return Interpolate(value, oldMid, oldMax, newMid, newMax);
        else
            return newMid;
    }

    float Interpolate(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (float)(newMin + (newMax - newMin) * (value - oldMin) / (oldMax - oldMin));
    }


    private void Stop()
    {
        alive = false;

        rocket.handleVelocity = false;
        rocket.updateVelocityValue = 0;
        rocket.rigidBody.velocity = Vector3.zero;
        rocket.rigidBody.simulated = false;

        network.Fitness += bestFitness / numExperiment; // to get avarage for all experiments
        //network.Fitness = bestFitness;
    }

    public void ResetIndividual()
    {
        ResetExperiment();

        network.Fitness = 0;
        hits = 0;
    }

    public void ResetExperiment()
    {
        alive = true;

        rocket.rigidBody.simulated = true;
        rocket.handleVelocity = true;
        rocket.updateVelocityValue = 0;

        fitness = 0;
        bestFitness = 0;
        
        rocketGravity.ResetPosition();
        rocket.rocketDeath.ResetRocketDeath();
    }
}
