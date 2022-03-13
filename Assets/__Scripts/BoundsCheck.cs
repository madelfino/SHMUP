using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps a GameObject on the screen
/// Note this only works for orthographic Main Camera at (0, 0, 0)
/// </summary>
public class BoundsCheck : MonoBehaviour
{
    [Header("Set in inspector")]
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Set dynamically")]
    public float camWidth;
    public float camHeight;
    public bool isOnScreen = true;

    [HideInInspector]
    public bool offRight, offLeft, offUp, offDown;

    void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        isOnScreen = true;
        offDown = offLeft = offRight = offUp = false;

        if (pos.x > camWidth - radius) {
            pos.x = camWidth - radius;
            offRight = true;
        }

        if (pos.x < -camWidth + radius) {
            pos.x = -camWidth + radius;
            offLeft = true;
        }

        if (pos.y > camHeight - radius) {
            pos.y = camHeight - radius;
            offUp = true;
        }

        if (pos.y < -camHeight + radius) {
            pos.y = -camHeight + radius;
            offDown = true;
        }

        isOnScreen = !(offRight || offLeft || offUp || offDown);
        if (keepOnScreen && !isOnScreen) {
            transform.position = pos;
            isOnScreen = true;
            offDown = offLeft = offRight = offUp = false;
        }
    }

    void OnDrawGizmos() {
        if (!Application.isPlaying) return;
        Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}
