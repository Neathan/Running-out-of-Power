using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Object[] monsters;
    public bool moveRight;

    public float hardSpawnRate;
    public float timeToHard;

    [HideInInspector]
    public bool active;

    public float spawnRate;
    public float lastSpawn;
    public float timer;
    
    public void Restart() {
        active = false;
        lastSpawn = 0.0f;
        timer = 30.0f;
        SetNewSpawnRate();
    }

    void SetNewSpawnRate() {
        spawnRate = Mathf.Clamp(timer / timeToHard * hardSpawnRate, 0, hardSpawnRate);
        if (spawnRate > 5.0f)
            spawnRate = hardSpawnRate;
        if (spawnRate < 0.0f) // Will never happend, no one can do it that far
            spawnRate = 0.0f;
    }

	void Start () {
        active = false;
        Restart();
    }
	void Update () {
        SetNewSpawnRate();
        if (active) {
            timer += Time.deltaTime;

            if ((timer - lastSpawn) >= 1.0f / spawnRate) {
                lastSpawn = timer;
                Spawn();
            }
        }
	}

    void Spawn() {
        Object monsterObject = monsters[Random.Range(0, monsters.Length)];
        GameObject go = Instantiate(monsterObject, transform.position, Quaternion.identity) as GameObject;
        go.GetComponent<Monster>().moveRight = moveRight;

        SetNewSpawnRate();
    }
}
