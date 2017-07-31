using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleFireWeapon : Weapon {

    public Object weaponShell;
    public float shootForce;

    public override void Shoot() {
        base.Shoot();
        Vector2 direction = Vector2.right;
        Vector2 barrel = this.barrel;
        if (!player.facingRight) {
            direction.x = -direction.x;
            barrel.x = -barrel.x;
        }
        CreateProjectile(weaponShell, barrel + (Vector2)transform.position, direction, shootForce);
    }
}
