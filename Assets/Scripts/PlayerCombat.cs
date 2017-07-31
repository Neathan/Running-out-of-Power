using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerCombat : MonoBehaviour {

    public Transform weaponSlot;
    public float reachDistance;
    public float attractionForce;
    public float collectDistance;
    public float particleRechargeAmount;

    public Transform collectionLocation;
    public Vector3 offset;

    public Slider energyLevel;
    public CanvasGroup hurtImage;

    public float flashTime;
    public float flashAlpha;
    float currentFlashTime;
    bool shouldFlash;

    List<ParticleSystem> systems;
    ParticleSystem.Particle[] particles;
        
    PlayerStats stats;
    Player player;

    public Sprite emptyIcon; // Just beacuse shit won't work.

    public void DropWeapon() {
        if (weaponSlot.childCount > 0) {
            weaponSlot.GetChild(0).GetComponent<Weapon>().Drop();
        }
    }

    public void EquipWeapon(GameObject weapon) {
        DropWeapon();
        GameObject go = Instantiate<GameObject>(weapon, Vector2.zero, Quaternion.identity, weaponSlot);
        go.transform.localPosition = Vector2.zero;
        GameObject.FindGameObjectWithTag("WeaponIcon").GetComponent<Image>().sprite = go.GetComponent<Weapon>().icon;
    }

    public void AddParicleSystemToList(ParticleSystem system) {
        systems.Add(system);
    }

    void Awake() {
        systems = new List<ParticleSystem>();
    }

    void Start () {
        stats = GetComponent<PlayerStats>();
        player = GetComponent<Player>();

        InitUI();
    }

    public void Hurt(float damage) {
        if (!player.dead) {
            shouldFlash = true;
            currentFlashTime = 0.0f;
            stats.AddEnergy(-damage);
        }
    }
	
    public void ShootWeapon() {
        if (weaponSlot.childCount > 0) {
            Weapon weapon = weaponSlot.GetChild(0).GetComponent<Weapon>();
            if(weapon.CanShoot()) {
                weapon.Shoot();
            }
        }
    }

	void Update () {
        if(player.inMenu) {
            GameObject.FindGameObjectWithTag("WeaponIcon").GetComponent<Image>().sprite = emptyIcon;
        }

        if (shouldFlash) {
            currentFlashTime += Time.deltaTime;
            if (currentFlashTime >= flashTime)
                shouldFlash = false;
            hurtImage.alpha = Mathf.Sin(currentFlashTime / flashTime * Mathf.PI) * flashAlpha;
        }


        for (int i = 0; i < systems.Count; i++) {
            if (systems[i] != null) {
                particles = new ParticleSystem.Particle[systems[i].main.maxParticles];
                int numParticleAlive = systems[i].GetParticles(particles);
                for (int j = 0; j < numParticleAlive; j++) {
                    float distance = Vector2.Distance((Vector2)particles[j].position, transform.position + offset);
                    if (distance <= collectDistance) {
                        particles[j].remainingLifetime = 0.0f;
                        stats.AddEnergy(particleRechargeAmount);
                    }
                    else if (distance <= reachDistance) {
                        particles[j].position = Vector3.MoveTowards(particles[j].position, collectionLocation.position + offset, Time.deltaTime * attractionForce * (reachDistance - distance));
                        particles[j].velocity -= new Vector3(particles[j].velocity.x * 0.2f, particles[j].velocity.y * 0.2f);
                    }
                }
                systems[i].SetParticles(particles, numParticleAlive);
            }
            else {
                systems.Remove(systems[i]);
            }
        }
        if (!player.dead && !player.inMenu) {
            stats.energy -= Mathf.Clamp(stats.timer / stats.timeToHard * stats.drainage, 0.0f, stats.drainage) * Time.deltaTime;
        }
    }

    void LateUpdate() {
        UpdateUI();
    }

    void InitUI() {
        energyLevel.maxValue = stats.maxEnergy;
    }

    void UpdateUI() {
        stats.AddEnergy(0.0f);
        energyLevel.value = stats.energy;
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(1, 0.5f, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, reachDistance);
    }
}
