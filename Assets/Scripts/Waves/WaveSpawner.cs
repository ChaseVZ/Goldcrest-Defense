using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = System.Random;

public class WaveSpawner : MonoBehaviour
{
    // purple enemies
    public Transform Pspearman;
    public Transform Pknight;
    public Transform Parcher;
    public Transform Pmage;

    // blue enemies
    public Transform Bspearman;
    public Transform Bknight;
    public Transform Barcher;
    public Transform Bmage;

    // green enemies
    public Transform Gspearman;
    public Transform Gknight;
    public Transform Garcher;
    public Transform Gmage;

    // yellow enemies
    public Transform Yspearman;
    public Transform Yknight;
    public Transform Yarcher;
    public Transform Ymage;

    // orange enemies
    public Transform Ospearman;
    public Transform Oknight;
    public Transform Oarcher;
    public Transform Omage;

    // red enemies
    public Transform Rspearman;
    public Transform Rknight;
    public Transform Rarcher;
    public Transform Rmage;

    private Transform spawnPoint1;
    private Transform spawnPoint2;
    private Transform spawnPoint3;
    private Transform spawnPoint4;
    private Transform spawnPoint5;
    private Transform spawnPoint6;

    public float timeBetweenWaves = 30f;
    public float initialCountdown = 60f;
    float speedScalar = 1f;
    float prevAtkPhaseTimeScale = 1f;
    public int[] miniBossWaveNumbers = {15, 30, 45, 60};
    private int finalWaveNumber = 60;

    private float countdown = 0f;  /* til next wave spawns */
    [SerializeField] private int waveNumber = 1;
    private int enemyCounter = 0;

    private bool waveSpawnComplete = true;  /* all enemies that need to be spawned for the current wave, have been spawned (or not) */
    private bool minibossDefeated = false; /* tracks if miniboss has been defeated */
    private bool continuousSpawning = false; /* tracks if we're in continuous spawning mode during miniboss wave */
    private bool waveCompletionScheduled = false; /* prevents multiple wave completion checks in the same frame */
    private int pendingEnemyDestructions = 0; /* tracks how many enemies are being destroyed this frame */
    private bool waveSpawnedThisPhase = false; /* prevents multiple wave spawns per attack phase (except continuous) */

    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI phase;
    public TextMeshProUGUI enemiesLeft;
    public TextMeshProUGUI waveCounter;
    public TextMeshProUGUI bossNextWave;

    [SerializeField] Button skipBuyPhaseButton;
    bool atkUIisSetup = false;
    bool buyUIisSetup = false;

    private Color green = new Color32(55, 255, 0, 255);
    private Color red = new Color32(255, 0, 0, 255);

    [SerializeField] AbilityCutscene cutscenes;
    private bool cutsceneActive = false;

    [SerializeField] GameObject x1_speed_button;
    [SerializeField] GameObject x2_speed_button;

    public GameObject winScreen;

    // ------------ WAVE SPAWNING ------------
    // Each list is a group of the following:
    // 1 - spearman
    // 2 - knight
    // 3 - archer
    // 4 - mage
    private int[,] wave = new int[,] {
                                    {1, 1, 1, 1},
                                    {1, 1, 2, 1},
                                    {2, 1, 1, 1},
                                    {2, 2, 1, 1},
                                    {2, 2, 3, 1},
                                    {2, 3, 1, 3},
                                    {1, 1, 3, 4},
                                    {2, 4, 3, 4},
                                    {2, 3, 3, 3},
                                    {2, 2, 2, 4},
                                    {1, 2, 3, 4}
                                    };


    private void Start()
    {
        spawnPoint1 = TrackWaypoints.points1[0];
        spawnPoint2 = TrackWaypoints.points2[0];
        spawnPoint3 = TrackWaypoints.points3[0];
        spawnPoint4 = TrackWaypoints.points4[0];
        spawnPoint5 = TrackWaypoints.points5[0];
        spawnPoint6 = TrackWaypoints.points6[0];
        countdown = initialCountdown;
    }

    private void Update()
    {
        if (countdown <= 0 && !GameManager.instance.isInAttackPhase())
        {
            GameManager.instance.attackModeBegin();
            return;
        }

        // Countdown during buy phase
        if (GameManager.instance.isInBuyPhase() && !cutsceneActive)
        {
            countdown -= Time.deltaTime;
        }

        checkWaveStart();
        setUI();
    }

    // Miniboss counts towards enemy counter
    private void checkWaveCompletion()
    {
        // Boss round win condition
        if (GameManager.instance.isInBossRound() && GameManager.instance.isInAttackPhase() && minibossDefeated && enemyCounter <= 0)
        {
            cutsceneActive = true;
            cutscenes.startCutscene();
            GameManager.instance.buyModeBegin();
            countdown = timeBetweenWaves;
            if (waveNumber == finalWaveNumber + 1) { winCondition(); }
            return;
        }

        // Normal wave win condition
        if (enemyCounter <= 0 && !GameManager.instance.isInBossRound() && GameManager.instance.isInAttackPhase())
        {
            minibossDefeated = false;
            GameManager.instance.buyModeBegin();
            countdown = timeBetweenWaves;
            if (waveNumber == finalWaveNumber + 1) { winCondition(); }
        }
    }

    // determine if wave should commence
    private void checkWaveStart()
    {
        if (GameManager.instance.isInBuyPhase() || waveCompletionScheduled) { return; }

        // Handle continuous spawning during miniboss waves (after initial wave spawn)
        if (GameManager.instance.isInBossRound() && GameManager.instance.isInAttackPhase() && waveSpawnedThisPhase && !minibossDefeated && waveSpawnComplete)
        {
            continuousSpawning = true;
            waveSpawnComplete = false;
            StartCoroutine(SpawnWave());
        }
        // Normal wave start (only if no wave has been spawned this phase)
        else if (GameManager.instance.isInAttackPhase() && waveSpawnComplete && !waveSpawnedThisPhase)
        {
            waveSpawnComplete = false;
            continuousSpawning = false; 
            StartCoroutine(SpawnWave());
        }
    }

    public void winCondition()
    {
        winScreen.GetComponent<GameOverScreen>().ShowScreen();
    }

    public IEnumerator SpawnWave()
    {
        speedScalar = 1f + ((0.05f) * (waveNumber % 10)); // oscilates from 1 - 1.45f 

        // Stop spawning if miniboss has been defeated
        if (GameManager.instance.isInBossRound() && minibossDefeated)
        {
            yield break;
        }

        // Spawn miniboss only on initial wave spawn, not during continuous spawning
        if (Array.Exists(miniBossWaveNumbers, wave => wave == waveNumber) && !continuousSpawning)   
        { 
            minibossDefeated = false;
            Miniboss.instance.spawnMiniboss(); 
            enemyCounter++;
        }

        // Determine group count based on spawning mode
        Random groups = new Random();
        int groupCount;
        
        if (continuousSpawning)
        {
            // Continuous spawning: smaller, more frequent waves
            groupCount = groups.Next(1, 4); // 1-3 groups for continuous spawning
        }
        else
        {
            // Normal wave spawning
            int countmult = (int)(waveNumber / 10) * 3;
            if ((waveNumber % 10) == 0)
            {
                groupCount = groups.Next(4 + countmult,4 + countmult + waveNumber);
            }
            else
            {
                groupCount = groups.Next(1 + countmult,1 + countmult + waveNumber);
            }

            // cap number of groups at 30
            if (groupCount > 30)
            {
                groupCount = 30;
            }
        }

        enemyCounter += groupCount * 4;

        for (int i = 0; i < groupCount; i++)
        {
            // random group formation
            Random groupProb = new Random();
            int group;
            // every 10nth wave, make complete random groups for each groupw
            if (waveNumber % 10 == 0)
            {
                group = groupProb.Next(0, 11);
            }
            // else still random but tends to center on wave mod 10. e.g. wave 15 tends towards group 5.
            else
            {
                group = (int)((waveNumber % 10) * 2 * groupProb.NextDouble());
            }
            if (group > 10)
            {
                group = 10;
            }

            for (int j = 0; j < wave.GetLength(1); j++)
            {
                SpawnEnemy(wave[group, j], waveNumber);

                // random wait
                Random rand = new Random();
                float wait = (float)rand.NextDouble();
                yield return new WaitForSeconds(wait);
            }
        }

        // done with coroutine
        waveSpawnComplete = true;
        if (!continuousSpawning)
        {
            waveSpawnedThisPhase = true; // Only set flag for initial wave spawn, not continuous spawning
        }
    }

    private void SpawnEnemy(int type, int waveNumber)
    {
        if (type != 0)
        {
            // random color
            Random colorProb = new Random();
            int color;
            // if the wave is the 8th-10th multiple, give a chance to spawn enemies of next color tier.
            if (waveNumber % 10 > 7)
            {
                // 10% chance to spawn enemy of next color
                double prob = colorProb.NextDouble();
                if (prob > .9f)
                {
                    prob = 1;
                }
                else
                {
                    prob = 0;
                }
                color = (int)(waveNumber / 10) + (int)prob;
            }
            else if (waveNumber % 10 == 0)
            {
                // 10% chance to spawn enemy of next color
                double prob = colorProb.NextDouble();
                if (prob > .9f)
                {
                    prob = 1;
                }
                else
                {
                    prob = 0;
                }
                color = (int)(waveNumber / 10) + (int)prob - 1;
            }
            // else spawn color based on wave divided by 10.
            else
            {
                color = (int)(waveNumber / 10);
            }

            if (color > 5)
            {
                color = 5;
            }
            else if (color < 0)
            {
                color = 0;
            }

            Dictionary<string, Transform> spawnPoints = new Dictionary<string, Transform>();
            spawnPoints.Add("1", spawnPoint1);
            spawnPoints.Add("2", spawnPoint2);
            spawnPoints.Add("3", spawnPoint3);
            spawnPoints.Add("4", spawnPoint4);
            spawnPoints.Add("5", spawnPoint5);
            spawnPoints.Add("6", spawnPoint6);

            Dictionary<int, string> colorMap = new Dictionary<int, string>();
            colorMap.Add(0, "P");
            colorMap.Add(1, "B");
            colorMap.Add(2, "G");
            colorMap.Add(3, "Y");
            colorMap.Add(4, "O");
            colorMap.Add(5, "R");

            Dictionary<int, string> typeMap = new Dictionary<int, string>();
            typeMap.Add(1, "spearman");
            typeMap.Add(2, "knight");
            typeMap.Add(3, "archer");
            typeMap.Add(4, "mage");

            Dictionary<string, Transform> enemyMap = new Dictionary<string, Transform>();
            enemyMap.Add("Pspearman", Pspearman);
            enemyMap.Add("Pknight", Pknight);
            enemyMap.Add("Parcher", Parcher);
            enemyMap.Add("Pmage", Pmage);

            enemyMap.Add("Bspearman", Bspearman);
            enemyMap.Add("Bknight", Bknight);
            enemyMap.Add("Barcher", Barcher);
            enemyMap.Add("Bmage", Bmage);

            enemyMap.Add("Gspearman", Gspearman);
            enemyMap.Add("Gknight", Gknight);
            enemyMap.Add("Garcher", Garcher);
            enemyMap.Add("Gmage", Gmage);

            enemyMap.Add("Yspearman", Yspearman);
            enemyMap.Add("Yknight", Yknight);
            enemyMap.Add("Yarcher", Yarcher);
            enemyMap.Add("Ymage", Ymage);

            enemyMap.Add("Ospearman", Ospearman);
            enemyMap.Add("Oknight", Oknight);
            enemyMap.Add("Oarcher", Oarcher);
            enemyMap.Add("Omage", Omage);

            enemyMap.Add("Rspearman", Rspearman);
            enemyMap.Add("Rknight", Rknight);
            enemyMap.Add("Rarcher", Rarcher);
            enemyMap.Add("Rmage", Rmage);

            // random path 1-6
            Random rnd = new Random();
            int path = rnd.Next(1,7);
            GameObject enemy = (GameObject)Instantiate(enemyMap[colorMap[color] + typeMap[type]],
                                                        spawnPoints[path.ToString()].position,
                                                        spawnPoints[path.ToString()].rotation).gameObject;
                enemy.GetComponent<EnemyMovement>().path = path;
            enemy.GetComponent<EnemyMovement>().setSpeed(speedScalar);
        }
    }

    private void setUI()
    {
        countdownText.text = Mathf.Round(countdown).ToString();
        enemiesLeft.text = GameManager.instance.isInBossRound() ? "Defeat Miniboss!" : "Enemies Left: " + enemyCounter;
        waveCounter.text = "Wave: " + waveNumber.ToString();

        if (GameManager.instance.isInBuyPhase() && !buyUIisSetup)
        {
            waveNumber++;
            if (Array.Exists(miniBossWaveNumbers, wave => wave == waveNumber))
            {
                bossNextWave.gameObject.SetActive(true);
                bossNextWave.color = red;
            }
            skipBuyPhaseButton.gameObject.SetActive(true);
            phase.text = "Buy Phase";
            phase.color = green;
            enemiesLeft.color = green;
            waveCounter.color = green;
            countdownText.gameObject.SetActive(true);
            buyUIisSetup = true;
            atkUIisSetup = false;
            waveSpawnedThisPhase = false; // Reset wave spawn flag when buy phase begins

            prevAtkPhaseTimeScale = Time.timeScale;
            Time.timeScale = 1f;
            x2_speed_button.SetActive(false);
            x1_speed_button.SetActive(false);
            
        }
        else if (GameManager.instance.isInAttackPhase() && !atkUIisSetup)
        {
            bossNextWave.gameObject.SetActive(false);
            skipBuyPhaseButton.gameObject.SetActive(false);
            phase.text = "Attack Phase";
            phase.color = red;
            enemiesLeft.color = red;
            waveCounter.color = red;
            countdownText.gameObject.SetActive(false);
            atkUIisSetup = true;
            buyUIisSetup = false;

            Time.timeScale = prevAtkPhaseTimeScale;
            if (prevAtkPhaseTimeScale == 1f)
            {
                x2_speed_button.SetActive(true);
            }
            else
            {
                x1_speed_button.SetActive(true);
            }
        }
    }

    public int getWaveNumber() { return waveNumber; }
    public void setWaveNumber(int num) { this.waveNumber = num; }
    public void setEnemyCounter(int num) { this.enemyCounter = num;  }
    public bool getBossDefeated() { return minibossDefeated; }
    public void setMinibossDefeated()
    {
        minibossDefeated = true;
    }
    public void setCutsceneInActive()
    {
        cutsceneActive = false;
    }
    public void forceNextWaveBuy() // God Mode feature ONLY
    {
        countdown = timeBetweenWaves;
        if (waveSpawnComplete && GameManager.instance.isInBuyPhase())
        {
            waveNumber++;
        }
        waveSpawnComplete = true;
        continuousSpawning = false; 
        StopAllCoroutines();
    }

    public void enemyDestroy()
    {
        if (enemyCounter > 0)
        {
            enemyCounter--;
        }
        
        pendingEnemyDestructions++;
        
        // Only schedule wave completion check if not already scheduled
        if (!waveCompletionScheduled)
        {
            waveCompletionScheduled = true;
            StartCoroutine(EndOfFrameWaveCompletionCheck());
        }
    }

    private IEnumerator EndOfFrameWaveCompletionCheck()
    {
        // Wait until the end of the current frame to process all enemy destructions
        yield return new WaitForEndOfFrame();

        // Reset the pending count and flag
        pendingEnemyDestructions = 0;
        waveCompletionScheduled = false;
        
        // Now check wave completion with the final enemy count
        checkWaveCompletion();
    }

    public void skipBuyPhase()
    {
        countdown = 0;
        GameManager.instance.attackModeBegin();
    }
}
