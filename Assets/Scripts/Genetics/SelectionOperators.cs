using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class SelectionOperators
{
    public static List<List<int>> RouletteWheelSelection(List<NeuralNetwork> population)
    {
        List<List<int>> selectedIndexes = new List<List<int>>();

        List<double> cumulativeFitness = new List<double>();
        double sum = 0;
        foreach (var individual in population)
        {
            sum += individual.fitness;
            cumulativeFitness.Add(sum);
        }
        cumulativeFitness = cumulativeFitness.Select(f => f / sum).ToList();

        for (int i = 0; i < population.Count / 2; i++)
        {
            List<int> parents = new List<int>();

            for (int p = 0; p < 2; p++)
            {
                int selectedIndex = 0;

                double randomValue = UnityEngine.Random.Range(0f, 1f);

                for (int j = 0; j < cumulativeFitness.Count; j++)
                {
                    if (randomValue <= cumulativeFitness[j])
                    {
                        selectedIndex = j;
                        break;
                    }
                }

                if (parents.Contains(selectedIndex))
                {
                    selectedIndex = selectedIndex == population.Count - 1
                        ? selectedIndex - 1 : selectedIndex + 1;
                }

                parents.Add(selectedIndex);
            }

            selectedIndexes.Add(parents);
        }

        return selectedIndexes;
    }
}