using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon {

    public Object shotgunShell;
    public float shootForce;
    public int numOfBulletsInSpray;
    public float sprayAngle;

    public override void Shoot() {
        base.Shoot();

        for(int i = 0; i < numOfBulletsInSpray; i++) {
            float angle = sprayAngle / numOfBulletsInSpray * i - sprayAngle / 2.0f;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector2 barrel = this.barrel;
            if(!player.facingRight) {
                direction.x = -direction.x;
                barrel.x = -barrel.x;
            }

            CreateProjectile(shotgunShell, barrel + (Vector2)transform.position + new Vector2(0.0f, direction.y) * 1f, direction, shootForce);
        }
    }

}
