using System;
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

    public Text currentExperimentFitness;
    public Text bestExperimentFitness;
    public Text bestGenomeFitness;
    public Text lastGenerationAverageFitness;
    public Text bestGenerationAverageFitness;

    void Update()
    {
        Time.timeScale = slider.value;
        timeScale.text = "Timescale: " + slider.value + "x";

        experiment.text = "Experiment #: " + academy.currentExperiment + " / " + academy.numExperiment;
        genome.text = "Genome #: " + (academy.currentGenome + 1 - academy.batchSimulate) + "-" + academy.currentGenome + " / " + academy.numGenomes;
        generation.text = "Generation #: " + academy.currentGeneration;

        currentExperimentFitness.text = "Current Experiment Fit: " + Math.Round(academy.currentExperimentFitness, 5);
        bestExperimentFitness.text = "Best Experiment Fit: " + Math.Round(academy.bestExperimentFitness, 5);
        bestGenomeFitness.text = "Best Genome Fit: " + Math.Round(academy.bestGenomeFitness, 5);
        lastGenerationAverageFitness.text = "Last Generation Avg Fit: " + Math.Round(academy.lastGenerationAverageFitness, 5);
        bestGenerationAverageFitness.text = "Best Generation Avg Fit: " + Math.Round(academy.bestGenerationAverageFitness, 5);
    }
}
