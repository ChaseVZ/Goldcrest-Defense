using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Heal : MonoBehaviour
{
    public float range = 40f;
    public string enemyTag = "Enemy";
    public float healing = 1f;
    public GameObject healEffect;

    public AudioClip healAudio;
    AudioSource heal;

    void Start()
    {
        heal = GetComponent<AudioSource>();
        InvokeRepeating("StartHeal", 4.0f, 10f);
    }

    void StartHeal()
    {
        heal.PlayOneShot(healAudio, 1f);
        Random rnd = new Random();
        int prob = rnd.Next(1, 4);
        if (prob > 0)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            List<GameObject> enemiesInRange = new List<GameObject>();
            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= range)
                {
                    enemiesInRange.Add(enemy);
                }
            }
            if(enemiesInRange.Count > 0)
            {
                GameObject effectIns = (GameObject)Instantiate(healEffect, this.transform);
                Destroy(effectIns, 2.5f);
                foreach (GameObject healed in enemiesInRange)
                {
                    if (healed != gameObject)
                    {
                        EnemyController mage = healed.GetComponent<EnemyController>();
                        mage.Heal(healing);
                    }
                } 
            }
        }
    }
}
