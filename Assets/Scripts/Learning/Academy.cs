using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Academy : MonoBehaviour
{
    public int numGenomes;
    public int numSimulate;
    public int numExperiment;

    public float mutationRate;

    public GameObject rocketFab;
    
    GameObject[] rockets;
    AIRocketController[] rocketControllers;

    public GeneticController species;

    public int currentGenome;
    public int currentExperiment;
    public int batchSimulate;
    public int currentGeneration;

    public CameraTarget cameraTarget;
    public float bestGenomeFitness;
    public AIRocketController bestRocket;

    public GameObject Network_GUI;
    private UI_Network networkUI;

    void Start()
    {
        species = new GeneticController(numGenomes, mutationRate);
        rockets = new GameObject[numSimulate];
        rocketControllers = new AIRocketController[numSimulate];

        for (int i = 0; i < numSimulate; i++)
        {
            rockets[i] = Instantiate(rocketFab, rocketFab.GetComponent<GravityTarget>().startingPosition, rocketFab.transform.rotation);
            rocketControllers[i] = rockets[i].GetComponent<AIRocketController>();
            rocketControllers[i].network = species.population[i];
        }

        currentGenome = numSimulate;
        batchSimulate = numSimulate;

        Network_GUI = Instantiate(Network_GUI);
        UI_Genetics genetics = Network_GUI.GetComponentInChildren<UI_Genetics>();
        genetics.academy = this;
        networkUI = Network_GUI.GetComponentInChildren<UI_Network>();
        networkUI.Display(rocketControllers[0].network);
    }

    void FixedUpdate()
    {
        bool allRocketsDead = true;
        float bestCarFitness = 0;

        foreach (AIRocketController rocket in rocketControllers)
        {
            if (rocket.alive) 
            {
                allRocketsDead = false;

                // find best rocket
                if (rocket.fitness > bestCarFitness)
                {
                    bestCarFitness = rocket.fitness;
                    bestRocket = rocket;

                    cameraTarget.target = rocket.transform;
                    networkUI.DrawConnections(rocket.network);

                    if (rocket.fitness > bestGenomeFitness)
                    {
                        bestGenomeFitness = rocket.fitness;
                    }
                }
            }
        }

        if (allRocketsDead)
        {
            Debug.Log("All Rockets Dead");

            if (currentExperiment == numExperiment)
            {
                if (currentGenome == numGenomes)
                {
                    species.NextGeneration();

                    for (int i = 0; i < numSimulate; i++)
                    {
                        rocketControllers[i].network = species.population[i];
                        rocketControllers[i].Reset();
                    }

                    currentGeneration++;
                    currentGenome = numSimulate;
                }
                else
                {
                    if (currentGenome + numSimulate <= numGenomes)
                    {
                        Debug.Log("Full Sim");
                        batchSimulate = numSimulate;
                    }
                    else
                    {
                        Debug.Log("Partial Sim");
                        batchSimulate = numGenomes - currentGenome;
                    }

                    for (int i = 0; i < batchSimulate; i++)
                    {
                        rocketControllers[i].network = species.population[currentGenome + i];
                        rocketControllers[i].Reset();
                    }

                    currentGenome += batchSimulate;
                }

                currentExperiment = 1;
            }
            else
            {
                for (int i = 0; i < batchSimulate; i++)
                {
                    rocketControllers[i].ResetExp();
                }

                currentExperiment += 1;
            }
        }
    }
}
