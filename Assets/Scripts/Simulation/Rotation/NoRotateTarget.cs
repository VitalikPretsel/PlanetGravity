using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRotateTarget : MonoBehaviour
{
    private Quaternion myRotation;

    void Start()
    {
        myRotation = this.transform.rotation;
    }

    void FixedUpdate()
    {
        this.transform.rotation = myRotation;
    }
}
