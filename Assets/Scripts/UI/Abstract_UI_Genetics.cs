using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Abstract_UI_Genetics<T> : MonoBehaviour where T : INeuralNetwork
{
    public AbstractAcademy<T> academy; // Academy

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

    public Text currentGenomeHitsNumber;
    public Text bestGenomeHitsPercent;
    public Text lastGenerationHitsPercent;
    public Text bestGenerationHitsPercent;

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

        currentGenomeHitsNumber.text = "Current Genome Hits Number: " + academy.currentGenomeHitsNumber;
        bestGenomeHitsPercent.text = "Best Genome Hits: " + Math.Round(academy.bestGenomeHitsPercent * 100, 2) + "%";
        lastGenerationHitsPercent.text = "Last Generation Hits: " + Math.Round(academy.lastGenerationHitsPercent * 100, 2) + "%";
        bestGenerationHitsPercent.text = "Best Generation Hits: " + Math.Round(academy.bestGenerationHitsPercent * 100, 2) + "%";
    }
}
