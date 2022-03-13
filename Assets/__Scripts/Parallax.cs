using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Set in inspector")]
    public GameObject poi; // point of interest: player ship
    public GameObject[] panels;
    public float scrollSpeed = -30f;
    public float motionMult = 0.25f; // how much the panels react to player movement

    private float panelH; //height of each panel
    private float depth; //pos.z

    void Start()
    {
        panelH = panels[0].transform.localScale.y;
        depth = panels[0].transform.position.z;

        panels[0].transform.position = new Vector3(0, 0, depth);
        panels[1].transform.position = new Vector3(0, panelH, depth);    
    }

    void Update()
    {
        float tY, tX = 0;
        tY = Time.time * scrollSpeed % panelH + (panelH * 0.5f);

        if (poi != null) {
            tX = -poi.transform.position.x * motionMult;
        }

        panels[0].transform.position = new Vector3(tX, tY, depth);
        if (tY >= 0) {
            panels[1].transform.position = new Vector3(tX, tY - panelH, depth);
        } else {
            panels[1].transform.position = new Vector3(tX, tY + panelH, depth);
        }
    }
}
