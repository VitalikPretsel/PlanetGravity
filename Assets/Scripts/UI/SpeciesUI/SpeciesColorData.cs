using System.Collections.Generic;
using UnityEngine;

public class SpeciesColorData
{
    public Color[] colors;
    public int[] distributions;

    public SpeciesColorData(List<Species> speciesList)
    {
        List<Color> colorList = new List<Color>();
        List<int> distributionList = new List<int>();

        for (int i = 0; i < speciesList.Count; i++)
        {
            colorList.Add(speciesList[i].color);
            distributionList.Add(speciesList[i].members.Count);
        }

        colors = colorList.ToArray();
        distributions = distributionList.ToArray();
    }
}

