using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class AcademyNEAT : AbstractAcademy<NeuralNetworkNEAT>
{
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


    protected override void InitializeSpecies()
    {
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
        }
    }

    protected override void InitializeUI()
    {
        Network_GUI = Instantiate(Network_GUI);
        UI_Genetics_NEAT genetics = Network_GUI.GetComponentInChildren<UI_Genetics_NEAT>();
        genetics.academy = this;
    }
}
