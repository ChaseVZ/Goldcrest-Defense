using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public float health = 1f;

    private float maxhealth;

    public Image healthBar;

    public GameObject healEffect;

    public GameObject deathEffect;

    //How much resource a enemy will drop on death.
    public int resource;

    private ResourceManager resourceManager;

    private WaveSpawner waveSpawner;

    private Transform pos;
    
    private Vector3 deathEffectOffset = new Vector3(0, 1.5f, 0);
    
    // called when this enemy is hit with projectile
    public void Hit(float strength)
    {
        health -= strength;
        healthBar.fillAmount = health / maxhealth;


    }

    public void Heal(float heal)
    {
        GameObject effectIns = (GameObject)Instantiate(healEffect, this.transform);
        health += heal;
        if(health > maxhealth)
        {
            health = maxhealth;
        }
        healthBar.fillAmount = health / maxhealth;
        Destroy(effectIns, 2.5f);
    }

    private void Start()
    {
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        waveSpawner = GameObject.Find("GameManager").GetComponent<WaveSpawner>();

        maxhealth = health;
    }

    private void Update()
    {
        if (health <= 0)
        {
            //TODO THIS DOESNT WORK
            Quaternion qt = transform.rotation;
            qt.Set(0, -90, 0, 0);

            GameObject deathEffectIns = (GameObject)Instantiate(deathEffect, transform.position + deathEffectOffset, qt);
            deathEffectIns.GetComponent<ParticleSystem>().Play();
            Destroy(deathEffectIns, 2f);

            resourceManager.addResource(resource);
            waveSpawner.enemyDestroy();
            Destroy(gameObject);
            
        }
    }
}
