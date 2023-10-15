using UnityEngine;
using System.Collections.Generic;

public class GeneticController_2 : IGeneticController<Network>
{
    public List<Network> Networks { get; set; }

    private List<Genome> genomes;
    private Dictionary<Genome, Network> networkMap;
    private Dictionary<Genome, Species> speciesMap;
    private List<Species> speciesList;

    public int population;

    public float C1 = 1f;
    public float C2 = 1f;
    public float C3 = 0.3f;

    public float compatibilityThreshold = 3f;
    public float survivalChance = 0.1f;
    public float weightMutationChance = 0.8f;
    public float randomWeightChance = 0.1f;
    public float addNodeChance = 0.03f;
    public float addConnectionChance = 0.05f;

    public int inputNodes = 8;
    public int outputNodes = 3;

    public float populationFitness;
    public float AverageFitness { get; set; }

    // Constructor creates randomly weighted neural networks
    public GeneticController_2(
        int popSize,
        float C1,
        float C2,
        float C3,
        float compatibilityThreshold,
        float survivalChance, 
        float weightMutationChance, 
        float randomWeightChance, 
        float addNodeChance, 
        float addConnectionChance)
    {
        this.C1 = C1;
        this.C2 = C2;
        this.C3 = C3;
        this.compatibilityThreshold = compatibilityThreshold;
        this.survivalChance = survivalChance;
        this.weightMutationChance = weightMutationChance;
        this.randomWeightChance = randomWeightChance;
        this.addNodeChance = addNodeChance;
        this.addConnectionChance = addConnectionChance;
        this.population = popSize;

        this.populationFitness = 0f;
        this.AverageFitness = 0f;

        genomes = new List<Genome>();
        speciesList = new List<Species>();

        System.Random r = new System.Random();
        for (int i = 0; i < population; i++)
        {
            Genome genome = new Genome(inputNodes, outputNodes, r);
            genomes.Add(genome);
        }

        AssignSpecies();
        MakeNetworks();
    }

    public void NextGeneration()
    {
        CalculateFitnessStats();

        SortNets();

        float totalFitness = 0;
        float leftPopulation = population * (1 - survivalChance);
        List<Genome> nextGenomes = new List<Genome>();

        foreach (Species species in speciesList)
        {
            totalFitness += species.GetFitness();
        }

        for (int i = 0; i < (int)(population * survivalChance); i++)
        {
            nextGenomes.Add(Networks[i].genome);
        }

        System.Random r = new System.Random();

        foreach (Species species in speciesList)
        {
            for (int i = 0; i < (int)(species.GetFitness() / totalFitness * leftPopulation); i++)
            {
                Genome parent1 = species.GetRandomGenome(r);
                Genome parent2 = species.GetRandomGenome(r);
                Genome child = new Genome();

                if (networkMap[parent1].Fitness > networkMap[parent2].Fitness)
                {
                    child = GenomeUtils.Crossover(parent1, parent2, r);
                }
                else
                {
                    child = GenomeUtils.Crossover(parent2, parent1, r);
                }
                nextGenomes.Add(child);
            }
        }

        while (nextGenomes.Count < population)
        {
            Genome parent1 = speciesList[0].GetRandomGenome(r);
            Genome parent2 = speciesList[0].GetRandomGenome(r);
            Genome child = new Genome();

            if (networkMap[parent1].Fitness > networkMap[parent2].Fitness)
            {
                child = GenomeUtils.Crossover(parent1, parent2, r);
            }

            else
            {
                child = GenomeUtils.Crossover(parent2, parent1, r);
            }

            nextGenomes.Add(child);
        }

        foreach (Genome genome in nextGenomes)
        {
            double roll = r.NextDouble();

            if (roll < weightMutationChance)
            {
                genome.Mutate(randomWeightChance, r);
            }
            else if (roll < weightMutationChance + addNodeChance)
            {
                genome.AddNodeMutation(r);
            }
            else if (roll < weightMutationChance + addNodeChance + addConnectionChance)
            {
                genome.AddConnectionMutation(r);
            }
        }

        foreach (Species species in speciesList)
        {
            species.Reset();
        }
        genomes = nextGenomes;


        AssignSpecies();
        MakeNetworks();
    }

    private void AssignSpecies()
    {
        speciesMap = new Dictionary<Genome, Species>();
        foreach (Genome gen in genomes)
        {
            bool found = false;
            foreach (Species species in speciesList)
            {
                float distance = GenomeUtils.CompatiblityDistance(gen, species.GetMascot(), C1, C2, C3);
                if (distance < compatibilityThreshold)
                {
                    species.AddMember(gen);
                    speciesMap.Add(gen, species);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Species species = new Species(gen);
                speciesList.Add(species);
                speciesMap.Add(gen, species);
            }
        }

        System.Random r = new System.Random();

        for (int i = speciesList.Count - 1; i >= 0; i--)
        {
            if (speciesList[i].GetCount() == 0)
            {
                speciesList.RemoveAt(i);
            }
            else
            {
                speciesList[i].RandomizeMascot(r);
            }
        }

        //Debug.Log("Gen: " + generation + ", Population: " + population + ", Species: " + speciesList.Count);
    }

    private void SortNets()
    {
        foreach (Network net in Networks)
        {
            net.Fitness = (net.Fitness / speciesMap[net.genome].GetCount());
            speciesMap[net.genome].AddFitness(net.Fitness);
        }

        Networks.Sort();
        speciesList.Sort();
    }

    private void MakeNetworks()
    {
        Networks = new List<Network>();
        networkMap = new Dictionary<Genome, Network>();

        foreach (Genome genome in genomes)
        {
            Network net = new Network(genome);
            Networks.Add(net);
            networkMap.Add(genome, net);
        }
    }

    private void CalculateFitnessStats()
    {
        populationFitness = 0f;
        for (int i = 0; i < Networks.Count; i++)
        {
            populationFitness += Networks[i].Fitness;
        }

        AverageFitness = (float)(populationFitness / Networks.Count);
    }
}
