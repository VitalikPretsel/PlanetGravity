using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Genetics_NEAT : Abstract_UI_Genetics<NeuralNetworkNEAT> 
{
    public Text currentGenomeInnovationsNumber;
    public GameObject specieMarker;

    protected override void AdditionalUpdate()
    {
        if (academy.bestRocket is not null)
        {
            currentGenomeInnovationsNumber.text = "Current Innovations: " + (academy.bestRocket.network as NeuralNetworkNEAT).genome.GetInnovationsCount();
            specieMarker.GetComponent<Image>().color = (academy.bestRocket.network as NeuralNetworkNEAT).genome.color;
        }
    }
}
