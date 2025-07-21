using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceChest : MonoBehaviour
{
    private ResourceManager resourceManager;
    public int resourceAmount = 10;
    AudioSource resourceSound;
    public GameObject collectEffect;
    private Vector3 collectEffectOffset = new Vector3(2f, 1f, 0);
    public GameObject resourceText;

    public void Start()
    {
        resourceSound = GameObject.Find("ResourceChestAudio").GetComponent<AudioSource>();
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        resourceText.GetComponent<TextMeshPro>().text = resourceAmount.ToString();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            GameObject collectEffectIns = (GameObject)Instantiate(collectEffect, transform.position + collectEffectOffset, Quaternion.Euler(0, 180, 0));
            collectEffectIns.GetComponent<ParticleSystem>().Play();
            Destroy(collectEffectIns, 5f);

            resourceSound.Play();
            resourceManager.addResource(resourceAmount);

            Destroy(gameObject);
        }
    }
}
