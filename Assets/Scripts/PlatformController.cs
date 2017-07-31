using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController {

    public LayerMask passengerMask;

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    public float speed;
    public bool cyclic;
    public float waitTime;
    [Range(0.0f,2.0f)]
    public float easeAmount;

    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

    struct PassengerMovement {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform) {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }

	public override void Start () {
        base.Start();
        globalWaypoints = new Vector3[localWaypoints.Length];
        for(int i = 0; i < localWaypoints.Length; i++) {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }

    }

	void Update () {
        UpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();

        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    float Ease(float x) {
        float a = easeAmount + 1.0f;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1.0f - x, a));
    }

    Vector3 CalculatePlatformMovement() {
        if(Time.time < nextMoveTime) {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPosition = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if(percentBetweenWaypoints >= 1.0f) {
            percentBetweenWaypoints = 0.0f;
            fromWaypointIndex++;
            if (cyclic) {
                if (fromWaypointIndex >= globalWaypoints.Length - 1) {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return newPosition - transform.position;
    }

    void MovePassengers(bool beforeMovePlatform) {
        foreach(PassengerMovement passenger in passengerMovement) {
            if(!passengerDictionary.ContainsKey(passenger.transform)) {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }

            if(passenger.moveBeforePlatform == beforeMovePlatform) {
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    void CalculatePassengerMovement(Vector3 velocity) {
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        HashSet<Transform> movedPassagers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        // Vertical movement
        if (velocity.y != 0) {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++) {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                if (hit && hit.distance != 0.0f) {
                    if (!movedPassagers.Contains(hit.transform)) {
                        movedPassagers.Add(hit.transform);

                        float pushX = directionY == 1 ? velocity.x : 0.0f;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }

        // Horizontal movement
        if (velocity.x != 0) {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++) {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                if (hit && hit.distance != 0.0f) {
                    if (!movedPassagers.Contains(hit.transform)) {
                        movedPassagers.Add(hit.transform);

                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }

        // horizontal and going down
        if(directionY == -1 || velocity.y == 0.0f && velocity.x != 0.0f) {
            float rayLength = skinWidth * 2.0f;

            for (int i = 0; i < verticalRayCount; i++) {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                if (hit && hit.distance != 0.0f) {
                    if (!movedPassagers.Contains(hit.transform)) {
                        movedPassagers.Add(hit.transform);

                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }

    void OnDrawGizmos() {
        if(localWaypoints != null) {
            Gizmos.color = Color.red;
            float size = 0.3f;
            for(int i = 0; i < localWaypoints.Length; i++) {
                Vector3 globalWaypointPosition = Application.isPlaying ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPosition - Vector3.up * size, globalWaypointPosition + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPosition - Vector3.left * size, globalWaypointPosition + Vector3.left * size);
            }
        }
    }
}
