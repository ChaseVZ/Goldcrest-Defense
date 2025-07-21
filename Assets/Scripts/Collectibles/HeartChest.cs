using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeartChest : MonoBehaviour
{
    private TownHealth healthManager;
    public int healAmount = 10;
    AudioSource healthSound;
    public GameObject collectEffect;
    private Vector3 collectEffectOffset = new Vector3(2f, 1f, 0);
    public GameObject healthText;


    public void Start()
    {
        healthSound = GameObject.Find("HealthChestAudio").GetComponent<AudioSource>();
        healthManager = GameObject.Find("GameManager").GetComponent<TownHealth>();
        healthText.GetComponent<TextMeshPro>().text = healAmount.ToString();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            GameObject collectEffectIns = (GameObject)Instantiate(collectEffect, transform.position + collectEffectOffset, Quaternion.Euler(0, 180, 0));
            collectEffectIns.GetComponent<ParticleSystem>().Play();
            Destroy(collectEffectIns, 5f);

            healthSound.Play();
            healthManager.AddHealth(healAmount);

            Destroy(gameObject);
        }
    }
}