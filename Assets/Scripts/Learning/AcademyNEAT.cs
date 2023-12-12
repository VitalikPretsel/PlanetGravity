using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AcademyNEAT : AbstractAcademy<NeuralNetworkNEAT>
{
    public bool updateNetUI = false;
    public bool autoUpdateParameters = false;


    public float C1 = 1f;
    public float C2 = 1f;
    public float C3 = 0.3f;

    public float compatibilityThreshold = 3f;

    public float weightMutationChance = 0.8f;
    public float weightMutationChange = 0.005f;
    public float eachWeightMutationChance = 0.05f;
    public float randomWeightChance = 0.05f;
    public float addNodeChance = 0.001f;
    public float addConnectionChance = 0.0015f;

    public float crossoverRate = 0.75f;
    public float interSpeciesCrossoverRate = 0.001f;


    public int noFitImproveMaxCount = 100;
    public int noFitImproveCount = 0;

    //public int maxMutationReducedCount = 4;
    //public int mutationReducedCount = 0;
    public float speed = 2;

    public GameObject networkUIPrefab;
    private GameObject networkUI;
    private TextureDraw textureDraw;

    public UnityEngine.Color currentColor;
    public float currentNodesCount = 0;
    public float currentConnectionsCount = 0;
    public float averageNodesCount = 0;
    public float averageConnectionsCount = 0;

    protected override void InitializeSpecies()
    {
        textureDraw = GetComponent<TextureDraw>();
        textureDraw.SetPopSize(numGenomes);

        species = new GeneticControllerNEAT(
            numGenomes,
            C1,
            C2,
            C3,
            compatibilityThreshold,
            weightMutationChance,
            weightMutationChange,
            eachWeightMutationChance,
            randomWeightChance,
            addNodeChance,
            addConnectionChance,
            crossoverRate,
            interSpeciesCrossoverRate);
        rockets = new GameObject[numSimulate];
        rocketControllers = new AIRocketController[numSimulate];

        for (int i = 0; i < numSimulate; i++)
        {
            rockets[i] = Instantiate(rocketFab, rocketFab.GetComponent<GravityTarget>().initialPosition, rocketFab.transform.rotation);
            rocketControllers[i] = rockets[i].GetComponent<AIRocketController>();
            rocketControllers[i].numExperiment = numExperiment;
            rocketControllers[i].network = species.Networks[i];
            UpdateRocketAdditionally(rocketControllers[i]);
        }

        averageNodesCount = ((GeneticControllerNEAT)species).AverageNodesCount;
        averageConnectionsCount = ((GeneticControllerNEAT)species).AverageConnectionsCount;
    }

    protected override void UpdateForNextGeneration()
    {
        base.UpdateForNextGeneration();

        averageNodesCount = ((GeneticControllerNEAT)species).AverageNodesCount;
        averageConnectionsCount = ((GeneticControllerNEAT)species).AverageConnectionsCount;
    }

    protected override void InitializeUI()
    {
        Network_GUI = Instantiate(Network_GUI);
        UI_Genetics_NEAT genetics = Network_GUI.GetComponentInChildren<UI_Genetics_NEAT>();
        genetics.academy = this;
    }

    protected override void UpdateNetworkUI(AIRocketController rocket)
    {
        currentNodesCount = (rocket.network as NeuralNetworkNEAT).genome.GetNodesCount();
        currentConnectionsCount = (rocket.network as NeuralNetworkNEAT).genome.GetConnectionsCount();
        currentColor = (rocket.network as NeuralNetworkNEAT).genome.color;
        
        if (networkUI != null) { Destroy(networkUI); }
        if (updateNetUI)
        {
            networkUI = Instantiate(networkUIPrefab);
            var ui = networkUI.GetComponentInChildren<UI_Network_NEAT>();
            ui.Display(rocket.network as NeuralNetworkNEAT);
            Canvas.ForceUpdateCanvases();
            ui.DrawConnections(rocket.network as NeuralNetworkNEAT);
        }
    }

    protected override void UpdateGenerationUI()
    {
        textureDraw.AddColorData(new SpeciesColorData((species as GeneticControllerNEAT).speciesList));
    }

    protected override void UpdateRocketAdditionally(AIRocketController rocket) 
    {
        rocket.rocket.UpdateRocketColor((rocket.network as NeuralNetworkNEAT).genome.color); 
    }

    protected override void UpdateGeneticControllerParameters() 
    {
        ((GeneticControllerNEAT)species).C3 = C3;

        ((GeneticControllerNEAT)species).compatibilityThreshold = compatibilityThreshold;
        ((GeneticControllerNEAT)species).weightMutationChance = weightMutationChance;
        ((GeneticControllerNEAT)species).weightMutationChange = weightMutationChange;
        ((GeneticControllerNEAT)species).eachWeightMutationChance = eachWeightMutationChance;
        ((GeneticControllerNEAT)species).randomWeightChance = randomWeightChance;
        ((GeneticControllerNEAT)species).addNodeChance = addNodeChance;
        ((GeneticControllerNEAT)species).addConnectionChance = addConnectionChance;
        ((GeneticControllerNEAT)species).crossoverRate = crossoverRate;
        ((GeneticControllerNEAT)species).interSpeciesCrossoverRate = interSpeciesCrossoverRate;

        if (autoUpdateParameters)
        {
            if (lastGenerationAverageFitness < bestGenerationAverageFitness)
            {
                noFitImproveCount += 1;
                
                if (noFitImproveCount >= noFitImproveMaxCount)
                {
                    noFitImproveCount = 0;

                    if (addNodeChance == 0 && addConnectionChance == 0)
                    //else
                    {
                        //if (eachWeightMutationChance > 0.01)
                        //{
                        //    eachWeightMutationChance /= 2;
                        //    if (eachWeightMutationChance < 0.01)
                        //    {
                        //        eachWeightMutationChance = 0.01;
                        //    }
                        //}
                        //else
                        //{
                        //    eachWeightMutationChance = 0.01;
                        //}

                        weightMutationChange /= speed;
                        //C3 *= 100;

                        //if (addNodeChance != 0 || addConnectionChance != 0)
                        //    mutationReducedCount += 1;
                    }
                    else
                    //if (mutationReducedCount == maxMutationReducedCount)
                    {
                        if (addNodeChance > 0.0001)
                        {
                            addNodeChance /= speed;
                        }
                        else
                        {
                            addNodeChance = 0;
                        }

                        if (addConnectionChance > 0.0001)
                        {
                            addConnectionChance /= speed;
                        }
                        else
                        {
                            addConnectionChance = 0;
                        }

                        if (randomWeightChance > 0.0001)
                        {
                            randomWeightChance /= speed;
                        }
                        else
                        {
                            randomWeightChance = 0;
                        }

                        //weightMutationChange *= (float)(Math.Pow(speed, mutationReducedCount));
                        //mutationReducedCount = 0;
                    }

                }
            }
            else
            {
                noFitImproveCount = 0;
            }
        }
    }

    protected override void EmptySaves()
    {
        base.EmptySaves();

        Directory.CreateDirectory("./Saves/ParamsSaves");
        Directory.CreateDirectory("./Saves/SpeciesSaves");
    }


    protected override void SaveStats()
    {
        base.SaveStats();

        StreamWriter paramsWriter = new StreamWriter("./Saves/ParamsSaves/params.csv", true);
        paramsWriter.Write($"{addNodeChance}, {addConnectionChance}, {weightMutationChange}, {weightMutationChance}, {randomWeightChance}, {eachWeightMutationChance}, {crossoverRate}, {interSpeciesCrossoverRate} \n");
        paramsWriter.Close();

        StreamWriter speciesWriter = new StreamWriter("./Saves/SpeciesSaves/species.csv", true);
        foreach (var s in ((GeneticControllerNEAT)species).speciesList)
        {
            speciesWriter.Write(s.members.Count + ", #" + ColorUtility.ToHtmlStringRGB(s.color) + ", ");
        }
        speciesWriter.Write("\n");
        speciesWriter.Close();
    }
}
