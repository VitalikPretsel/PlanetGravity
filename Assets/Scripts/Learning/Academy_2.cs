using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Academy_2 : MonoBehaviour
{
    public int numGenomes;
    public int numSimulate;
    public int numExperiment;

    public float C1 = 1f;
    public float C2 = 1f;
    public float C3 = 0.3f;

    public float compatibilityThreshold = 3f;
    public float survivalChance = 0.1f;
    public float weightMutationChance = 0.8f;
    public float randomWeightChance = 0.1f;
    public float addNodeChance = 0.03f;
    public float addConnectionChance = 0.05f;

    public bool resetPosition;

    public GameObject rocketFab;
    
    private GameObject[] rockets;
    private AIRocketController[] rocketControllers;

    public GeneticController_2 species;

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

    //public GameObject Network_GUI;
    //private UI_Network networkUI;

    void Start()
    {
        //EmptySaves();

        InitializeSpecies();
        //InitializeUI();

        currentGenome = numSimulate;
        batchSimulate = numSimulate;
    }

    void FixedUpdate()
    {
        bool allRocketsDead = CheckRockets();

        if (allRocketsDead)
        {
            Debug.Log("All Rockets Dead");

            if (currentExperiment == numExperiment)
            {
                UpdateBestGenomeStats();

                if (currentGenome == numGenomes)
                {
                    //SaveBestNetwork();
                    //SaveStats();
                    Debug.Log("Next generation");
                    UpdateForNextGeneration();
                }
                else
                {
                    Debug.Log("Next batch");
                    UpdateForNextBatch();
                }

                currentExperiment = 1;
            }
            else
            {
                Debug.Log("Next experiment");
                UpdateForNextExperiment();
            }
        }
    }

    private bool CheckRockets()
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
                    //networkUI.DrawConnections(rocket.network);

                    if (rocket.fitness > bestExperimentFitness)
                    {
                        bestExperimentFitness = rocket.fitness;
                    }
                }
            }
        }

        return allRocketsDead;
    }

    private void UpdateBestGenomeStats()
    {
        foreach (AIRocketController rocket in rocketControllers)
        {
            if (rocket.network.Fitness > bestGenomeFitness)
            {
                bestGenomeFitness = rocket.network.Fitness;
            }

            float hitsPercent = (float)rocket.hits / numExperiment;
            if (hitsPercent > bestGenomeHitsPercent)
            {
                bestGenomeHitsPercent = hitsPercent;
            }
        }
    }

    private void UpdateForNextGeneration()
    {
        species.NextGeneration();
        Debug.Log("Got next generation");
        lastGenerationAverageFitness = species.averageFitness;
        if (lastGenerationAverageFitness > bestGenerationAverageFitness)
        {
            bestGenerationAverageFitness = lastGenerationAverageFitness;
        }

        for (int i = 0; i < numSimulate; i++)
        {
            currentGenerationHitsNumber += rocketControllers[i].hits;
            rocketControllers[i].network = species.nets[i];
            rocketControllers[i].ResetIndividual();
        }

        lastGenerationHitsPercent = currentGenerationHitsNumber / (numGenomes * numExperiment);
        if (lastGenerationHitsPercent > bestGenerationHitsPercent)
        {
            bestGenerationHitsPercent = lastGenerationHitsPercent;
        }
        currentGenerationHitsNumber = 0;

        currentGeneration += 1;
        currentGenome = numSimulate;
    }

    private void UpdateForNextBatch()
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
            rocketControllers[i].network = species.nets[currentGenome + i];
            rocketControllers[i].ResetIndividual();
        }

        currentGenome += batchSimulate;
    }

    private void UpdateForNextExperiment()
    {
        for (int i = 0; i < batchSimulate; i++)
        {
            rocketControllers[i].ResetExperiment();
        }

        currentExperiment += 1;
    }

    private void InitializeSpecies()
    {
        species = new GeneticController_2(
            numGenomes,
            C1,
            C2,
            C3,
            compatibilityThreshold,
            survivalChance, 
            weightMutationChance, 
            randomWeightChance, 
            addNodeChance, 
            addConnectionChance);
        rockets = new GameObject[numSimulate];
        rocketControllers = new AIRocketController[numSimulate];

        for (int i = 0; i < numSimulate; i++)
        {
            rockets[i] = Instantiate(rocketFab, rocketFab.GetComponent<GravityTarget>().initialPosition, rocketFab.transform.rotation);
            rocketControllers[i] = rockets[i].GetComponent<AIRocketController>();
            rocketControllers[i].network = species.nets[i];
        }
    }

    //private void InitializeUI()
    //{
    //    Network_GUI = Instantiate(Network_GUI);
    //    UI_Genetics genetics = Network_GUI.GetComponentInChildren<UI_Genetics>();
    //    genetics.academy = this;
    //    networkUI = Network_GUI.GetComponentInChildren<UI_Network>();
    //    networkUI.Display(rocketControllers[0].network);
    //}


    //private void EmptySaves()
    //{
    //    if (Directory.Exists("./Saves"))
    //    {
    //        Directory.Delete("./Saves", true);
    //    }
    //    Directory.CreateDirectory("./Saves");
    //    Directory.CreateDirectory("./Saves/FitnessSaves");
    //    Directory.CreateDirectory("./Saves/HitsSaves");
    //    Directory.CreateDirectory("./Saves/NetworkSaves");
    //}

    //private void SaveBestNetwork()
    //{
    //    species.population.OrderByDescending(n => n.Fitness).FirstOrDefault().Save(currentGeneration);
    //}

    //private void SaveStats()
    //{
    //    StreamWriter fitWriter = new StreamWriter("./Saves/FitnessSaves/fits.csv", true);
    //    StreamWriter hitWriter = new StreamWriter("./Saves/HitsSaves/hits.csv", true);

    //    foreach (var individual in species.population)
    //    {
    //        fitWriter.Write(individual.Fitness + ", ");
    //    }
    //    fitWriter.Write("\n");
    //    hitWriter.Write(lastGenerationHitsPercent + "\n");

    //    fitWriter.Close();
    //    hitWriter.Close();
    //}
}
