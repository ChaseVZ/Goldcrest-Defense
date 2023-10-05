using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceChest : MonoBehaviour
{
    private ResourceManager resourceManager;
    public int resourceAmount = 10;

    public GameObject chestTop;
    public float openSpeed;
    AudioSource resourceSound;

    private bool collected = false;

    public void Start()
    {
        resourceSound = GameObject.Find("ResourceChestAudio").GetComponent<AudioSource>();
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
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
            resourceSound.Play();
            resourceManager.addResource(resourceAmount);

            Destroy(gameObject, 5f);
        }
    }
}
