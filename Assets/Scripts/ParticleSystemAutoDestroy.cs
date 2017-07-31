using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemAutoDestroy : MonoBehaviour {

	void Start () {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.duration + 0.1f); // + 0.1f seconds delay
	}
}
