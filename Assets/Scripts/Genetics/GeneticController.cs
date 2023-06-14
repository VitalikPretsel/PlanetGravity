using UnityEngine;
using System.Collections.Generic;

public class GeneticController
{
    public List<NeuralNetwork> population;
    public List<NeuralNetwork> nextGeneration;

    private double populationFitness;

    public float mutationRate;
    public float mutationChange;
    public float crossoverRate;
    
    public float averageFitness;
    int popSize;

    private delegate List<List<int>> SelectionDelegate(List<NeuralNetwork> population);
    private SelectionDelegate Selection;

    private delegate List<List<double>> CrossoverDelegate(List<double> parent1, List<double> parent2, double crossoverRate);
    private CrossoverDelegate Crossover;
    
    private delegate List<double> MutationDelegate(List<double> chromosome, double mutationRate, double mutationChange);
    private MutationDelegate Mutation;

    // Constructor creates randomly weighted neural networks
    public GeneticController(int popSize, float mutationRate, float crossoverRate, float mutationChange)
    {
        Selection = SelectionOperators.RouletteWheelSelection;
        Crossover = CrossoverOperators.UniformCrossover;
        Mutation = MutationOperators.AdditiveMutation;

        this.population = new List<NeuralNetwork>(popSize);
        this.populationFitness = 0f;
        this.mutationRate = mutationRate;
        this.crossoverRate = crossoverRate;
        this.mutationChange = mutationChange;
        this.averageFitness = 0f;
        this.popSize = popSize;

        for (int i = 0; i < popSize; i++)
        {
            // Create NN with specific structure
            this.population.Add(new NeuralNetwork(new int[] { 8, 7, 3 })); // 8,7,3 // 8,15,15,3 // 8,11,11,3
        }
    }

    public NeuralNetwork[] Breed(NeuralNetwork parent1, NeuralNetwork parent2)
    {
        var siblings = new NeuralNetwork[] { 
            new NeuralNetwork(parent1.layerStructure), 
            new NeuralNetwork(parent2.layerStructure) 
        };

        var siblingChromosomes = Crossover(parent1.Encode(), parent2.Encode(), crossoverRate);

        siblings[0].Decode(siblingChromosomes[0]);
        siblings[1].Decode(siblingChromosomes[1]);

        return siblings;
    }

    public void Mutate(NeuralNetwork creature)
    {
        creature.Decode(
            Mutation(creature.Encode(), mutationRate, mutationChange));
    }
    
    public void NextGeneration()
    {
        CalculateFitnessStats();

        nextGeneration = new List<NeuralNetwork>();

        population.Sort((x, y) => y.fitness.CompareTo(x.fitness));
        nextGeneration.Add(population[0]);  // to guarantee that best solution survives

        List<List<int>> selectedIndexes = Selection(population);

        foreach (var parents in selectedIndexes)
        {
            NeuralNetwork[] children = Breed(population[parents[0]], population[parents[1]]);

            Mutate(children[0]);
            Mutate(children[1]);

            nextGeneration.Add(children[0]);
            nextGeneration.Add(children[1]);
        }

        for (int i = 1; i < popSize; i++)
        {
            population[i] = nextGeneration[i];
        }
    }

    public void CalculateFitnessStats()
    {
        populationFitness = 0f;
        for (int i = 0; i < population.Count; i++)
        {
            populationFitness += population[i].fitness;
        }

        averageFitness = (float)(populationFitness / population.Count);
    }
}
