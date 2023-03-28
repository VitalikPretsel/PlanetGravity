using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityHandler : MonoBehaviour
{
    [SerializeField] float g = 1; // 6.67384e-11f
    static float G;

    public static bool isSimulatingLive = true;

    //in physical universe every body would be both attractor and attractee
    public static List<GravityTarget> attractors = new List<GravityTarget>();
    public static List<GravityTarget> attractees = new List<GravityTarget>();

    void FixedUpdate()
    {
        G = g;  //in case g is changed in editor
        if (isSimulatingLive)   //PathHandler changes this
        {
            SimulateGravities();
        }
    }

    public static void SimulateGravities()
    {
        foreach (GravityTarget attractor in attractors)
        {
            foreach (GravityTarget attractee in attractees)
            {
                if (attractor != attractee)
                {
                    var dist = Vector3.Distance(attractor.rigidBody.position, attractee.rigidBody.position);

                    if (dist > attractor.GetComponent<Collider2D>().bounds.size.x && dist > attractor.GetComponent<Collider2D>().bounds.size.y
                        && dist > attractee.GetComponent<Collider2D>().bounds.size.x && dist > attractee.GetComponent<Collider2D>().bounds.size.y)
                    {
                        AddGravityForce(attractor.rigidBody, attractee.rigidBody);
                    }
                }
            }
        }
    }

    public static void AddGravityForce(Rigidbody2D attractor, Rigidbody2D target)
    {
        double massProduct = attractor.mass * target.mass;

        Vector3 difference = attractor.position - target.position;
        float distance = difference.magnitude; // r = Mathf.Sqrt((x*x)+(y*y))

        //F = G * ((m1*m2)/r^2)
        double unScaledforceMagnitude = massProduct / Math.Pow(distance, 2);
        double forceMagnitude = G * unScaledforceMagnitude;

        Vector3 forceDirection = difference.normalized;

        Vector3 forceVector = forceDirection * (float)forceMagnitude;

        target.AddForce(forceVector);
    }
}
