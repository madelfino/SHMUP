using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Set in Inspector: Enemy_2")]
    public float sinEccentricity = 0.6f; // Determines how much the sin wave will affect movement
    public float lifeTime = 10;

    [Header("Set dynamically: Enemy_2")]
    // Enemy_2 uses a sin wave to modify a 2-point linear interpolation
    public Vector2 p0;
    public Vector2 p1;
    public float birthTime;

    void Start() {
        // pick any point on the left side of the screen
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range( -bndCheck.camHeight, bndCheck.camHeight );

        // pick any point on the right side of the screen
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range( -bndCheck.camHeight, bndCheck.camHeight );

        // possibly switch sides
        if (Random.value > 0.5f) {
            // multuplying the x value by x will move it to the other side of the screen
            p0.x *= -1;
            p1.x *= -1;
        }

        birthTime = Time.time;
    }

    public override void Move()
    {
        // bezier curves work based on a u value between 0 and 1
        float u = (Time.time - birthTime) / lifeTime;
        if (u > 1) { // age > lifetime
            Destroy( this.gameObject );
            return;
        }

        // adjust u by adding a U curve based on a sine wave
        u = u + sinEccentricity * Mathf.Sin(u * Mathf.PI * 2);

        // interpolate the two linear interpolation points
        pos = (1 - u) * p0 + u * p1;
    }
}
