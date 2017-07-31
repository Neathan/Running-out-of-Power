using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToParticleList : MonoBehaviour {

    public ParticleSystem system;

    void Start() {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>().AddParicleSystemToList(system);
    }
}
