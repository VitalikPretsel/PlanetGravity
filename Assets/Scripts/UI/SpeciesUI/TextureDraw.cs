using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TextureDraw : MonoBehaviour
{
    private Texture2D texture;
    private float screenWidth, screenHeight;

    private Color backgroundColor;

    private int maxLoop = 10;

    List<SpeciesColorData> speciesData = new List<SpeciesColorData>();

    private int populationSize;
    

    public void SetPopSize(int popSize)
    {
        populationSize = popSize;
    }

    // Use this for initialization
    void Start()
    {
        texture = new Texture2D(1, 1);

        backgroundColor = Color.grey; 
        backgroundColor.a = 1f;
    }

    void OnGUI()
    {
        this.screenWidth = Screen.width * 0.27f;
        this.screenHeight = Screen.height;

        float offsetVertical = this.screenHeight * 0.62f;
        float offsetHorizontal = Screen.width * 0.73f;

        float height = (screenHeight * 0.015f);


        GUI.color = backgroundColor;
        GUI.DrawTexture(new Rect(0f + offsetHorizontal, 0f + offsetVertical, screenWidth, height * ((float)maxLoop - 1)), texture);

        if (speciesData != null && speciesData.Count > 0)
        {
            float xOffset = 0;
            float width = ((screenWidth) / (float)populationSize);

            for (int i = 0; i < speciesData.Count; i++)
            {
                xOffset = 0;
                for (int j = 0; j < speciesData[i].distributions.Length; j++)
                {
                    float totalWidth = width * speciesData[i].distributions[j];
                    GUI.color = speciesData[i].colors[j];
                    GUI.DrawTexture(new Rect(xOffset + offsetHorizontal, (int)height * i + offsetVertical, totalWidth, (int)height), texture);
                    xOffset += totalWidth;
                }
            }
        }
    }

    public void AddColorData(SpeciesColorData colordata)
    {
        if (this.speciesData.Count == 0)
        {
            for (var i = 0; i < maxLoop; i++)
            {
                this.speciesData.Add(colordata);
            }
        }
        else
        {
            this.speciesData.Insert(0, colordata);
            this.speciesData.RemoveAt(this.speciesData.Count - 1);
        }
    }
}
