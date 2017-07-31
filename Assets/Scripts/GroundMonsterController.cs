using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundMonster))]
public class GroundMonsterController : MonoBehaviour {

    GroundMonster monster;
    Controller2D controller;

    float scaleX;

    void Start () {
        monster = GetComponent<GroundMonster>();
        controller = GetComponent<Controller2D>();
        scaleX = transform.localScale.x;
    }

	void Update () {
        if(controller.collisions.right && monster.moveRight) {
            monster.moveRight = false;
        }
        else if(controller.collisions.left && !monster.moveRight) {
            monster.moveRight = true;
        }

        if(monster.moveRight) {
            monster.SetDirectionalInput(new Vector2(1.0f, 0.0f));
            transform.localScale = new Vector3(scaleX, transform.localScale.y, 1.0f);
        }
        else {
            monster.SetDirectionalInput(new Vector2(-1.0f, 0.0f));
            transform.localScale = new Vector3(-scaleX, transform.localScale.y, 1.0f);
        }
	}
}
