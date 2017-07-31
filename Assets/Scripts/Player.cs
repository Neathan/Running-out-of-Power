using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerStats))]
public class Player : MonoBehaviour {

    public float minJumpHeight = 1.0f;
    public float maxJumpHeight = 4.0f;
    public float timeToJumpApex = 0.4f;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax = 3.0f;
    public float wallStickTime = 0.25f;
    float timetoWallUnstick;

    public float accelerationTimeAirborne = 0.2f;
    public float accelerationTimeGrounded = 0.1f;
    public float moveSpeed = 6.0f;

    public float monsterDamage = 20.0f;
    public float monsterKnockbackX = 5.0f;
    public float monsterKnockbackY = 3.0f;

    public Animator playerAnimator;
    public Animator playerHandAnimator;

    public Light light;
    public float maxLightIntensity, minLightIntensity, lightAfterDeathIntensity;
    public float lightDeathDecendingTime;

    float lightDeathStartTime;

    [HideInInspector]
    public bool facingRight = true;

    float gravity;
    float minJumpVelocity;
    float maxJumpVelocity;
    Vector2 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    PlayerCombat combat;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    [HideInInspector]
    public bool dead, inMenu;

    PlayerStats stats;

    public void UpdateLight() {
        if(!dead)
            light.intensity = stats.energy / stats.maxEnergy * (maxLightIntensity - minLightIntensity) + minLightIntensity;
        else {
            light.intensity = (1.0f - Mathf.Clamp01((Time.time - lightDeathStartTime) / lightDeathDecendingTime)) * (minLightIntensity - lightAfterDeathIntensity) + lightAfterDeathIntensity;
        }
    }

    public void GameStarted() {
        inMenu = false;
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        for(int i = 0; i < spawners.Length; i++) {
            spawners[i].GetComponent<Spawner>().active = true;
        }
        GameObject[] PickupSpawners = GameObject.FindGameObjectsWithTag("PickupSpawner");
        for (int i = 0; i < PickupSpawners.Length; i++) {
            PickupSpawners[i].GetComponent<PickupSpawner>().GameStarts();
        }
    }

    public void Die() {
        if (!dead && !inMenu) {
            GameObject.FindGameObjectWithTag("GameStatsManager").GetComponent<GameStatsManager>().Save();

            playerAnimator.Play("Dying");
            playerHandAnimator.Play("HandDying");

            GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
            for (int i = 0; i < spawners.Length; i++) {
                spawners[i].GetComponent<Spawner>().active = false;
            }

            lightDeathStartTime = Time.time;

            dead = true;
        }
    }

    public void Restart() {
        combat.DropWeapon();
        inMenu = true;
        dead = false;

        stats.Restart();
        playerAnimator.Play("PlayerAnimation");
        playerHandAnimator.Play("HandAnimation");
        transform.position = Vector2.zero;
        velocity = Vector2.zero;

        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        for (int i = 0; i < monsters.Length; i++) {
            Destroy(monsters[i]);
        }

        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        for (int i = 0; i < spawners.Length; i++) {
            spawners[i].GetComponent<Spawner>().Restart();
        }

        GameObject[] PickupSpawners = GameObject.FindGameObjectsWithTag("PickupSpawner");
        for (int i = 0; i < PickupSpawners.Length; i++) {
            PickupSpawners[i].GetComponent<PickupSpawner>().Restart();
        }
    }

    void Start() {
        inMenu = true;
        dead = false;
        controller = GetComponent<Controller2D>();
        combat = GetComponent<PlayerCombat>();
        stats = GetComponent<PlayerStats>();

        gravity = -(2.0f * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2.0f);
        maxJumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
        minJumpVelocity = Mathf.Sqrt(2.0f * Mathf.Abs(gravity) * minJumpHeight);
    }

    void Update() {
        UpdateLight();

        CalculateVelocity();
        HandleWallSliding();

        wallDirX = controller.collisions.left ? -1 : 1;
        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below) {
            if (controller.collisions.slidingDownMaxSlope) {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else {
                velocity.y = 0.0f;
            }
        }
        if(directionalInput.x != 0)
            facingRight = Mathf.Sign(directionalInput.x) == 1.0f;
    }

    public void AddVelocity(Vector2 force) {
        if(!dead && !inMenu)
            velocity += force;
    }

    public void SetDirectionalInput(Vector2 input) {
        directionalInput = input;
    }

    public void OnJumpInputDown() {
        if (!dead && !inMenu) {
            if (wallSliding) {
                if (wallDirX == directionalInput.x) {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                else if (directionalInput.x == 0.0f) {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            if (controller.collisions.below) {
                if (controller.collisions.slidingDownMaxSlope) {
                    if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) {
                        velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                        velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                    }
                }
                else {
                    velocity.y = maxJumpVelocity;
                }
            }
        }
    }

    public void OnJumpInputUp() {
        if (velocity.y > minJumpVelocity) {
            velocity.y = minJumpVelocity;
        }
    }

    void CalculateVelocity() {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;

    }

    void HandleWallSliding() {
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0.0f) {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timetoWallUnstick > 0.0f) {
                velocityXSmoothing = 0.0f;
                velocity.x = 0.0f;

                if (directionalInput.x != wallDirX && directionalInput.x != 0.0f) {
                    timetoWallUnstick -= Time.deltaTime;
                }
                else {
                    timetoWallUnstick = wallStickTime;
                }
            }
            else {
                timetoWallUnstick = wallStickTime;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.CompareTag("Monster")) {
            if(transform.position.x < collider.transform.position.x) { // Right of the player
                AddVelocity(new Vector2(-monsterKnockbackX, monsterKnockbackY));
            }
            else {
                AddVelocity(new Vector2(monsterKnockbackX, monsterKnockbackY));
            }

            // Take damage
            combat.Hurt(collider.GetComponent<Monster>().damage);
        }
    }

}
