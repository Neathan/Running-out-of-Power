using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSizeFixer : MonoBehaviour {

    public float size16_9_res = 5.049777f;
    public float size16_10_res = 5.534977f;

    void Start () {
        float activeRes = size16_9_res;
        if(InRange((float)Screen.width / (float)Screen.height, 16.0f / 10.0f)) {
            activeRes = size16_10_res;
        }
        GetComponent<Camera>().orthographicSize = activeRes;

    }

    bool InRange(float value, float target) {
        return value >= target - 0.01f && value <= target + 0.01f;
    }
}
