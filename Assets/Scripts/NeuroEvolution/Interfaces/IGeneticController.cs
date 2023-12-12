using System.Collections.Generic;

public interface IGeneticController<T> where T : INeuralNetwork
{
    void NextGeneration();
    float AverageFitness { get; set; }
    List<T> Networks { get; set; }
}