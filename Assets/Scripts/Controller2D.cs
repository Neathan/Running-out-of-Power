using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : RaycastController {

    public float maxSlopeAngle = 80.0f;

    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 playerInput;

    public override void Start() {
        base.Start();
        collisions.faceDirection = 1;
    }

    public void Move(Vector2 moveDelta, bool standingOnPlatform) {
        Move(moveDelta, Vector2.zero, standingOnPlatform);
    }
    public void Move(Vector2 moveDelta, Vector2 input, bool standingOnPlatform = false) {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.moveDeltaOld = moveDelta;
        playerInput = input;

        if(moveDelta.y < 0.0f) {
            DecendSlope(ref moveDelta);
        }

        if (moveDelta.x != 0) {
            collisions.faceDirection = (int)Mathf.Sign(moveDelta.x);
        }

        HorizontalCollisions(ref moveDelta);

        if (moveDelta.y != 0.0f) {
            VerticalCollisions(ref moveDelta);
        }


        transform.Translate(moveDelta);

        if(standingOnPlatform) {
            collisions.below = true;
        }
    }

    void HorizontalCollisions(ref Vector2 moveDelta) {
        float directionX = collisions.faceDirection;
        float rayLength = Mathf.Abs(moveDelta.x) + skinWidth;

        if(Mathf.Abs(moveDelta.x) < skinWidth) {
            rayLength = 2.0f * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++) {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit) {

                if (hit.distance == 0.0f)
                    continue;

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(i == 0 && slopeAngle <= maxSlopeAngle) {
                    if(collisions.descendingSlope) {
                        collisions.descendingSlope = false;
                        moveDelta = collisions.moveDeltaOld;
                    }
                    float distanceToSlopeStart = 0.0f;
                    if (slopeAngle != collisions.slopeAngleOld) {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveDelta.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveDelta, slopeAngle, hit.normal);
                    moveDelta.x += distanceToSlopeStart * directionX;
                }


                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle) {
                    moveDelta.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if(collisions.climbingSlope) {
                        moveDelta.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveDelta.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector2 moveDelta) {
        float directionY = Mathf.Sign(moveDelta.y);
        float rayLength = Mathf.Abs(moveDelta.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveDelta.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit) {
                if(hit.collider.CompareTag("Hollow")) {
                    if(directionY == 1 || hit.distance == 0) {
                        continue;
                    }
                    if(collisions.fallingThroughHollow) {
                        continue;
                    }
                    if(playerInput.y == -1.0f) {
                        collisions.fallingThroughHollow = true;
                        Invoke("ResetFallingThorughHollow", 0.1f);
                        continue;
                    }
                }

                moveDelta.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope) {
                    moveDelta.x = moveDelta.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveDelta.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        if(collisions.climbingSlope) {
            float directionX = Mathf.Sign(moveDelta.x);
            rayLength = Mathf.Abs(moveDelta.x) + skinWidth;
            Vector2 rayOrigin = (directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveDelta.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            if(hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != collisions.slopeAngle) {
                    moveDelta.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }

        }
    }

    void ClimbSlope(ref Vector2 moveDelta, float slopeAngle, Vector2 slopeNormal) {
        float moveDistance = Mathf.Abs(moveDelta.x);
        float climbmoveDeltaY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (moveDelta.y <= climbmoveDeltaY) {
            moveDelta.y = climbmoveDeltaY;
            moveDelta.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveDelta.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
    }

    void DecendSlope(ref Vector2 moveDelta) {

        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveDelta.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveDelta.y) + skinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight) {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveDelta);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveDelta);
        }

        if (!collisions.slidingDownMaxSlope) {
            float directionX = Mathf.Sign(moveDelta.x);
            Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0.0f && slopeAngle <= maxSlopeAngle) {
                    if (Mathf.Sign(hit.normal.x) == directionX) {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveDelta.x)) {
                            float moveDistance = Mathf.Abs(moveDelta.x);
                            float descendmoveDeltaY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveDelta.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveDelta.x);
                            moveDelta.y -= descendmoveDeltaY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveDelta) {
        if(hit) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle > maxSlopeAngle) {
                moveDelta.x = Mathf.Sign(hit.normal.x) * ((Mathf.Abs(moveDelta.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad));

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    void ResetFallingThorughHollow() {
        collisions.fallingThroughHollow = false;
    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope, descendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;

        public Vector2 moveDeltaOld;
        public int faceDirection;
        public bool fallingThroughHollow;

        public void Reset() {
            above = below = left = right = climbingSlope = descendingSlope = slidingDownMaxSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0.0f;
            slopeNormal = Vector2.zero;
        }
    }

}
