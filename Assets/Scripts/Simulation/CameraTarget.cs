using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform target;
    public float height = -1f;

    void Update()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, height);
        }
    }
}
