using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

    public float health;
    public float damage;
    public Object deathParticleSystem;
    [HideInInspector]
    public bool dead = false;
    public bool moveRight;
    public Color rageTint;
    public float rageSpeedMultiplier;
    [HideInInspector]
    public bool raging;
    public ParticleSystem rageEffect;

    public void Hurt(float damage) {
        health -= damage;
        if (health <= 0.0f)
            Die();
    }

    public virtual void Die() {
        if (!dead) {
            dead = true;
            GameObject.FindGameObjectWithTag("GameStatsManager").GetComponent<GameStatsManager>().AddScore();
            GameObject go = Instantiate(deathParticleSystem, transform.position, Quaternion.identity) as GameObject;
            Destroy(go, go.GetComponent<ParticleSystem>().main.duration + go.GetComponent<ParticleSystem>().main.startLifetimeMultiplier);
            Destroy(gameObject);
        }
    }

    public virtual void Rage() {
        rageEffect.gameObject.SetActive(true);
        raging = true;
        GetComponent<Renderer>().material.color = rageTint;
    }

}
