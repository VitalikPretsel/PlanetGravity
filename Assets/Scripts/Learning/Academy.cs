using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Academy : AbstractAcademy<NeuralNetwork>
{
    public float mutationRate = 0.05F;
    public float crossoverRate = 0.5F;
    public float mutationChange = 0.001F;

    public bool autoUpdateParameters = false;
    public int noFitImproveMaxCount = 100;
    public int noFitImproveCount = 0;

    protected UI_Network networkUI;


    protected override void InitializeSpecies()
    {
        species = new GeneticController(numGenomes, mutationRate, crossoverRate, mutationChange);
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
        UI_Genetics genetics = Network_GUI.GetComponentInChildren<UI_Genetics>();
        genetics.academy = this;
        networkUI = Network_GUI.GetComponentInChildren<UI_Network>();
        networkUI.Display(rocketControllers[0].network as NeuralNetwork);
    }

    protected override void UpdateNetworkUI(AIRocketController rocket)
    {
        networkUI.DrawConnections(rocket.network as NeuralNetwork);
    }

    protected override void UpdateGeneticControllerParameters()
    {
        ((GeneticController)species).mutationRate = mutationRate;
        ((GeneticController)species).crossoverRate = crossoverRate;
        ((GeneticController)species).mutationChange = mutationChange;

        if (autoUpdateParameters)
        {
            if (lastGenerationAverageFitness < bestGenerationAverageFitness)
            {
                noFitImproveCount += 1;

                if (noFitImproveCount >= noFitImproveMaxCount)
                {
                    noFitImproveCount = 0;
                    if (mutationRate > mutationChange)
                        mutationRate /= 2;
                    else
                        mutationChange /= 2;
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
        paramsWriter.Write($"{mutationChange}, {mutationRate}, {crossoverRate} \n");
        paramsWriter.Close();
    }
}
