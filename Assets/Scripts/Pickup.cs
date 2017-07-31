using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    public GameObject weapon;
    [HideInInspector]
    public PickupSpawner origin;

    PlayerCombat combat;

	void Start () {
        combat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
	}

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.CompareTag("Player")) {
            combat.EquipWeapon(weapon);
            origin.ResetSpawner();
            Destroy(gameObject);
        }
    }
}
