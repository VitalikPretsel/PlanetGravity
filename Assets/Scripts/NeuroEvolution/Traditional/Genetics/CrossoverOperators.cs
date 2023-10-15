using UnityEngine;
using System.Collections.Generic;

public static class CrossoverOperators
{
    public static List<List<double>> UniformCrossover(List<double> parent1, List<double> parent2, double crossoverRate)
    {
        var siblings = new List<List<double>>() {
            new List<double>(),
            new List<double>()
        };

        for (int i = 0; i < parent1.Count; i++)
        {
            if (crossoverRate > UnityEngine.Random.Range(0, 1f))
            {
                siblings[0].Add(parent2[i]);
                siblings[1].Add(parent1[i]);
            }
            else
            {
                siblings[0].Add(parent1[i]);
                siblings[1].Add(parent2[i]);
            }
        }

        return siblings; 
    }
}