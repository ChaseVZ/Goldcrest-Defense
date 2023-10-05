using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherShoot : MonoBehaviour
{
    // Start is called before the first frame update
    public float range = 10f;
    public float fireRate = 4f;
    public float projDamage = 1f;

    public string enemyTag = "Finish";

    public AudioClip bowShot;
    AudioSource shoot;

    public Transform target;

    public GameObject projectilePrefab;
    public Transform firepoint;

    private float fireCountdown = 0f;
    void Start()
    {
        shoot = GetComponent<AudioSource>();
        InvokeRepeating("UpdateTarget", 0f, 0.5f); // calls update target 2 times a sec probably needs to be changed later
    }

    // Update is called once per frame
    void Update()
    {

        if(target == null)
        {
            return;
        }
        if(fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = fireRate;
        }

        fireCountdown -= Time.deltaTime;

    }

    void Shoot()
    {
        shoot.PlayOneShot(bowShot, 1f);
        GameObject projectile = (GameObject)Instantiate(projectilePrefab, firepoint.position, firepoint.rotation);
        ArcherProjectileEnemy archProjectile = projectile.GetComponent<ArcherProjectileEnemy>();

        if(archProjectile != null)
        {
            archProjectile.Seek(target, projDamage);
        }
    }

    //allows you to see range when selected in editor can use this to show in game
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= range)
            {
                nearestEnemy = enemy;
            }
        }

        if(nearestEnemy != null)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }
}
