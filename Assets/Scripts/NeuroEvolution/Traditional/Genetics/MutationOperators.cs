using UnityEngine;
using System.Collections.Generic;

public static class MutationOperators
{
    public static List<double> AdditiveMutation(List<double> chromosome, double mutationRate, double mutationChange)
    {
        List<double> mutated = new List<double>(chromosome);

        for (int i = 0; i < mutated.Count; i++)
        {
            if (mutationRate > UnityEngine.Random.Range(0f, 1f))
            {
                mutated[i] += mutationChange * UnityEngine.Random.Range(-1f, 1f);
                
                if (mutated[i] > 1)
                {
                    mutated[i] = 1;
                }
                else if (mutated[i] < -1)
                {
                    mutated[i] = -1;
                }
            }
        }

        return mutated;
    }
}