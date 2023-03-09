using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RotationTarget : MonoBehaviour
{
    Rigidbody2D rigidBody;

    [SerializeField] public float rotationAngleSpeed;
    private float rotation = 0;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rigidBody.SetRotation(rotation);
        rotation += rotationAngleSpeed;
    }
}
