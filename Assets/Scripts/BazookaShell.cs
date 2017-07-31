using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaShell : Projectile {

    public GameObject explosion;
    public ParticleSystem trail;
    public float knockbackForce;
    public float extraVerticalKnockbackForce;
    public float explosionRange;

    public override void OnDestroy() {
        GameObject go = Instantiate<GameObject>(explosion);
        go.transform.position = transform.position;
        Explode();
        Vector3 scale = trail.transform.localScale;
        trail.transform.SetParent(null, true);
        trail.transform.localScale = scale;
        trail.Stop();
        trail.gameObject.AddComponent<ParticleSystemAutoDestroy>();
        base.OnDestroy();
    }

    void Explode() {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        for(int i = 0; i < monsters.Length; i++) {
            ApplyKnockback(monsters[i]);
        }
    }

    void ApplyKnockback(GameObject monster) {
        float distance = Vector2.Distance(monster.transform.position, transform.position);
        if (distance <= explosionRange) {
            /*
            Vector2 toMonster = (transform.position - monster.transform.position).normalized;
            toMonster.y *= extraVerticalKnockbackForce;
            monster.GetComponent<GroundMonster>().AddVelocity(toMonster * knockbackForce * distance / explosionRange);
            */
            monster.GetComponent<GroundMonster>().Die();
        }
    }


    void OnDrawGizmos() {
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, explosionRange);
    }
}
