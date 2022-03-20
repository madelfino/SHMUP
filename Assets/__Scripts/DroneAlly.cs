using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAlly : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float waveFrequency = 2; // # seconds for a full sine wave
    public float followRadius = 7;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon droneWeapon;
    public GameObject leader;

    private GameObject lastTrigger = null;
    private float birthTime;

    void Awake()
    {
        birthTime = Time.time;
    }

    void Start() {
      if (leader == null) {
        leader = Hero.S.gameObject;
      }
    }

    // Update is called once per frame
    void Update()
    {
        if (leader == null) {
            Destroy(this.gameObject);
        } else {
            droneWeapon.Fire();
            Vector3 tempPos = transform.position;
            float age = Time.time - birthTime;
            float theta = Mathf.PI * 2 * age / waveFrequency;
            float sin = Mathf.Sin(theta);
            float cos = Mathf.Cos(theta);
            tempPos.x = leader.transform.position.x + followRadius * cos;
            tempPos.y = leader.transform.position.y + followRadius * sin;
            transform.position = tempPos;
        }
    }

    private void OnTriggerEnter(Collider other) {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go == lastTrigger) {
            return;
        }
        lastTrigger = go;

        if (go.tag == "Enemy") {
            Destroy(go);
            Destroy(this.gameObject);
        }
    }
}
