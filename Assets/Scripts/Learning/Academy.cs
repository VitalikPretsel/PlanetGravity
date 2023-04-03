using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        EmptySaves();

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
        float bestRocketFitness = 0;

        foreach (AIRocketController rocket in rocketControllers)
        {
            if (rocket.alive) 
            {
                allRocketsDead = false;

                // find best rocket
                if (rocket.fitness > bestRocketFitness)
                {
                    bestRocketFitness = rocket.fitness;
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
                    SaveBestNetwork();
                    SaveStats();
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

    private void EmptySaves()
    {
        if (Directory.Exists("./Saves"))
        {
            Directory.Delete("./Saves", true);
        }
        Directory.CreateDirectory("./Saves");
        Directory.CreateDirectory("./Saves/FitnessSaves");
        Directory.CreateDirectory("./Saves/NetworkSaves");
    }

    private void SaveBestNetwork()
    {
        species.population.OrderByDescending(n => n.fitness).FirstOrDefault().Save(currentGeneration);
    }

    private void SaveStats()
    {
        StreamWriter writer = new StreamWriter("./Saves/FitnessSaves/fits.csv", true);

        foreach (var individual in species.population)
        {
            writer.Write(individual.fitness + ", ");
        }
        writer.Write("\n");

        writer.Close();
    }
}
