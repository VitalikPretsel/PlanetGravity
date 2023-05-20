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

    public float mutationRate = 0.05F;
    public float crossoverRate = 0.5F;
    public float mutationChange = 0.001F;

    public bool resetPosition;

    public GameObject rocketFab;
    
    GameObject[] rockets;
    AIRocketController[] rocketControllers;

    public GeneticController species;

    public CameraTarget cameraTarget;
    public AIRocketController bestRocket;

    public int currentGenome;
    public int currentExperiment;
    public int batchSimulate;
    public int currentGeneration;

    public float currentExperimentFitness;
    public float bestExperimentFitness;
    public float bestGenomeFitness;
    public float lastGenerationAverageFitness;
    public float bestGenerationAverageFitness;

    public float currentGenomeHitsNumber;
    public float bestGenomeHitsPercent;
    private float currentGenerationHitsNumber;
    public float lastGenerationHitsPercent;
    public float bestGenerationHitsPercent;

    public GameObject Network_GUI;
    private UI_Network networkUI;

    private List<GravityTarget> obstacles = new List<GravityTarget>();
    private GravityTarget destination;

    void Start()
    {
        EmptySaves();

        obstacles.Add(GameObject.Find("Earth").GetComponent<GravityTarget>());
        destination = GameObject.Find("Moon").GetComponent<GravityTarget>();

        species = new GeneticController(numGenomes, mutationRate, crossoverRate, mutationChange);
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
                    currentExperimentFitness = bestRocketFitness;
                    currentGenomeHitsNumber = bestRocket.hits;

                    cameraTarget.target = rocket.transform;
                    networkUI.DrawConnections(rocket.network);

                    if (rocket.fitness > bestExperimentFitness)
                    {
                        bestExperimentFitness = rocket.fitness;
                    }
                }
            }
        }

        if (allRocketsDead)
        {
            Debug.Log("All Rockets Dead");

            if (currentExperiment == numExperiment)
            {
                foreach (AIRocketController rocket in rocketControllers)
                {
                    if (rocket.network.fitness > bestGenomeFitness)
                    {
                        bestGenomeFitness = rocket.network.fitness;
                    }

                    float hitsPercent = (float)rocket.hits / numExperiment;
                    if (hitsPercent > bestGenomeHitsPercent)
                    {
                        bestGenomeHitsPercent = hitsPercent;
                    }
                }

                if (currentGenome == numGenomes)
                {
                    foreach (var obstacle in obstacles)
                    {
                        obstacle.ResetPosition();
                    }
                    destination.ResetPosition();

                    SaveBestNetwork();
                    SaveStats();
                    species.NextGeneration();

                    lastGenerationAverageFitness = species.averageFitness;
                    if (lastGenerationAverageFitness > bestGenerationAverageFitness)
                    {
                        bestGenerationAverageFitness = lastGenerationAverageFitness;
                    }

                    for (int i = 0; i < numSimulate; i++)
                    {
                        currentGenerationHitsNumber += rocketControllers[i].hits;
                        rocketControllers[i].network = species.population[i];
                        rocketControllers[i].Reset();
                    }

                    lastGenerationHitsPercent = currentGenerationHitsNumber / (numGenomes * numExperiment);
                    if (lastGenerationHitsPercent > bestGenerationHitsPercent)
                    {
                        bestGenerationHitsPercent = lastGenerationHitsPercent;
                    }
                    currentGenerationHitsNumber = 0;

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
                        currentGenerationHitsNumber += rocketControllers[i].hits;
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
        Directory.CreateDirectory("./Saves/HitsSaves");
        Directory.CreateDirectory("./Saves/NetworkSaves");
    }

    private void SaveBestNetwork()
    {
        species.population.OrderByDescending(n => n.fitness).FirstOrDefault().Save(currentGeneration);
    }

    private void SaveStats()
    {
        StreamWriter fitWriter = new StreamWriter("./Saves/FitnessSaves/fits.csv", true);
        StreamWriter hitWriter = new StreamWriter("./Saves/HitsSaves/hits.csv", true);

        foreach (var individual in species.population)
        {
            fitWriter.Write(individual.fitness + ", ");
        }
        fitWriter.Write("\n");
        hitWriter.Write(lastGenerationHitsPercent + "\n");

        fitWriter.Close();
        hitWriter.Close();
    }
}
