using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon {

    public float damage = 20.0f;
    public GameObject rifleEffect;

    void HideEffect() {
        rifleEffect.SetActive(false);
    }

	public override void Shoot () {
        base.Shoot();

        Vector2 direction = Vector2.right;
        Vector2 barrel = this.barrel;
        if (!player.facingRight) {
            direction.x = -direction.x;
            barrel.x = -barrel.x;
        }
        RaycastHit2D[] hits = Physics2D.RaycastAll(barrel + (Vector2)transform.position, direction);
        Vector2 wallHit = Vector2.zero;
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].collider.CompareTag("Monster")) {
                hits[i].collider.GetComponent<Monster>().Hurt(damage);
                print("hurt: " + hits[i].collider.name);
            }
            else if (hits[i].collider.CompareTag("Tile")) {
                wallHit = hits[i].point;
                break;
            }
        }
        rifleEffect.transform.localScale = new Vector2(Vector2.Distance(wallHit, barrel + (Vector2)transform.position) / 3.0f, rifleEffect.transform.localScale.y);
        rifleEffect.transform.localPosition = new Vector2(Vector2.Distance(wallHit, barrel + (Vector2)transform.position) / 2.0f / 3.0f, rifleEffect.transform.localPosition.y);
        rifleEffect.SetActive(true);
        Invoke("HideEffect", 0.1f);
    }
}
