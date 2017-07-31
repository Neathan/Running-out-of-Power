using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour {

    public GameObject[] weaponPickups;
    public float maxSpawnTime;
    public float minSpawnTime;
    public bool spawnInStart;

    float currentSpawnTime;
    float spawnTime;

    bool hasWeapon;
    GameObject lastSpawnedPickup;

    Player player;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
	
    public void GameStarts() {
        SetNewTime();
        if (spawnInStart)
            currentSpawnTime = spawnTime;
    }

    public void Restart() {
        currentSpawnTime = 0.0f;
        hasWeapon = false;
        if (lastSpawnedPickup != null) {
            Destroy(lastSpawnedPickup);
        }
    }

    void SetNewTime() {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        currentSpawnTime = 0.0f;
    }

    void SpawnWeaponPickup() {
        hasWeapon = true;
        GameObject go = Instantiate<GameObject>(weaponPickups[Random.Range(0, weaponPickups.Length)]);
        go.transform.position = transform.position;
        go.GetComponent<Pickup>().origin = this;
        lastSpawnedPickup = go;
    }

    public void ResetSpawner() {
        hasWeapon = false;
        SetNewTime();
    }

	void Update () {
		if(!player.inMenu && !hasWeapon) {
            currentSpawnTime += Time.deltaTime;
            if(currentSpawnTime >= spawnTime) {
                SpawnWeaponPickup();
            }
        }
	}
}
