using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Genetics_NEAT : Abstract_UI_Genetics<NeuralNetworkNEAT> 
{
    public Text currentNodesNumber;
    public Text currentConnectionsNumber;
    public Text averageNodesNumber;
    public Text averageConnectionsNumber;

    public GameObject specieMarker;

    protected override void AdditionalUpdate()
    {
        averageConnectionsNumber.text = "Average Pop Connections: " + (academy as AcademyNEAT).averageConnectionsCount;
        averageNodesNumber.text = "Average Pop Nodes: " + (academy as AcademyNEAT).averageNodesCount;
        currentConnectionsNumber.text = "Current Connections: " + (academy as AcademyNEAT).currentConnectionsCount;
        currentNodesNumber.text = "Current Nodes: " + (academy as AcademyNEAT).currentNodesCount;
        
        specieMarker.GetComponent<Image>().color = (academy as AcademyNEAT).currentColor;
    }
}
