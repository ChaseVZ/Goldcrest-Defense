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

        
        GameObject[] allMinibosses = GameObject.FindGameObjectsWithTag("Miniboss");
        foreach (GameObject m in allMinibosses)
        {
            Destroy(m);
        }

        //StopAllCoroutines();

        ws.ForceNextWaveBuy();
        //Go to next wave
        //StartCoroutine(ws.SpawnWave());
    }
}
