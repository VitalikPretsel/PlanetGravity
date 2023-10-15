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

    protected UI_Network networkUI;

    protected override bool CheckRockets()
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
                    networkUI.DrawConnections(rocket.network as NeuralNetwork);

                    if (rocket.fitness > bestExperimentFitness)
                    {
                        bestExperimentFitness = rocket.fitness;
                    }
                }
            }
        }

        return allRocketsDead;
    }

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
        networkUI.Display(rocketControllers[0].network as NeuralNetwork); // "as"???
    }
}
