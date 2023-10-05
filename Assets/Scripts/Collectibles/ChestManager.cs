using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    // Start is called before the first frame update

    List<GameObject> groundListAll;
    List<GameObject> groundListAvailable;

    public GameObject phaseObject;
    private TextMeshProUGUI phaseText;
    public AudioSource popSound;

    public GameObject resourceChest;
    public GameObject healthChest;

    private TownHealth townHealth;

    private bool spawned = false;
    private bool spawnedTutorial = false;

    public float chestTimer = 15f;
    private int minWaveNumberToBeginChestSpawning = 5;

    public void Start()
    {
        phaseText = phaseObject.GetComponent<TextMeshProUGUI>();
        townHealth = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TownHealth>();
    }
    public void Update()
    {
        if (phaseText.text == "Attack Phase" && !spawned && GetComponent<WaveSpawner>().getWaveNumber() >= minWaveNumberToBeginChestSpawning)
        {
            GenerateResourceChests();
            spawned = true;
        }
        if (phaseText.text == "Buy Phase")
        {
            spawned = false;
        }
    }
    void GenerateResourceChests()
    {
        // A List to hold all available ground objects
        groundListAvailable = new List<GameObject>();

        // Grab all GameObject with tag "Ground" into a list.
        groundListAll = new List<GameObject>(GameObject.FindGameObjectsWithTag("Ground"));

        // Iterate through and add ground tiles that don't have a tower built on it.
        foreach (GameObject go in groundListAll)
        {
            //Debug.Log(go.name);
            if (!go.GetComponent<Node>().getTowerBuilt())
            {
                groundListAvailable.Add(go);
            }
        }

        // Get the size of the list and pick a random valid index to spawn a chest
        int max = groundListAvailable.Count;
        int indexToSpawnChest = Random.Range(0, max);

        // Number of Chests to Spawn (0 OR 1)
        int numChestsToSpawn = Random.Range(0, 2);

        if (numChestsToSpawn > 0)
        {
            StartCoroutine(SpawnChestAfterSeconds(indexToSpawnChest));
        }
    }

    //Spawn a Health chest at a given ground index...
    void SpawnHealthChest(int idx)
    {
        //Instantiate a chest at each index.
        GameObject groundToSpawnChest = groundListAvailable[idx];
        Vector3 chestPos = new Vector3(0f, 1f, 0f);

        popSound.Play();
        GameObject hChest = Instantiate(healthChest, groundToSpawnChest.transform.position + chestPos, Quaternion.Euler(0, 180, 0));

        Destroy(hChest, chestTimer);
    }

    //Spawn a Resource chest at a given ground index...
    void SpawnResourceChest(int idx)
    {
        //Instantiate a chest at each index.
        GameObject groundToSpawnChest = groundListAvailable[idx];
        Vector3 chestPos = new Vector3(0f, 1f, 0f);

        popSound.Play();
        GameObject rChest = Instantiate(resourceChest, groundToSpawnChest.transform.position + chestPos, Quaternion.Euler(0, 180, 0));

        Destroy(rChest, chestTimer);
    }

    //Had to make this seperate than the SpawnChest cause it wasnt delaying each spawn
    IEnumerator SpawnChestAfterSeconds(int idx)
    {
        if (townHealth.currentHealth > (townHealth.maxHealth - 10))
        {
            int chestSpawnOffset = Random.Range(4, 21);
            yield return new WaitForSeconds(chestSpawnOffset);
            SpawnResourceChest(idx);
        }
        else
        {
            int chestSpawnOffset = Random.Range(4, 21);
            yield return new WaitForSeconds(chestSpawnOffset);
            SpawnHealthChest(idx);
        }
    }

    public void TutorialChests()
    {
        if (!spawnedTutorial)
        {
            GameObject chest1_node = GameObject.FindGameObjectWithTag("ManualChest");
            GameObject chest2_node = GameObject.FindGameObjectWithTag("ManualChest2");

            Vector3 chestPos = new Vector3(0f, 1f, 0f);

            popSound.Play();
            GameObject rChest = Instantiate(resourceChest, chest1_node.transform.position + chestPos, Quaternion.Euler(0, 180, 0));
            GameObject rChest2 = Instantiate(healthChest, chest2_node.transform.position + chestPos, Quaternion.Euler(0, 180, 0));

            spawnedTutorial = true;
        }
    }
}

