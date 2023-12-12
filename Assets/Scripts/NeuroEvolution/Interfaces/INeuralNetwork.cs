using System.Collections.Generic;

public interface INeuralNetwork
{
    double[] Run(List<double> input);
    float Fitness { get; set; }
}