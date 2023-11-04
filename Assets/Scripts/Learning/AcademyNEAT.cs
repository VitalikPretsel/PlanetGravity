using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AcademyNEAT : AbstractAcademy<NeuralNetworkNEAT>
{
    public bool updateNetUI = false;
    public bool autoUpdateParameters = false;

    public float C1 = 1f;
    public float C2 = 1f;
    public float C3 = 0.3f;

    public float compatibilityThreshold = 3f;
    public float survivalChance = 0.1f;
    public float weightMutationChance = 0.05f;
    public float weightMutationChange = 0.005f;
    public float randomWeightChance = 0.01f;
    public float addNodeChance = 0.001f;
    public float addConnectionChance = 0.0015f;

    public int noFitImproveMaxCount = 100;
    public int noFitImproveCount = 0; 

    public GameObject networkUIPrefab;
    private GameObject networkUI;
    private TextureDraw textureDraw;


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
            survivalChance,
            weightMutationChance,
            weightMutationChange,
            randomWeightChance,
            addNodeChance,
            addConnectionChance);
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
    }

    protected override void InitializeUI()
    {
        Network_GUI = Instantiate(Network_GUI);
        UI_Genetics_NEAT genetics = Network_GUI.GetComponentInChildren<UI_Genetics_NEAT>();
        genetics.academy = this;
    }

    protected override void UpdateNetworkUI(AIRocketController rocket)
    {
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
        ((GeneticControllerNEAT)species).compatibilityThreshold = compatibilityThreshold;
        ((GeneticControllerNEAT)species).survivalChance = survivalChance;
        ((GeneticControllerNEAT)species).weightMutationChance = weightMutationChance;
        ((GeneticControllerNEAT)species).weightMutationChange = weightMutationChange;
        ((GeneticControllerNEAT)species).randomWeightChance = randomWeightChance;
        ((GeneticControllerNEAT)species).addNodeChance = addNodeChance;
        ((GeneticControllerNEAT)species).addConnectionChance = addConnectionChance;

        if (autoUpdateParameters)
        {
            if (lastGenerationAverageFitness < bestGenerationAverageFitness)
            {
                noFitImproveCount += 1;
                
                if (noFitImproveCount >= noFitImproveMaxCount)
                {
                    noFitImproveCount = 0;

                    if (addNodeChance > 0.0001)
                    {
                        addNodeChance /= 2;
                    }
                    else
                    {
                        addNodeChance = 0;
                    }

                    if (addConnectionChance > 0.0001)
                    {
                        addConnectionChance /= 2;
                    }
                    else
                    {
                        addConnectionChance = 0;
                    }

                    if (addNodeChance == 0 && addConnectionChance == 0)
                    {
                        weightMutationChange /= 2;
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
    }


    protected override void SaveStats()
    {
        base.SaveStats();

        StreamWriter paramsWriter = new StreamWriter("./Saves/ParamsSaves/params.csv", true);
        
        paramsWriter.Write($"{addNodeChance}, {addConnectionChance}, {weightMutationChange} \n");

        paramsWriter.Close();
    }
}
