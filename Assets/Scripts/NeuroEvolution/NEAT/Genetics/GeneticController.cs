using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GeneticControllerNEAT : IGeneticController<NeuralNetworkNEAT>
{
    public List<NeuralNetworkNEAT> Networks { get; set; }

    private List<Genome> genomes;
    private Dictionary<Genome, NeuralNetworkNEAT> networkMap;
    private Dictionary<Genome, Species> speciesMap;
    public List<Species> speciesList;

    public int population;

    public float C1 = 1f;
    public float C2 = 1f;
    public float C3 = 0.3f;

    public float compatibilityThreshold;
    public float weightMutationChance;
    public float weightMutationChange;
    public float eachWeightMutationChance;
    public float randomWeightChance;
    public float addNodeChance;
    public float addConnectionChance;
    public float crossoverRate;
    public float interSpeciesCrossoverRate;

    public int inputNodes = 8;
    public int outputNodes = 3;

    public float populationFitness;
    public float AverageFitness { get; set; }

    public float AverageNodesCount { get => genomes.Select(g => (float)g.GetNodesCount()).Average(); }
    public float AverageConnectionsCount { get => genomes.Select(g => (float)g.GetConnectionsCount()).Average(); }

    // Constructor creates randomly weighted neural networks
    public GeneticControllerNEAT(
        int popSize,
        float C1,
        float C2,
        float C3,
        float compatibilityThreshold,
        float weightMutationChance,
        float weightMutationChange,
        float eachWeightMutationChance,
        float randomWeightChance, 
        float addNodeChance, 
        float addConnectionChance,
        float crossoverRate,
        float interSpeciesCrossoverRate)
    {
        this.C1 = C1;
        this.C2 = C2;
        this.C3 = C3;
        this.compatibilityThreshold = compatibilityThreshold;
        this.weightMutationChance = weightMutationChance;
        this.weightMutationChange = weightMutationChange;
        this.eachWeightMutationChance = eachWeightMutationChance;
        this.randomWeightChance = randomWeightChance;
        this.addNodeChance = addNodeChance;
        this.addConnectionChance = addConnectionChance;
        this.crossoverRate = crossoverRate;
        this.interSpeciesCrossoverRate = interSpeciesCrossoverRate;
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

        List<Genome> nextGenomes = new List<Genome>();

        // save best in each species 
        int speciesChampionsCount = 0;
        foreach (Species species in speciesList)
        {
            if (species.members.Count > 5)
            {
                nextGenomes.Add(species.members.OrderByDescending(g => networkMap[g].Fitness).First());
                speciesChampionsCount += 1;
            }
        }

        //// save best without mating and mutations
        //for (int i = 0; i < (int)(population * survivalChance); i++)
        //{
        //    nextGenomes.Add(Networks[i].genome);
        //}

        System.Random r = new System.Random();

        // selection and mating
        List<double> cumulativeFitness = new List<double>();
        double sum = 0;
        foreach (var individual in speciesList)
        {
            sum += individual.GetFitness();
            cumulativeFitness.Add(sum);
        }
        cumulativeFitness = cumulativeFitness.Select(f => f / sum).ToList();

        while (nextGenomes.Count < population)
        {
            var index = PickIndexFromCumulFitness(cumulativeFitness, r);

            if (r.NextDouble() < crossoverRate)
            {
                var index2 = index;

                if (r.NextDouble() < interSpeciesCrossoverRate)
                {
                    index2 = PickIndexFromCumulFitness(cumulativeFitness, r);
                }

                nextGenomes.Add(CrossoverFromSpecies(speciesList[index], speciesList[index2], r));
            }
            else
            {
                nextGenomes.Add(CopyGenome(speciesList[index], r));
            }
        }

        // mutation
        for (int i = speciesChampionsCount; i < nextGenomes.Count; i++)
        {
            double roll = r.NextDouble();

            if (roll < weightMutationChance)
            {
                nextGenomes[i].Mutate(eachWeightMutationChance, randomWeightChance, weightMutationChange, r);
            }
            else if (roll < weightMutationChance + addNodeChance)
            {
                nextGenomes[i].AddNodeMutation(r);
            }
            else if (roll < weightMutationChance + addNodeChance + addConnectionChance)
            {
                nextGenomes[i].AddConnectionMutation(r);
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



    private Genome CrossoverFromSpecies(Species species1, Species species2, System.Random r)
    {
        Genome parent1 = species1.GetRandomGenome(r);
        Genome parent2 = species2.GetRandomGenome(r);

        if (networkMap[parent1].Fitness > networkMap[parent2].Fitness)
        {
            return GenomeUtils.Crossover(parent1, parent2, r);
        }
        else
        {
            return GenomeUtils.Crossover(parent2, parent1, r);
        }
    }

    private Genome CopyGenome(Species species, System.Random r)
    {
        var parent = species.GetRandomGenome(r);
        
        Genome child = new Genome();

        List<Genome.NodeGene> nodes = parent.GetNodes();
        Dictionary<int, Genome.ConnectionGene> connections = parent.GetConnections();

        foreach (Genome.NodeGene node in nodes)
        {
            child.AddNode(node);
        }

        foreach (Genome.ConnectionGene con in connections.Values)
        {
            child.AddConnection(con);
        }

        return child;
    }

    private int PickIndexFromCumulFitness(List<double> cumulativeFitness, System.Random r)
    {
        var rand = r.NextDouble();
        var index = 0;

        while (rand > cumulativeFitness[index])
        {
            index++;
        }

        return index;
    }

    private void AssignSpecies()
    {
        speciesMap = new Dictionary<Genome, Species>();
        foreach (Genome gen in genomes)
        {
            bool found = false;
            foreach (Species species in speciesList)
            {
                double distance = GenomeUtils.CompatiblityDistance(gen, species.GetMascot(), C1, C2, C3);
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
        foreach (NeuralNetworkNEAT net in Networks)
        {
            net.Fitness = (net.Fitness / speciesMap[net.genome].GetCount());
            speciesMap[net.genome].AddFitness(net.Fitness);
        }

        Networks.Sort();
        speciesList.Sort();
    }

    private void MakeNetworks()
    {
        Networks = new List<NeuralNetworkNEAT>();
        networkMap = new Dictionary<Genome, NeuralNetworkNEAT>();

        foreach (Genome genome in genomes)
        {
            NeuralNetworkNEAT net = new NeuralNetworkNEAT(genome);
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
