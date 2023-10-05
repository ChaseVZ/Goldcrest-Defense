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

    public float timeBetweenWaves = 20f;
    float speedScalar = 1f;
    float prevAtkPhaseTimeScale = 1f;

    private float countdown = 0f;  /* til next wave spawns */
    [SerializeField] private int waveNumber = 1;
    private int enemyCounter = 0;

    private bool waveSpawnComplete = true;  /* all enemies that need to be spawned for the current wave, have been spawned (or not) */
    private bool waveEnemiesComplete = true;  /* are all spawned enemies dead (killed/reached castle)? */
    //private bool stopNextWave = true; /* multi-purpose flag | Potential uses: start of game; before boss fight waves; alchemist stuff */

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

    bool bossDefeated = false;
    [SerializeField] AbilityCutscene cutscenes;

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
        countdown = 30f;
    }

    private void Update()
    {
        checkWaveStatus();

        checkWaveStart();

        setUI();
    }

    public void winCondition()
    {
        winScreen.GetComponent<GameOverScreen>().ShowScreen();
    }

    public IEnumerator SpawnWave()
    {
        speedScalar = 1f + ((0.05f) * (waveNumber % 10)); // oscilates from 1 - 1.45f 

        // random number of groups, extra large wave every 10th wave
        if (waveNumber == 15 || waveNumber == 30 || waveNumber == 45 || waveNumber == 60) { Miniboss.instance.spawnMiniboss(); enemyCounter++;}

        Random groups = new Random();
        int groupCount;
        int countmult = (int)(waveNumber / 10) * 3;
        if ((waveNumber % 10) == 0)
        {
            groupCount = groups.Next(4 + countmult,4 + countmult + waveNumber);
        }
        else
        {
            groupCount = groups.Next(1 + countmult,1 + countmult + waveNumber);
        }

        // cap number of groups at 20
        if (groupCount > 30)
        {
            groupCount = 30;
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
        enemiesLeft.text = "Enemies Left: " + enemyCounter;
        waveCounter.text = "Wave: " + waveNumber.ToString();

        if (waveSpawnComplete && waveEnemiesComplete && !buyUIisSetup)
        {
            waveNumber++;
            if (waveNumber == 15 || waveNumber == 30 || waveNumber == 45 || waveNumber == 60)
            {
                bossNextWave.gameObject.SetActive(true);
                bossNextWave.color = red;
            }
            skipBuyPhaseButton.gameObject.SetActive(true);
            GameManager.instance.buyModeBegin();
            phase.text = "Buy Phase";
            phase.color = green;
            enemiesLeft.color = green;
            waveCounter.color = green;
            countdownText.gameObject.SetActive(true);
            buyUIisSetup = true;
            atkUIisSetup = false;

            prevAtkPhaseTimeScale = Time.timeScale;
            Time.timeScale = 1f;
            x2_speed_button.SetActive(false);
            x1_speed_button.SetActive(false);
            
        }
        else if (!waveSpawnComplete && !waveEnemiesComplete && !atkUIisSetup)
        {
            bossNextWave.gameObject.SetActive(false);
            skipBuyPhaseButton.gameObject.SetActive(false);
            GameManager.instance.attackModeBegin();
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

    private void checkWaveStatus()
    {
        // check if all spawned enemies died/finished course
        if (enemyCounter == 0)
        {
            waveEnemiesComplete = true;
        }
        else
        {
            waveEnemiesComplete = false;
        }

        // only countdown once wave spawning & killing is done
        if (waveSpawnComplete && waveEnemiesComplete)
        {
            if (bossDefeated) { cutscenes.startCutscene(); }
            else { countdown -= Time.deltaTime; }
            
            if (waveNumber == 61) { winCondition(); }
        }
    }

    // determine if wave should commence
    private void checkWaveStart()
    {
        // wave begin
        if (countdown <= 0 && waveSpawnComplete && waveEnemiesComplete)
        {
            countdown = timeBetweenWaves;
            waveSpawnComplete = false;
            waveEnemiesComplete = false;
            StartCoroutine(SpawnWave());
        }
    }

    public bool getWaveStatus() { return waveEnemiesComplete; }
    public int getWaveNumber() { return waveNumber; }

    public void setWaveNumber(int num) { this.waveNumber = num; }

    public void setEnemyCounter(int num) { this.enemyCounter = num;  }

    public void forceNextWaveBuy()
    {
        countdown = timeBetweenWaves;
        if (waveSpawnComplete && waveEnemiesComplete)
        {
            waveNumber++;
        }
        waveSpawnComplete = true;
        waveEnemiesComplete = true;
        StopAllCoroutines();
    }

    public void enemyDestroy()
    {
        if (enemyCounter > 0)
        {
            enemyCounter = enemyCounter - 1;
        }
    }

    public void skipBuyPhase()
    {
        countdown = 0;
    }


    public void setBossDefeated(bool setter)
    {
        bossDefeated = setter;
    }

    public bool getBossDefeated() { return bossDefeated; }

}
