using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [HideInInspector]
    public Transform owner;
    public float damage;
    public float aliveTime;


    void Start() {
        if(aliveTime > 0.0f)
            Invoke("OnDestroy", aliveTime);   
    }

    public virtual void OnDestroy() {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Monster")) {
            collision.gameObject.GetComponent<GroundMonster>().Hurt(damage);
        }
        if (!collision.collider.CompareTag("Shell") && !collision.collider.CompareTag("Weapon")) {
            if (aliveTime > 0.0f)
                CancelInvoke("OnDestroy");
            OnDestroy();
        }
    }
}
