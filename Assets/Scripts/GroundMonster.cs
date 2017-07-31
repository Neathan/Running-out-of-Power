using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Monster))]
[RequireComponent(typeof(Controller2D))]
public class GroundMonster : Monster {

    public float moveSpeed = 4.0f;
    public float accelerationTime = 0.1f;
    public float gravity = -20.0f;

    Controller2D controller;

    Vector2 directionalInput;
    int wallDirX;
    Vector2 velocity;
    float velocityXSmoothing;

    [HideInInspector]
    public bool facingRight;

    void Start() {
        controller = GetComponent<Controller2D>();
    }

    void Update() {
        CalculateVelocity();

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

        facingRight = controller.collisions.faceDirection == 1;
    }

    public void AddVelocity(Vector2 force) {
        velocity += force;
    }

    public void SetDirectionalInput(Vector2 input) {
        if(!dead)
            directionalInput = input;
    }

    void CalculateVelocity() {
        float rage = 1.0f;
        if (raging)
            rage = rageSpeedMultiplier;

        float targetVelocityX = directionalInput.x * moveSpeed * rage;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);
        velocity.y += gravity * Time.deltaTime;
    }
}
