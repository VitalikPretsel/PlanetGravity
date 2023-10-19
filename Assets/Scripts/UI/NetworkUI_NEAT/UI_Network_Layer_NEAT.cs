using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UI_Network_Layer_NEAT : MonoBehaviour
{
    public List<NeuralNetworkNEAT.Node> networkNodes;

    public RectTransform contents;
    public List<UI_Network_Layer_Node_NEAT> nodes;


    public void Display()
    {
        UI_Network_Layer_Node_NEAT nodeTemplate = nodes[0];

        nodes[0].ID = networkNodes[0].GetID();

        for (int i = 1; i < networkNodes.Count; i++)
        {
            UI_Network_Layer_Node_NEAT node = Instantiate(nodeTemplate);
            node.ID = networkNodes[i].GetID();
            node.transform.SetParent(contents.transform, false);
            nodes.Add(node);
        }
    }

    public UI_Network_Layer_Node_NEAT GetNodeByID(int id)
    {
        return nodes.FirstOrDefault(n => n.ID == id);
    }

    public void DisplayConnections(UI_Network_NEAT ui_network, NeuralNetworkNEAT network, float scaleFactor)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].DisplayConnections(ui_network, network, networkNodes[i].outConnections, scaleFactor);
        }
    }
}
