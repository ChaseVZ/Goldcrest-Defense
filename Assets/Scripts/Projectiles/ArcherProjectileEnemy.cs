using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherProjectileEnemy : MonoBehaviour
{
    private Transform target;

    //Speed of projectile
    public float speed = 50f;

    //The effect when a projectile hits object
    public GameObject impactEffect;

    private float strength;
    private bool doOnce = true;

    private GameObject gameManager;
    private TownHealth townHealth;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager");
        townHealth = gameManager.GetComponent<TownHealth>();
    }
    public void Seek(Transform _target, float _strength)
    {
        target = _target;
        strength = _strength;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
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
    }

    void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);

        //Deal Damage if arrow hits
        townHealth.TakeDamage((int) strength);

        Destroy(effectIns, 2f); //Destroys particle system after 2 sec
        Destroy(this.gameObject);
    }
}
