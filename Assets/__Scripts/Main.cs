using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public GameObject prefabDroneAlly;
    public float enemySpawnPerSecond = 2; // #Enemies / sec
    public float enemyDefaultPadding = 1.5f; // Padding for position
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] {
        WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield, WeaponType.drone
    };

    private BoundsCheck bndCheck;

    public void ShipDestroyed(Enemy e) {
        if (Random.value <= e.powerUpDropChance) {
            int index = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[index];
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            pu.SetType(puType);
            pu.transform.position = e.transform.position;
        }
    }

    void Awake() {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond );

        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach( WeaponDefinition def in weaponDefinitions) {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy() {
        int i = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[i]);

        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null) {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void SpawnDroneAlly() {
      Instantiate<GameObject>(prefabDroneAlly);
    }

    public void DelayedRestart( float delay ) {
        Invoke("Restart", delay);
    }

    public void Restart() {
        SceneManager.LoadScene("_Scene_0");
    }

    ///<summary>
    ///  Static function that gets a WeaponDefinition from the WEAP_DICT
    ///  static protected field of the Main class.
    ///</summary>
    ///<returns> The WeaponDefinition or, if there is no WeaponDefinition with
    ///  the WeaponType passed in, returns a new WeaponDefinition with a
    ///  WeaponType of none.</returns>
    ///<param name="wt">The WeaponType of the desired WeaponDefinition</param>
    static public WeaponDefinition GetWeaponDefinition( WeaponType wt ) {
        if (WEAP_DICT.ContainsKey(wt)) {
            return WEAP_DICT[wt];
        }
        return new WeaponDefinition();
    }
}
