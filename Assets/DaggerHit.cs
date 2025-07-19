using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerHit : MonoBehaviour
{ 
    public GameObject impactEffect;
    public float strength = 0.1f;

    // Could be optimized by giving dagger a layer and ignoring collisions with Ground layer
    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();

        if (enemy != null)
        {
            enemy.Hit(strength);
        }
    }
}