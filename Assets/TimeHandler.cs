using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeHandler : MonoBehaviour
{
    public float timeScale = 1;

    void FixedUpdate()
    {
        Time.timeScale = timeScale;
    }
}
