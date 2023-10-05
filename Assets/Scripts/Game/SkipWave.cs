using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipWave : MonoBehaviour
{
    WaveSpawner ws;
    void Start()
    {
        ws = GameObject.FindGameObjectWithTag("GameManager").GetComponent<WaveSpawner>();
    }

    public void SkipOneWave()
    {
        //Destroy all enemies on the map
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in allEnemies)
        {
            Destroy(e);
        }

        //Set enemies to 0
        ws.setEnemyCounter(0);

        //StopAllCoroutines();

        ws.forceNextWaveBuy();
        //Go to next wave
        //StartCoroutine(ws.SpawnWave());
    }
}
