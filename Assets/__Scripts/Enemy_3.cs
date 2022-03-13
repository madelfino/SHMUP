using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy
{
    // Enemy_3 will move following a bezier curve, which is a linear
    // interpolation between more than 2 points
    [Header("Set in inspector: Enemy_3")]
    public float lifetime = 5;

    [Header("Set dynamically: Enemy_3")]
    public Vector3[] points;
    public float birthTime;

    //Start works well because it's not used by the superclass
    void Start() {
        points = new Vector3[3];

        //the first point is set in Main.SpawnEnemy()
        points[0] = pos;

        float xMin = -bndCheck.camWidth + bndCheck.radius;
        float xMax = bndCheck.camWidth - bndCheck.radius;

        Vector3 v = Vector3.zero; // pick a random middle position in the bottim half of the screen
        v.x = Random.Range(xMin, xMax);
        v.y = -bndCheck.camHeight * Random.Range(2.75f, 2);
        points[1] = v;

        v = Vector3.zero; // pick a random final position above the top of the screen
        v.x = Random.Range(xMin, xMax);
        v.y = pos.y;
        points[2] = v;

        birthTime = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - birthTime) / lifetime;

        if (u > 1) { // this enemy_3 has finished its life
            Destroy(this.gameObject);
            return;
        }

        // interpolate the 3 bezier curve points
        Vector3 p01, p12;
        u = u - 0.2f * Mathf.Sin(u * Mathf.PI * 2); // add easing to speed up the middle of the enemy's path
        p01 = (1 - u) * points[0] + u * points[1];
        p12 = (1 - u) * points[1] + u * points[2];
        pos = (1 - u) * p01 + u * p12;
    }
}
