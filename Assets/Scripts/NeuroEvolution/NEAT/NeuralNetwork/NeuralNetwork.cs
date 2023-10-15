using System;
using System.Collections.Generic;

[Serializable]
public class Network : IComparable<Network>, INeuralNetwork
{
    public Genome genome { get; set; }
    private List<Genome.NodeGene> nodeGenes;
    private Dictionary<int, Genome.ConnectionGene> connectionGenes;
    private Dictionary<int, Node> nodes;
    private List<Node> inputNodes;
    private List<Node> outputNodes;
    private List<Node> hiddenNodes;
    public float Fitness { get; set; }

    public Network(Genome gen)
    {
        nodes = new Dictionary<int, Node>();
        inputNodes = new List<Node>();
        outputNodes = new List<Node>();
        hiddenNodes = new List<Node>();
        genome = gen;
        nodeGenes = genome.GetNodes();
        connectionGenes = genome.GetConnections();

        MakeNetwork();
    }

    public class Node
    {
        int ID;
        double value;
        List<Connection> inConnections;
        List<Connection> outConnections;

        public Node(int id)
        {
            ID = id;
            value = 0f;
            inConnections = new List<Connection>();
            outConnections = new List<Connection>();
        }

        public int GetID()
        {
            return ID;
        }

        public bool Ready()
        {
            bool ready = true;
            foreach (Connection con in inConnections)
            {
                if (!con.GetStatus())
                {
                    ready = false;
                    break;
                }
            }
            return ready;
        }

        public void AddInConnection(Connection con)
        {
            inConnections.Add(con);
        }

        public void AddOutConnection(Connection con)
        {
            outConnections.Add(con);
        }

        public double GetValue()
        {
            return value;
        }

        public void SetValue(double val)
        {
            value = Math.Tanh(val);
        }

        public void CalculateValue()
        {
            foreach (Connection con in inConnections)
            {
                value += con.GetValue();
            }
            value = Math.Tanh(value);
        }

        public void TransmitValue()
        {
            foreach (Connection con in outConnections)
            {
                con.SetValue(value);
            }
            value = 0;
        }
    }

    public class Connection
    {
        private int inNode;
        private int outNode;
        private double value;
        private double weight;
        private bool ready;

        public Connection(int input, int output, double weight)
        {
            inNode = input;
            outNode = output;
            value = 0;
            ready = false;
            this.weight = weight;
        }

        public Connection(Genome.ConnectionGene con)
        {
            inNode = con.GetInNode();
            outNode = con.GetOutNode();
            value = 0;
            ready = false;
            weight = con.GetWeight();
        }

        public int GetInNode()
        {
            return inNode;
        }

        public int GetOutNode()
        {
            return outNode;
        }

        public double GetValue()
        {
            double val = value;
            ready = false;
            value = 0;
            return val * weight;
        }

        public void SetValue(double val)
        {
            value = val;
            ready = true;
        }

        public bool GetStatus()
        {
            return ready;
        }
    }

    private void MakeNetwork()
    {
        foreach (Genome.NodeGene nodeGene in nodeGenes)
        {
            Node node = new Node(nodeGene.GetID());
            //nodes.Add(node);
            if (nodeGene.GetNodeType() == Genome.NodeGene.TYPE.INPUT)
            {
                inputNodes.Add(node);
            }
            else if (nodeGene.GetNodeType() == Genome.NodeGene.TYPE.OUTPUT)
            {
                outputNodes.Add(node);
            }
            else
            {
                hiddenNodes.Add(node);
            }
            nodes.Add(nodeGene.GetID(), node);
        }

        foreach (Genome.ConnectionGene conGene in connectionGenes.Values)
        {
            if (conGene.IsExpressed())
            {
                Connection con = new Connection(conGene);
                nodes[con.GetInNode()].AddOutConnection(con);
                nodes[con.GetOutNode()].AddInConnection(con);
            }
        }
    }

    public double[] Run(List<double> input)
    {
        double[] output = new double[outputNodes.Count];
        for (int i = 0; i < inputNodes.Count; i++)
        {
            inputNodes[i].SetValue(input[i]);
            inputNodes[i].TransmitValue();
        }

        List<Node> copyList = new List<Node>(hiddenNodes);

        while (copyList.Count != 0)
        {
            List<Node> removeNodes = new List<Node>();
            foreach (Node node in copyList)
            {
                if (node.Ready())
                {
                    node.CalculateValue();
                    node.TransmitValue();
                    removeNodes.Add(node);
                }
            }

            foreach (Node node in removeNodes)
            {
                copyList.Remove(node);
            }
        }

        for (int i = 0; i < outputNodes.Count; i++)
        {
            outputNodes[i].CalculateValue();
            output[i] = outputNodes[i].GetValue();
        }

        return output;
    }

    public int CompareTo(Network other)
    {
        return other.Fitness.CompareTo(Fitness);
    }
}