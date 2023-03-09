using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollideBlocker : MonoBehaviour
{
    Collider2D parentCollider;
    Collider2D childCollider;

    void Start()
    {
        childCollider = GetComponent<Collider2D>();
        parentCollider = GetComponentInParent<Collider2D>();
        Physics2D.IgnoreCollision(childCollider, parentCollider, true);
    }
}
