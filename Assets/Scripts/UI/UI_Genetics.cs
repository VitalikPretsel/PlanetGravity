using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Genetics : MonoBehaviour
{
    public Academy academy;

    public Text timeScale;
    public Slider slider;

    public Text experiment;
    public Text genome;
    public Text generation;

    public Text fitness;
    public Text averageFitness;
    public Text bestFitness;

    void Update()
    {
        Time.timeScale = slider.value;
        timeScale.text = "Timescale: " + slider.value + "x";

        experiment.text = "Experiment #: " + academy.currentExperiment + " / " + academy.numExperiment;
        genome.text = "Genome #: " + (academy.currentGenome + 1 - academy.batchSimulate) + "-" + academy.currentGenome + " / " + academy.numGenomes;
        generation.text = "Generation #: " + academy.currentGeneration;

        if (academy.bestRocket != null)
        {
            fitness.text = "Current Fitness: " + academy.bestRocket.fitness;
        }

        bestFitness.text = "Alltime Best Fitness: " + academy.bestGenomeFitness;
        averageFitness.text = "Last Average Fitness: " + academy.species.averageFitness;
    }
}
