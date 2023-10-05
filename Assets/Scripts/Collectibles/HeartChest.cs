using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartChest : MonoBehaviour
{
    private TownHealth healthManager;
    public int healAmount = 10;

    public GameObject chestTop;
    public float openSpeed;
    AudioSource healthSound;

    private bool collected = false;

    public void Start()
    {
        healthSound = GameObject.Find("HealthChestAudio").GetComponent<AudioSource>();
        healthManager = GameObject.Find("GameManager").GetComponent<TownHealth>();
    }

    public void Update()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(startPos.x, startPos.y + 50, startPos.z);
        if (collected)
        {
            transform.position = Vector3.Lerp(startPos, endPos, openSpeed * Time.deltaTime);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = new Vector3(startPos.x, startPos.y + 50, startPos.z);
            chestTop.transform.eulerAngles = new Vector3(chestTop.transform.eulerAngles.x - 45, chestTop.transform.eulerAngles.y, chestTop.transform.eulerAngles.z);

            collected = true;
            healthSound.Play();
            healthManager.AddHealth(healAmount);

            Destroy(gameObject, 5f);
        }
    }
}