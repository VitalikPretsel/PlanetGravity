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

    // Constructor creates randomly weighted neural networks
    public GeneticController(int popSize, float mutationRate, float crossoverRate, float mutationChange)
    {
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

    public void Crossover(List<double> mother, List<double> father)
    {
        List<double> tempM = new List<double>();
        List<double> tempF = new List<double>();
        for (int i = 0; i < mother.Count; i++)
        {
            if (crossoverRate > UnityEngine.Random.Range(0, 1f))
            {
                tempM.Add(father[i]);
                tempF.Add(mother[i]);
            }
            else
            {
                tempM.Add(mother[i]);
                tempF.Add(father[i]);
            }
        }

        mother.RemoveRange(0, mother.Count);
        father.RemoveRange(0, mother.Count);

        mother.InsertRange(0, tempF);
        father.InsertRange(0, tempM);
    }

    public NeuralNetwork[] Breed(NeuralNetwork mother, NeuralNetwork father)
    {
        NeuralNetwork child1 = new NeuralNetwork(mother.layerStructure);
        NeuralNetwork child2 = new NeuralNetwork(mother.layerStructure);

        List<double> motherChromosome = mother.Encode();
        List<double> fatherChromosome = father.Encode();

        Crossover(motherChromosome, fatherChromosome);

        child1.Decode(motherChromosome);
        child2.Decode(fatherChromosome);

        return new NeuralNetwork[] { child1, child2 };
    }

    public void Mutate(NeuralNetwork creature)
    {
        List<double> chromosome = creature.Encode();
        for (int i = 0; i < chromosome.Count; i++)
        {
            if (this.mutationRate > UnityEngine.Random.Range(0f, 1f))
            {
                chromosome[i] += mutationChange * UnityEngine.Random.Range(-1f, 1f);
                if (chromosome[i] > 1) chromosome[i] = 1;
                else if (chromosome[i] < -1) chromosome[i] = -1;
            }
        }

        creature.Decode(chromosome);
    }

    // Create next generation through Roulette-Wheel selection
    public void NextGeneration()
    {
        // Create a new generation 
        this.nextGeneration = new List<NeuralNetwork>();
        this.populationFitness = 0f;

        // Calculate population fitness
        for (int i = 0; i < this.population.Count; i++)
        {
            this.populationFitness += population[i].fitness;
        }

        // Calcualte fitness ratio for each population member
        for (int i = 0; i < this.population.Count; i++)
        {
            population[i].fitnessRatio = population[i].fitness / this.populationFitness;
        }

        // Calcuate the average fitness of the population
        averageFitness = (float)(this.populationFitness / this.population.Count);

        // Sort population list by fitness ratio
        population.Sort((x, y) => y.fitnessRatio.CompareTo(x.fitnessRatio));

        //Save the best creature for next gen
        nextGeneration.Add(population[0]);

        // Create 2 children for every breeding of NN
        for (int i = 0; i < this.population.Count / 2; i++)
        {
            // Select parents to breed
            int parent1Index = -1;
            int parent2Index = -1;
            double chance = UnityEngine.Random.Range(0f, 100f) / 100;
            double chance2 = UnityEngine.Random.Range(0f, 100f) / 100;
            double range = 0;

            for (int j = 0; j < this.population.Count; j++)
            {

                range += population[j].fitnessRatio;

                // This creature isnt selected move on
                if (chance > range && chance2 > range)
                {
                    continue;
                }

                // At this point one of the parents been selected
                if (chance <= range && parent1Index < 0)
                { // Parent 1 selected
                    parent1Index = j;
                }

                if (chance2 <= range && parent2Index < 0)
                { // Parent 2 selected
                    // Avoid two of the same parent
                    if (parent1Index == j)
                    {
                        // Parent 2 is the next availible parent
                        parent2Index = (j + 1) % population.Count;
                    }
                    else
                    {
                        parent2Index = j;
                    }
                }
                if (parent1Index >= 0 && parent2Index >= 0)
                {
                    break;

                }
            }

            // If somehow we have no parents chosen choose the worst parent
            if (parent1Index < 0)
            {
                parent1Index = this.population.Count - 1;
            }
            if (parent2Index < 0)
            {
                parent2Index = this.population.Count - 1;
            }

            //Breed the two selected parents and add them to the next generation
            NeuralNetwork[] children = Breed(population[parent1Index], population[parent2Index]);

            // Mutate children
            Mutate(children[0]);
            Mutate(children[1]);

            // Add the children to the next generation
            nextGeneration.Add(children[0]);
            nextGeneration.Add(children[1]);
        }

        // Make the children adults
        for (int i = 1; i < popSize; i++)
        {
            population[i] = nextGeneration[i];
        }
    }
}
