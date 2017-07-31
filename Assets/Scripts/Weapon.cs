using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class Weapon : MonoBehaviour {

    [HideInInspector]
    public Player player;
    public string name;
    public Vector2 barrel;
    public float fireRate;
    public int ammo;
    public Sprite icon;

    float lastShot;

    Text ammoText;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        ammoText = GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>();
    }

    void LateUpdate() {
        if (player.inMenu)
            ammo = 0;
        ammoText.text = "" + ammo;
    }

    public virtual void Shoot() {
        lastShot = Time.time;
        ammo--;
        if (ammo <= 0)
            Drop();
    }

    public void Drop() {
        GetComponent<BoxCollider2D>().enabled = true;
        gameObject.AddComponent<Rigidbody2D>();
        transform.SetParent(null);
        Destroy(gameObject, 1.0f);
        GameObject.FindGameObjectWithTag("WeaponIcon").GetComponent<Image>().sprite = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>().emptyIcon;
    }

    public bool CanShoot() {
        if (Time.time - lastShot >= 1.0f / fireRate && ammo > 0) {
            return true;
        }
        return false;
    }

    void OnDrawGizmos() {
        float size = 0.1f;
        Vector2 barrel = this.barrel;
        if (player != null && !player.facingRight)
            barrel.x = -barrel.x;

        barrel = barrel + (Vector2)transform.position;
        Gizmos.DrawLine(new Vector3(barrel.x, barrel.y) + Vector3.left * size, new Vector3(barrel.x, barrel.y) + Vector3.right * size);
        Gizmos.DrawLine(new Vector3(barrel.x, barrel.y) + Vector3.up * size, new Vector3(barrel.x, barrel.y) + Vector3.down * size);
    }

    public void CreateProjectile(Object obj, Vector2 origin, Vector2 direction, float force) {
        GameObject go = Instantiate(obj, origin, Quaternion.Euler(0.0f, 0.0f, Vector2.Angle(Vector2.right, direction))) as GameObject;
        go.transform.position = origin;
        go.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
        go.GetComponent<Projectile>().owner = transform;
    }
}
