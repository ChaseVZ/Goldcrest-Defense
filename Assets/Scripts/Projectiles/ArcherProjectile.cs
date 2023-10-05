using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherProjectile : MonoBehaviour
{
    private Transform target;

    AudioSource enemyHitSound;

    //Speed of projectile
    public float speed = 50f;

    //The effect when a projectile hits object
    public GameObject impactEffect;

    private float strength;
    private bool doOnce = true;

    public void Seek(Transform _target, float _strength)
    {
        target = _target;
        strength = _strength;
    }

    private void Start() 
    {
        enemyHitSound = GameObject.Find("EnemyHit").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (doOnce)
        {
            transform.LookAt(target);
            doOnce = false;
        }
        Vector3 targetMiddle = new Vector3(target.position.x, target.position.y + 1f, target.position.z);

        Vector3 dir = targetMiddle - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        //when projectile hits target
        if(dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        //transform.LookAt(target);

    }

    void HitTarget()
    {
        EnemyController archProjectile = target.GetComponent<EnemyController>();

        if (archProjectile == null) { GameManager.instance.GetComponent<Miniboss>().Hit(strength); }
        else { archProjectile.Hit(strength); }

        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        enemyHitSound.Play();
        Destroy(effectIns, 2f); //Destroys particle system after 2 sec
        Destroy(gameObject);
    }
}
