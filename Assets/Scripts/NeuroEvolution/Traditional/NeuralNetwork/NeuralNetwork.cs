﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class NeuralNetwork: INeuralNetwork
{
    public List<Layer> layers;
    public int[] layerStructure;    // number of neurons in layers
    public float Fitness { get; set; }

    public int NumLayers()
    {
        return layers.Count;
    }

    // Constructor creates a random NN with specified layer structure 
    public NeuralNetwork(int[] layers)
    {
        // We must have at least 2 layers (input, output)
        if (layers.Length < 2)
        {
            return;
        }

        this.layers = new List<Layer>();
        this.layerStructure = layers;
        this.Fitness = 0f;

        // Initalize NN with layers of neurons
        for (int i = 0; i < layers.Length; i++)
        {
            // Make a new layer with right number of neurons
            Layer currentLayer = new Layer(layers[i], i);
            this.layers.Add(currentLayer);

            // Add neurons to current layer
            for (int n = 0; n < layers[i]; n++)
            {
                currentLayer.neurons.Add(new Neuron());
            }

            // Initalize neurons
            foreach (Neuron neuron in currentLayer.neurons)
            {
                if (i == 0) // if we are the first layer set our bias to 0
                {
                    neuron.bias = 0;
                }
                else
                {
                    for (int d = 0; d < layers[i - 1]; d++) // for each neuron create dendrite to other neurons
                    {
                        neuron.dendrites.Add(new Dendrite());
                    }
                }
            }
        }
    }

    // Constructor reads in a specified filename and creates a NN from the encoded String
    public NeuralNetwork(String fileName)
    {
        string[] lines = File.ReadAllLines(fileName);
        // Get network structure
        string[] structure = lines[0].Split(new char[] { ',' });
        int[] numStrucutre = new int[structure.Length];
        for (int i = 0; i < structure.Length; i++)
        {
            numStrucutre[i] = System.Convert.ToInt32(structure[i]);
        }

        // Make NN with those specifications
        NeuralNetwork NN = new NeuralNetwork(numStrucutre);

        // Get the encoded value
        string[] element = lines[1].Split(new char[] { ',' });

        List<Double> encoded = new List<double>();
        for (int i = 0; i < element.Length; i++)
        {
            encoded.Add(Convert.ToDouble(element[i]));
            //Debug.Log(encoded[i]);

        }

        //Update NN with the value
        NN.Decode(encoded);
        this.layers = NN.layers;
        this.layerStructure = NN.layerStructure;
        this.Fitness = 0f;
    }

    // Activation Functions
    public double Sigmoid(double x)
    {
        return 1 / (1 + Math.Exp(-x));
    }
    double Tanh(double x)
    {
        return System.Math.Tanh(x);
    }
    double TanhNorm(double x)
    {
        return System.Math.Tanh(x) / System.Math.Tanh(1);
    }

    // Encode Neural network into a chromosome that we can evolve
    public List<double> Encode()
    {
        List<double> chromosome = new List<double>();
        // Get data from NN for chromosome
        for (int i = 1; i < layers.Count; i++)
        {
            for (int j = 0; j < layers[i].neurons.Count; j++)
            {
                // Add the neuron's bias to the chromosome
                chromosome.Add(layers[i].neurons[j].bias);
                // Add each weight input to the chromosome
                for (int k = 0; k < layers[i].neurons[j].NumDendrites(); k++)
                {
                    chromosome.Add(layers[i].neurons[j].dendrites[k].weight);
                }
            }
        }
        return chromosome;
    }

    // Apply new chromosome to NN
    public void Decode(List<double> chromosome)
    {
        int geneIndex = 0;

        for (int i = 1; i < layers.Count; i++)
        {
            for (int j = 0; j < layers[i].neurons.Count; j++)
            {
                layers[i].neurons[j].bias = chromosome[geneIndex];
                geneIndex++;
                // Add each weight input to the neuron
                for (int k = 0; k < layers[i].neurons[j].NumDendrites(); k++)
                {
                    layers[i].neurons[j].dendrites[k].weight = chromosome[geneIndex];
                    geneIndex++;
                }
            }
        }
    }

    // Run the NN
    public double[] Run(List<double> input)
    {
        // Check to see if input is right size
        if (input.Count != this.layers[0].neurons.Count)
        {
            return null;
        }

        // Pass input through each layer of network
        for (int l = 0; l < layers.Count; l++)
        {
            Layer currentLayer = layers[l];

            for (int n = 0; n < currentLayer.neurons.Count; n++)
            {
                Neuron neuron = currentLayer.neurons[n];

                // If first layer pass in input
                if (l == 0)
                {
                    neuron.value = input[n];
                }
                else
                {
                    // Get weighted value from all neruons connected to it
                    neuron.value = 0;
                    for (int lastNeuron = 0; lastNeuron < this.layers[l - 1].neurons.Count; lastNeuron++)
                    {
                        neuron.value += this.layers[l - 1].neurons[lastNeuron].value * neuron.dendrites[lastNeuron].weight;
                    }

                    neuron.value = Tanh(neuron.value + neuron.bias); // or sigmoid
                }
            }
        }

        // Return output
        Layer lastLayer = this.layers[this.layers.Count - 1];
        int numOutput = lastLayer.neurons.Count;
        double[] output = new double[numOutput];
        for (int i = 0; i < numOutput; i++)
        {
            output[i] = lastLayer.neurons[i].value;
        }
        return output;
    }

    // Saves the encoded genes to a file
    public void Save(int network_number)
    {
        StreamWriter write = new StreamWriter("./Saves/NetworkSaves/nn" + network_number + ".txt", true);

        // Write out layer structure
        for (int i = 0; i < layerStructure.Length - 1; i++)
        {
            write.Write(layerStructure[i] + ", ");
        }
        write.Write(layerStructure[layerStructure.Length - 1] + "\n");

        // Write out encoded NN
        List<double> encoded = this.Encode();
        for (int i = 0; i < encoded.Count - 1; i++)
        {
            write.Write(encoded[i] + ", ");
        }
        write.Write(encoded[encoded.Count - 1]);

        write.Close();
    }
}

