using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRotateTarget : MonoBehaviour
{
    private Quaternion my_rotation;
    
    void Start()
    {
        my_rotation = this.transform.rotation;
    }

    void FixedUpdate()
    {
        this.transform.rotation = my_rotation;
    }
}
