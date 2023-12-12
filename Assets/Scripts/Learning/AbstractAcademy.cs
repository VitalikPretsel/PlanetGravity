using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public abstract class AbstractAcademy<T> : MonoBehaviour where T : INeuralNetwork
{
    public int numGenomes;
    public int numSimulate;
    public int numExperiment;

    public bool resetPosition;

    public GameObject rocketFab;

    protected GameObject[] rockets;
    protected AIRocketController[] rocketControllers;

    public IGeneticController<T> species;

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
    //protected UI_Network networkUI;

    void Start()
    {
        EmptySaves();

        InitializeSpecies();
        InitializeUI();
        UpdateGenerationUI();

        currentGenome = numSimulate;
        batchSimulate = numSimulate;
        currentExperiment = 1;
    }

    void FixedUpdate()
    {
        bool allRocketsDead = CheckRockets();

        if (allRocketsDead)
        {
            //Debug.Log("All Rockets Dead");
            bestRocket = null;

            if (currentExperiment == numExperiment)
            {
                UpdateBestGenomeStats();

                if (currentGenome == numGenomes)
                {
                    //SaveBestNetwork();
                    SaveStats();

                    UpdateForNextGeneration();
                    UpdateGenerationUI();
                    UpdateGeneticControllerParameters();
                }
                else
                {
                    UpdateForNextBatch();
                }

                currentExperiment = 1;
            }
            else
            {
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
                    currentExperimentFitness = bestRocketFitness;
                    currentGenomeHitsNumber = rocket.hits;

                    if (rocket != bestRocket)
                    {
                        bestRocket = rocket;
                        cameraTarget.target = rocket.transform;
                        UpdateNetworkUI(rocket);
                    }

                    if (rocket.fitness > bestExperimentFitness)
                    {
                        bestExperimentFitness = rocket.fitness;
                    }
                }
            }
            else
            {
                if (rocket == bestRocket)
                {
                    bestRocket = null;
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

    protected virtual void UpdateForNextGeneration()
    {
        species.NextGeneration();

        lastGenerationAverageFitness = species.AverageFitness;
        if (lastGenerationAverageFitness > bestGenerationAverageFitness)
        {
            bestGenerationAverageFitness = lastGenerationAverageFitness;
        }

        for (int i = 0; i < numSimulate; i++)
        {
            currentGenerationHitsNumber += rocketControllers[i].hits;
            rocketControllers[i].network = species.Networks[i];
            rocketControllers[i].ResetIndividual();

            if (resetPosition)
            {
                ResetPositions();
            }
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
            //Debug.Log("Full Sim");
            batchSimulate = numSimulate;
        }
        else
        {
            //Debug.Log("Partial Sim");
            batchSimulate = numGenomes - currentGenome;
        }

        for (int i = 0; i < batchSimulate; i++)
        {
            currentGenerationHitsNumber += rocketControllers[i].hits;
            rocketControllers[i].network = species.Networks[currentGenome + i];
            rocketControllers[i].ResetIndividual();
            UpdateRocketAdditionally(rocketControllers[i]);

            if (resetPosition)
            {
                ResetPositions();
            }
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

    private void ResetPositions()
    {
        foreach (var obstacle in rocketControllers[0].obstacles)
        {
            obstacle.ResetPosition();
        }
        rocketControllers[0].destination.ResetPosition();
    }


    protected abstract void InitializeSpecies();

    protected abstract void InitializeUI();
    
    protected abstract void UpdateNetworkUI(AIRocketController rocket);

    protected virtual void UpdateGenerationUI() { return; }

    protected virtual void UpdateRocketAdditionally(AIRocketController rocket) { return; }

    protected virtual void UpdateGeneticControllerParameters() { return; }

    protected virtual void EmptySaves()
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

    //private void SaveBestNetwork()
    //{
    //    species.Networks.OrderByDescending(n => n.Fitness).FirstOrDefault().Save(currentGeneration);
    //}

    protected virtual void SaveStats()
    {
        StreamWriter fitWriter = new StreamWriter("./Saves/FitnessSaves/fits.csv", true);
        StreamWriter hitWriter = new StreamWriter("./Saves/HitsSaves/hits.csv", true);

        foreach (var individual in species.Networks)
        {
            fitWriter.Write(individual.Fitness + ", ");
        }
        fitWriter.Write("\n");
        hitWriter.Write(lastGenerationHitsPercent + "\n");

        fitWriter.Close();
        hitWriter.Close();
    }
}
