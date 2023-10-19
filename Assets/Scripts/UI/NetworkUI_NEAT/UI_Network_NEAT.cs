using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UI_Network_NEAT : MonoBehaviour
{
    public List<UI_Network_Layer_NEAT> layers;
    public Canvas canvas;

    public int layersCount = 1;

    // Build and display Neural Network
    public void Display(NeuralNetworkNEAT network)
    {
        //Clear();

        layersCount = network.LayersCount();

        UI_Network_Layer_NEAT layerTemplate = layers[0];
        layerTemplate.networkNodes = new List<NeuralNetworkNEAT.Node>();

        for (int i = 0; i < layersCount - 1; i++)
        {
            UI_Network_Layer_NEAT newLayer = Instantiate(layerTemplate);
            newLayer.transform.SetParent(this.transform, false);
            newLayer.networkNodes = new List<NeuralNetworkNEAT.Node>();
            layers.Add(newLayer);
        }

        foreach (var node in network.nodes)
        {
            var level = layersCount - 1;
            if (node.Value.outConnections.Count > 0)
            {
                level = node.Value.GetLevel(network.nodes) - 1;
            }

            layers[level].networkNodes.Add(node.Value);
        }

        //Set input and hidden layer contents
        foreach (var l in layers)
        {
            l.Display();
        }

        // DrawConnections(network);
    }

    public UI_Network_Layer_Node_NEAT GetNodeByID(int level, int id)
    {
        return layers[level].GetNodeByID(id);
    }

    // Draw the connections (coroutine).
    public void DrawConnections(NeuralNetworkNEAT network)
    {
        //Draw node connections
        for (int l = 0; l < layers.Count - 1; l++) 
        {
            layers[l].DisplayConnections(this, network, canvas.scaleFactor);
        }
    }

    public void Clear()
    {
        for (int i = 1; i < layers.Count; i++)
        {
            Destroy(layers[i].gameObject);
            for (int j = 1; j < layers[i].nodes.Count; j++)
            {
                Destroy(layers[i].nodes[j].gameObject);
                for (int k = 1; j < layers[i].nodes[j].connections.Count; k++)
                {
                    Destroy(layers[i].nodes[j].connections[k].gameObject);
                }
            }
        }
        var firstLayer = layers[0];
        var firstNode = firstLayer.nodes[0];
        var firstConnection = firstNode.connections[0];
        firstNode.connections = new List<Image> { firstConnection };
        firstLayer.nodes = new List<UI_Network_Layer_Node_NEAT> { firstNode };
        firstLayer.networkNodes = new List<NeuralNetworkNEAT.Node>();
        layers = new List<UI_Network_Layer_NEAT> { firstLayer };
    }
}
