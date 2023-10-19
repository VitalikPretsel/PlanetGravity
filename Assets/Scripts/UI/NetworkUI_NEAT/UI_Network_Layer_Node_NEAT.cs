using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_Network_Layer_Node_NEAT : MonoBehaviour
{
    public int ID;
    public List<Image> connections;
    public Color posColour;
    public Color negColour;


    public void DisplayConnections(
        UI_Network_NEAT ui_network, 
        NeuralNetworkNEAT network, 
        List<NeuralNetworkNEAT.Connection> networkConnections, 
        float scaleFactor)
    {
        if (networkConnections.Count > 0)
        {
            Image connectionTemplate = connections[0];
            for (int i = 0; i < networkConnections.Count - 1; i++)
            {
                Image newConnection = Instantiate(connectionTemplate);
                newConnection.transform.SetParent(this.transform, false);
                connections.Add(newConnection);
            }

            // Position Connections
            for (int i = 0; i < connections.Count; i++)
            {
                var networkNode = network.nodes[networkConnections[i].GetOutNode()];

                var level = ui_network.layersCount - 1;
                if (networkNode.outConnections.Count > 0)
                {
                    level = networkNode.GetLevel(network.nodes) - 1;
                }

                var otherNode = ui_network.GetNodeByID(level, networkNode.GetID());

                PositionConnections(connections[i], otherNode, networkConnections[i].weight, scaleFactor);
            }
        }
    }

    private void PositionConnections(Image connection, UI_Network_Layer_Node_NEAT otherNode, double weight, float scaleFactor)
    {
        //Set local position to 0
        connection.transform.localPosition = Vector3.zero;

        //Set connection width
        Vector2 sizeDelta = connection.rectTransform.sizeDelta;
        sizeDelta.x = (float)System.Math.Abs(weight * 4);
        if (sizeDelta.x < 1)
        {
            sizeDelta.x = 1;
        }

        //Set conenction color
        if (weight >= 0)
        {
            posColour.a = 1f;
            connection.color = posColour;
        }
        else
        {
            negColour.a = 1f;
            connection.color = negColour;
        }

        //Set connection length (height)
        Vector2 connectionVec = this.transform.position - otherNode.transform.position;
        sizeDelta.y = connectionVec.magnitude / scaleFactor;

        connection.rectTransform.sizeDelta = sizeDelta;

        //Set connection rotation
        float angle = Vector2.Angle(Vector2.up, connectionVec);
        connection.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
    }
}
