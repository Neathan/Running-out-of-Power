using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerCombat))]
public class PlayerInput : MonoBehaviour {

    Player player;
    PlayerCombat combat;
    Vector2 directionalInput;

    void Start () {
        player = GetComponent<Player>();
        combat = GetComponent<PlayerCombat>();
    }

    void Update() {
        if (!player.dead && !player.inMenu) {
            directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);
            
            if (Input.GetButtonDown("Jump")) {
                player.OnJumpInputDown();
            }
            if (Input.GetButtonUp("Jump")) {
                player.OnJumpInputUp();
            }
            if (Input.GetButtonDown("Shoot")) {
                combat.ShootWeapon();
            }
        }
        else {
            player.SetDirectionalInput(Vector2.zero);
            if (Input.GetButtonDown("Start")) {
                if (player.inMenu) {
                    player.GameStarted();
                    GameObject.FindGameObjectWithTag("GameStatsManager").GetComponent<GameStatsManager>().GameStarted();
                }
                else {
                    player.Restart();
                    GameObject.FindGameObjectWithTag("GameStatsManager").GetComponent<GameStatsManager>().Reset();
                }
            }
            else if(!Input.GetButtonDown("Exit") && Input.anyKeyDown) {
                GameObject.FindGameObjectWithTag("GameStatsManager").GetComponent<GameStatsManager>().ExitCancle();
            }
            else if(Input.GetButtonDown("Exit")) {
                GameObject.FindGameObjectWithTag("GameStatsManager").GetComponent<GameStatsManager>().Exit();
            }
        }
	}
}
