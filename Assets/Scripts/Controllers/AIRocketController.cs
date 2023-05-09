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

            var normCoef = rocket.awayDistance;

            //foreach (var obstacle in obstacles)
            //{
            //    input.Add(ToNormValue(obstacle.rigidBody.position.x, normCoef));
            //    input.Add(ToNormValue(obstacle.rigidBody.position.y, normCoef));
            //}

            input.Add(ToNormValue(destination.rigidBody.position.x, normCoef));
            input.Add(ToNormValue(destination.rigidBody.position.y, normCoef));

            input.Add(ToNormValue(rocket.rigidBody.position.x, normCoef));
            input.Add(ToNormValue(rocket.rigidBody.position.y, normCoef));

            input.Add(ToNormValue(rocket.rigidBody.velocity.x, normCoef));
            input.Add(ToNormValue(rocket.rigidBody.velocity.y, normCoef));

            // get outputs from network

            double[] output = network.Run(input);

            rocket.updateVelocityValue = ToUseValue((float)output[0], rocket.maxUpdateVelocityValue, 0, rocket.maxUpdateVelocityValue);
            rocket.moveVector = new Vector3((float)output[1], (float)output[2], 0);

            // get fitness and check rocket status

            CalculateFitness();


            if (rocket.hit)
            {
                hits += 1;
            }
            if (rocket.stuck || rocket.collided || rocket.away || rocket.idle || rocket.old || rocket.crushed || rocket.hit)
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
        if (fitness > bestFitness)
        {
            bestFitness = fitness;  // matters only value when rocket was closest to destination
        }
    }

    private float ToNormValue(float value, float coef, float? min = null, float? max = null)
    {
        var normval = value / coef;
        
        min ??= -coef;
        max ??= coef;

        if (normval < min)
            normval = (float)min;
        if (normval > max)
            normval = (float)max;
        
        return normval;
    }

    private float ToUseValue(float value, float coef, float min, float max)
    {
        var useval = min + ((value + 1) / 2) * (max - min);
        return useval;
    }

    private void Stop()
    {
        alive = false;

        rocket.handleVelocity = false;
        rocket.updateVelocityValue = 0;
        rocket.rigidBody.velocity = Vector3.zero;
        rocket.rigidBody.simulated = false;

        network.fitness += bestFitness / numExperiment; // to get avarage for all experiments
    }

    public void Reset()
    {
        rocket.rigidBody.simulated = true;

        alive = true;

        rocket.handleVelocity = true;
        rocket.updateVelocityValue = 0;
        
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
        rocket.rigidBody.simulated = true;

        alive = true;

        rocket.handleVelocity = true;
        rocket.updateVelocityValue = 0;

        fitness = 0;
        bestFitness = 0;

        rocket.ResetRocket();
        rocketGravity.ResetPosition();
    }
}
