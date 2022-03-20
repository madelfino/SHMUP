using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part {
    public string name;
    public float health;
    public string[] protectedBy;

    [HideInInspector]
    public GameObject go;
    [HideInInspector]
    public Material mat;
}

///<summary>
///  Enemy_4 will start offscreen and then pick a random point on the screen
///  to move to. Once it has arrive, it will pick another random point and
///  continue until the player has shot it down
///</summary>
public class Enemy_4 : Enemy
{
    [Header("Set in inspector: Enemy_4")]
    public Part[] parts;

    private Vector3 p0, p1; // two points to interpolate
    private float timeStart; // birth time for this Enemy_4
    private float duration = 4; // duration of movement

    void Start() {
        p0 = p1 = pos;

        InitMovement();

        // cache gameobject & material of each Part in parts
        Transform t;
        foreach (Part part in parts) {
            t = transform.Find(part.name);
            if (t != null) {
                part.go = t.gameObject;
                part.mat = part.go.GetComponent<Renderer>().material;
            }
        }
    }

    void InitMovement() {
        p0 = p1; // set p0 to the old p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if (u >= 1) {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2); // apply easing out to u
        pos = (1 - u) * p0 + u * p1; // simple linear interpolation
    }

    Part FindPart(string n) {
        foreach( Part part in parts ) {
            if (part.name == n) return part;
        }
        return  null;
    }

    Part FindPart(GameObject go) {
        foreach( Part part in parts ) {
            if (part.go == go) return part;
        }
        return null;
    }

    bool Destroyed(GameObject go) {
        return Destroyed(FindPart(go));
    }

    bool Destroyed(string n) {
        return Destroyed(FindPart(n));
    }

    bool Destroyed(Part part) {
        if (part == null) return true;
        return (part.health <= 0);
    }

    void ShowLocalizedDamage(Material m) {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    void OnCollisionEnter(Collision coll) {
        GameObject other = coll.gameObject;
        switch (other.tag) {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                if (!bndCheck.isOnScreen) {
                    Destroy(other);
                    break;
                }

                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part partHit = FindPart(goHit);
                if (partHit == null) {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    partHit = FindPart(goHit);
                }
                //Debug.Log(partHit.name + " hit!");
                if (partHit.protectedBy != null) {
                    foreach (string s in partHit.protectedBy) {
                        if (!Destroyed(s)) {
                            Destroy(other);
                            return;
                        }
                    }
                }
                partHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                ShowLocalizedDamage(partHit.mat);
                if (partHit.health <= 0) {
                    partHit.go.SetActive(false);
                }
                bool allDestroyed = true;
                foreach (Part part in parts) {
                    if (!Destroyed(part)) {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed) {
                    Main.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
        }
    }
}
