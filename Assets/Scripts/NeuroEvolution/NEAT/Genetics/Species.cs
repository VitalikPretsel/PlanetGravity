using System;
using System.Collections.Generic;

public class Species : IComparable<Species>
{
    private Genome mascot;
    public List<Genome> members;
    private float fitness;
    public UnityEngine.Color color;


    public Species(Genome firstMember)
    {
        color = new UnityEngine.Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        firstMember.color = color;
        members = new List<Genome>
            {
                firstMember
            };
        mascot = firstMember;
        fitness = 0;
    }

    public Genome GetRandomGenome(Random r)
    {
        return members[r.Next(members.Count)];
    }

    public void RandomizeMascot(Random r)
    {
        mascot = members[r.Next(members.Count)];
    }

    public void AddMember(Genome genome)
    {
        genome.color = color;
        members.Add(genome);
    }

    public void AddFitness(float fit)
    {
        fitness += fit;
    }

    public float GetFitness()
    {
        return fitness;
    }

    public int GetCount()
    {
        return members.Count;
    }

    public Genome GetMascot()
    {
        return mascot;
    }

    public void Reset()
    {
        members.Clear();
        fitness = 0;
    }

    public int CompareTo(Species other)
    {
        return other.GetFitness().CompareTo(fitness);
    }
}