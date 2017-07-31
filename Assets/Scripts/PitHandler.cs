using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PitHandler : MonoBehaviour {

    public Spawner[] spawners;
    BoxCollider2D collider;

	void Start () {
        collider = GetComponent<BoxCollider2D>();

    }
	
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.CompareTag("Monster")) {
            Monster monster = collider.GetComponent<Monster>();
            monster.Rage();
            Spawner spawner = spawners[Random.Range(0, spawners.Length)];
            monster.moveRight = spawner.moveRight;
            collider.transform.position = spawner.transform.position;
        }
        else if(collider.CompareTag("Player")) {
            collider.GetComponent<Player>().Die();
        }
    }

    void OnDrawGizmos() {
        if(collider == null)
            collider = GetComponent<BoxCollider2D>();

        Gizmos.color = new Color(1,0,0,0.2f);
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }
}
