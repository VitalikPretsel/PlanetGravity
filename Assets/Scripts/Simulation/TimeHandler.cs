using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeHandler : MonoBehaviour
{
    public float time = 0f;

    public float timeScale = 1;

    void FixedUpdate()
    {
        if (timeScale >= 0)
        {
            Time.timeScale = timeScale;
        }
        time += 0.02f;
    }
}
