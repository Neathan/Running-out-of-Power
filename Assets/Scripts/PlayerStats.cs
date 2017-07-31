using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerStats : MonoBehaviour {

    public float maxEnergy;
    public float energy;
    public float drainage;
    public float timeToHard;

    [HideInInspector]
    public float timer;

    Player player;

    void Start() {
        player = GetComponent<Player>();
    }

    public void Restart() {
        timer = 30.0f;
        energy = maxEnergy;
    }

    void Update() {
        if(!player.inMenu && !player.dead)
            timer += Time.deltaTime;
    }

    public void AddEnergy(float amount) {
        energy += amount;
        if(energy > maxEnergy) {
            energy = maxEnergy;
        }
        else if(energy <= 0.0f) {
            player.Die();
            energy = 0.0f;
        }
    }

}
