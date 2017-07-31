using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerAnimator : MonoBehaviour {

    Player player;

    void Start () {
        player = GetComponent<Player>();
	}

	void Update () {
        if(!player.facingRight) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (player.facingRight) {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }


}
