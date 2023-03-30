using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Speed : MonoBehaviour
{
    public Rigidbody2D rocket;
    public Text speedText;

    void Update()
    {
        speedText.text = "Speed: " + rocket.velocity.magnitude.ToString();
    }
}
