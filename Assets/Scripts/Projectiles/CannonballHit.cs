using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballHit : MonoBehaviour
{
    public GameObject impactEffect;
    public GameObject firingEffect;
    private bool hit = false;
    public float radius = 4f;
    private float strength;

    public void Seek(float _strength)
    {
        strength = _strength;
    }

    void Start()
    {
        GameObject firing = (GameObject)Instantiate(firingEffect, transform.position, transform.rotation);
        Destroy(firing, 2f);
    }

    void Update()
    {
        if (transform.position.y <= .5 && !hit) 
        {
            HitGround();
            hit = true;
            return;
        }
    }

    void HitGround()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider c in colliders)
        {
            EnemyController cannonProjectile = c.GetComponent<EnemyController>();
            
            if (c.gameObject.tag == "Miniboss") { GameManager.instance.GetComponent<Miniboss>().Hit(strength); }
            else if (cannonProjectile != null) { cannonProjectile.Hit(strength); }
        }
        GameObject effectExplode = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectExplode, 2f);
        Destroy(gameObject, 2.1f);
    }
}