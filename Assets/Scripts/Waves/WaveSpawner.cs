using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = System.Random;

[System.Serializable]
public class EnemyPrefabSet
{
    public Transform spearman;
    public Transform knight;
    public Transform archer;
    public Transform mage;
}

[System.Serializable]
public class WaveData
{
    public int spearmanCount;
    public int knightCount;
    public int archerCount;
    public int mageCount;
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private EnemyPrefabSet purpleEnemies;
    [SerializeField] private EnemyPrefabSet blueEnemies;
    [SerializeField] private EnemyPrefabSet greenEnemies;
    [SerializeField] private EnemyPrefabSet yellowEnemies;
    [SerializeField] private EnemyPrefabSet orangeEnemies;
    [SerializeField] private EnemyPrefabSet redEnemies;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private float timeBetweenWaves = 60f;
    [SerializeField] private float initialCountdown = 100f;
    [SerializeField] private int[] miniBossWaveNumbers = { 15, 30, 45, 60 };
    [SerializeField] private int finalWaveNumber = 60;
    [SerializeField] private float minGroupSpawnDelay = 1f;
    [SerializeField] private float maxGroupSpawnDelay = 3f;
    [SerializeField] private float speedScalarBase = 1f;
    [SerializeField] private float speedScalarIncrement = 0.05f;
    [SerializeField] private float speedScalarMax = 1.45f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private TextMeshProUGUI enemiesLeftText;
    [SerializeField] private TextMeshProUGUI waveCounterText;
    [SerializeField] private TextMeshProUGUI bossNextWaveText;
    [SerializeField] private Button skipBuyPhaseButton;
    [SerializeField] private GameObject x1SpeedButton;
    [SerializeField] private GameObject x2SpeedButton;

    [Header("Other References")]
    [SerializeField] private AbilityCutscene cutscenes;
    [SerializeField] private GameObject winScreen;

    // Wave configuration data
    private readonly WaveData[] wavePatterns = new WaveData[]
    {
        new WaveData { spearmanCount = 1, knightCount = 1, archerCount = 1, mageCount = 1 },
        new WaveData { spearmanCount = 1, knightCount = 1, archerCount = 2, mageCount = 1 },
        new WaveData { spearmanCount = 2, knightCount = 1, archerCount = 1, mageCount = 1 },
        new WaveData { spearmanCount = 2, knightCount = 2, archerCount = 1, mageCount = 1 },
        new WaveData { spearmanCount = 2, knightCount = 2, archerCount = 3, mageCount = 1 },
        new WaveData { spearmanCount = 2, knightCount = 3, archerCount = 1, mageCount = 3 },
        new WaveData { spearmanCount = 1, knightCount = 1, archerCount = 3, mageCount = 4 },
        new WaveData { spearmanCount = 2, knightCount = 4, archerCount = 3, mageCount = 4 },
        new WaveData { spearmanCount = 2, knightCount = 3, archerCount = 3, mageCount = 3 },
        new WaveData { spearmanCount = 2, knightCount = 2, archerCount = 2, mageCount = 4 },
        new WaveData { spearmanCount = 1, knightCount = 2, archerCount = 3, mageCount = 4 }
    };

    // State variables
    private float countdown;
    private int waveNumber = 0;
    private int groupsRemaining = 0;
    private float speedScalar = 1f;
    private float prevAtkPhaseTimeScale = 1f;

    // Wave state flags
    private bool waveSpawnComplete = true;
    private bool minibossDefeated = false;
    private bool continuousSpawning = false;
    private bool waveCompletionScheduled = false;
    private bool waveSpawnedThisPhase = false;
    private bool cutsceneActive = false;

    // UI state flags
    private bool atkUIisSetup = false;
    private bool buyUIisSetup = false;

    // Cached data
    private Dictionary<int, EnemyPrefabSet> colorToEnemySet;
    private Dictionary<int, string> colorNames;
    private Random random;

    // Colors for UI
    private readonly Color green = new Color32(55, 255, 0, 255);
    private readonly Color red = new Color32(255, 0, 0, 255);

    private void Start()
    {
        InitializeSpawnPoints();
        InitializeDictionaries();
        random = new Random();
        countdown = initialCountdown;
    }

    /// <summary>
    /// Initializes spawn points from TrackWaypoints if not already set in inspector
    /// </summary>
    private void InitializeSpawnPoints()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = new Transform[]
            {
                TrackWaypoints.points1[0],
                TrackWaypoints.points2[0],
                TrackWaypoints.points3[0],
                TrackWaypoints.points4[0],
                TrackWaypoints.points5[0],
                TrackWaypoints.points6[0]
            };
        }
    }

    /// <summary>
    /// Sets up lookup dictionaries for enemy prefabs and color names
    /// </summary>
    private void InitializeDictionaries()
    {
        colorToEnemySet = new Dictionary<int, EnemyPrefabSet>
        {
            { 0, purpleEnemies },
            { 1, blueEnemies },
            { 2, greenEnemies },
            { 3, yellowEnemies },
            { 4, orangeEnemies },
            { 5, redEnemies }
        };

        colorNames = new Dictionary<int, string>
        {
            { 0, "Purple" },
            { 1, "Blue" },
            { 2, "Green" },
            { 3, "Yellow" },
            { 4, "Orange" },
            { 5, "Red" }
        };
    }

    private void Update()
    {
        HandleWaveTransition();
        UpdateCountdown();
        CheckWaveStart();
        UpdateUI();
    }

    private void HandleWaveTransition()
    {
        if (countdown <= 0 && !GameManager.instance.isInAttackPhase())
        {
            GameManager.instance.attackModeBegin();
        }
    }

    private void UpdateCountdown()
    {
        if (GameManager.instance.isInBuyPhase() && !cutsceneActive)
        {
            countdown -= Time.deltaTime;
        }
    }

    private void CheckWaveStart()
    {
        if (GameManager.instance.isInBuyPhase() || waveCompletionScheduled) 
            return;

        if (ShouldStartContinuousSpawning())
        {
            StartContinuousSpawning();
        }
        else if (ShouldStartNormalWave())
        {
            StartNormalWave();
        }
    }

    /// <summary>
    /// Determines if continuous spawning should start during miniboss waves
    /// </summary>
    private bool ShouldStartContinuousSpawning()
    {
        return GameManager.instance.isInBossRound() && 
               GameManager.instance.isInAttackPhase() && 
               waveSpawnedThisPhase && 
               !minibossDefeated && 
               waveSpawnComplete;
    }

    /// <summary>
    /// Determines if a normal wave should start (not continuous spawning)
    /// </summary>
    private bool ShouldStartNormalWave()
    {
        return GameManager.instance.isInAttackPhase() && 
               waveSpawnComplete && 
               !waveSpawnedThisPhase;
    }

    private void StartContinuousSpawning()
    {
        continuousSpawning = true;
        waveSpawnComplete = false;
        StartCoroutine(SpawnWave());
    }

    private void StartNormalWave()
    {
        waveSpawnComplete = false;
        continuousSpawning = false;
        StartCoroutine(SpawnWave());
    }

    private void CheckWaveCompletion()
    {
        if (IsBossRoundComplete())
        {
            HandleBossRoundCompletion();
        }
        else if (IsNormalWaveComplete())
        {
            HandleNormalWaveCompletion();
        }
    }

    /// <summary>
    /// Checks if the current boss round is complete (miniboss defeated + no enemies left)
    /// </summary>
    private bool IsBossRoundComplete()
    {
        return GameManager.instance.isInBossRound() && 
               GameManager.instance.isInAttackPhase() && 
               minibossDefeated && 
               GetEnemyCount() <= 0;
    }

    /// <summary>
    /// Checks if the current normal wave is complete (no enemies left, not a boss round)
    /// </summary>
    private bool IsNormalWaveComplete()
    {
        return GetEnemyCount() <= 0 && 
               !GameManager.instance.isInBossRound() && 
               GameManager.instance.isInAttackPhase();
    }

    private void HandleBossRoundCompletion()
    {
        cutsceneActive = true;
        cutscenes.startCutscene();
        GameManager.instance.buyModeBegin();
        countdown = timeBetweenWaves;
        
        if (waveNumber == finalWaveNumber + 1)
        {
            TriggerWinCondition();
        }
    }

    private void HandleNormalWaveCompletion()
    {
        minibossDefeated = false;
        GameManager.instance.buyModeBegin();
        countdown = timeBetweenWaves;
        
        if (waveNumber == finalWaveNumber + 1)
        {
            TriggerWinCondition();
        }
    }

    private void TriggerWinCondition()
    {
        winScreen.GetComponent<GameOverScreen>().ShowScreen();
    }

    public IEnumerator SpawnWave()
    {
        UpdateSpeedScalar();

        if (ShouldStopSpawning())
        {
            yield break;
        }

        SpawnMinibossIfNeeded();

        int groupCount = CalculateGroupCount();
        groupsRemaining = groupCount;
        
        yield return SpawnEnemyGroups(groupCount);

        waveSpawnComplete = true;
        if (!continuousSpawning)
        {
            waveSpawnedThisPhase = true;
        }
    }

    /// <summary>
    /// Updates enemy speed based on wave number
    /// </summary>
    private void UpdateSpeedScalar()
    {
        speedScalar = speedScalarBase + (speedScalarIncrement * (waveNumber % 10));
        speedScalar = Mathf.Min(speedScalar, speedScalarMax);
    }

    private bool ShouldStopSpawning()
    {
        return GameManager.instance.isInBossRound() && minibossDefeated;
    }

    private void SpawnMinibossIfNeeded()
    {
        if (Array.Exists(miniBossWaveNumbers, wave => wave == waveNumber) && !continuousSpawning)
        {
            minibossDefeated = false;
            Miniboss.instance.spawnMiniboss();
        }
    }

    /// <summary>
    /// Calculates how many enemy groups to spawn based on wave number and spawning mode
    /// </summary>
    private int CalculateGroupCount()
    {
        if (continuousSpawning)
        {
            return random.Next(1, 4); // 1-3 groups for continuous spawning
        }

        int baseCount = (int)(waveNumber / 10) * 3;
        int groupCount;

        if (waveNumber % 10 == 0)
        {
            groupCount = random.Next(4 + baseCount, 4 + baseCount + waveNumber);
        }
        else
        {
            groupCount = random.Next(1 + baseCount, 1 + baseCount + waveNumber);
        }

        return Mathf.Min(groupCount, 30); // Cap at 30 groups
    }

    /// <summary>
    /// Gets the actual number of enemies and minibosses in the scene
    /// </summary>
    private int GetEnemyCount()
    {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int minibossCount = GameObject.FindGameObjectsWithTag("Miniboss").Length;
        return enemyCount + minibossCount;
    }

    private IEnumerator SpawnEnemyGroups(int groupCount)
    {
        for (int i = 0; i < groupCount; i++)
        {
            WaveData wavePattern = SelectWavePattern();
            SpawnEnemyGroup(wavePattern);
            groupsRemaining--;
            
            // Wait between groups
            if (i < groupCount - 1) // Don't wait after the last group
            {
                float waitTime = (float)(random.NextDouble() * (maxGroupSpawnDelay - minGroupSpawnDelay) + minGroupSpawnDelay);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    /// <summary>
    /// Selects a wave pattern based on wave number (every 10th wave is completely random)
    /// </summary>
    private WaveData SelectWavePattern()
    {
        int patternIndex;
        
        if (waveNumber % 10 == 0)
        {
            patternIndex = random.Next(0, wavePatterns.Length);
        }
        else
        {
            patternIndex = (int)((waveNumber % 10) * 2 * random.NextDouble());
            patternIndex = Mathf.Min(patternIndex, wavePatterns.Length - 1);
        }

        return wavePatterns[patternIndex];
    }

    private int SpawnEnemyGroup(WaveData wavePattern)
    {
        int totalSpawned = 0;
        totalSpawned += SpawnEnemyType(wavePattern.spearmanCount, 1);
        totalSpawned += SpawnEnemyType(wavePattern.knightCount, 2);
        totalSpawned += SpawnEnemyType(wavePattern.archerCount, 3);
        totalSpawned += SpawnEnemyType(wavePattern.mageCount, 4);

        return totalSpawned;
    }

    private int SpawnEnemyType(int count, int enemyType)
    {
        int spawned = 0;
        for (int i = 0; i < count; i++)
        {
            if (SpawnEnemy(enemyType, waveNumber))
            {
                spawned++;
            }
        }
        return spawned;
    }

    private bool SpawnEnemy(int enemyType, int waveNumber)
    {
        if (enemyType == 0) return false;

        int color = DetermineEnemyColor(waveNumber);
        int spawnPath = random.Next(1, 7);
        
        Transform enemyPrefab = GetEnemyPrefab(enemyType, color);
        if (enemyPrefab == null) return false;

        Vector3 spawnPosition = spawnPoints[spawnPath - 1].position;
        Quaternion spawnRotation = spawnPoints[spawnPath - 1].rotation;

        GameObject enemy = Instantiate(enemyPrefab.gameObject, spawnPosition, spawnRotation);
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        
        if (enemyMovement != null)
        {
            enemyMovement.path = spawnPath;
            enemyMovement.setSpeed(speedScalar);
        }
        
        return true;
    }

    /// <summary>
    /// Determines enemy color tier based on wave number with 10% chance for next tier
    /// </summary>
    private int DetermineEnemyColor(int waveNumber)
    {
        int baseColor = (int)(waveNumber / 10);
        
        if (waveNumber % 10 > 7 || waveNumber % 10 == 0)
        {
            // 10% chance to spawn enemy of next color tier
            if (random.NextDouble() > 0.9)
            {
                baseColor += (waveNumber % 10 > 7) ? 1 : -1;
            }
        }

        return Mathf.Clamp(baseColor, 0, 5);
    }

    /// <summary>
    /// Gets the appropriate enemy prefab based on type and color
    /// </summary>
    private Transform GetEnemyPrefab(int enemyType, int color)
    {
        if (!colorToEnemySet.ContainsKey(color))
            return null;

        EnemyPrefabSet enemySet = colorToEnemySet[color];
        
        return enemyType switch
        {
            1 => enemySet.spearman,
            2 => enemySet.knight,
            3 => enemySet.archer,
            4 => enemySet.mage,
            _ => null
        };
    }

    private void UpdateUI()
    {
        UpdateUIText();
        UpdateUISetup();
    }

    private void UpdateUIText()
    {
        countdownText.text = Mathf.Round(countdown).ToString();
        
        if (GameManager.instance.isInBossRound())
        {
            enemiesLeftText.text = "Defeat Miniboss!";
        }
        else if (groupsRemaining > 0)
        {
            enemiesLeftText.text = $"Enemies Spawning!";
        }
        else
        {
            int enemyCount = GetEnemyCount();
            enemiesLeftText.text = $"Enemies Left: {enemyCount}";
        }
        
        waveCounterText.text = $"Wave: {waveNumber}";
    }

    private void UpdateUISetup()
    {
        if (GameManager.instance.isInBuyPhase() && !buyUIisSetup)
        {
            SetupBuyPhaseUI();
        }
        else if (GameManager.instance.isInAttackPhase() && !atkUIisSetup)
        {
            SetupAttackPhaseUI();
        }
    }

    private void SetupBuyPhaseUI()
    {
        waveNumber++;
        
        if (Array.Exists(miniBossWaveNumbers, wave => wave == waveNumber))
        {
            bossNextWaveText.gameObject.SetActive(true);
            bossNextWaveText.color = red;
        }
        
        skipBuyPhaseButton.gameObject.SetActive(true);
        phaseText.text = "Buy Phase";
        SetUIColor(green);
        countdownText.gameObject.SetActive(true);
        
        buyUIisSetup = true;
        atkUIisSetup = false;
        waveSpawnedThisPhase = false;

        prevAtkPhaseTimeScale = Time.timeScale;
        Time.timeScale = 1f;
        SetSpeedButtons(true); // Show x1 button when at normal speed
    }

    private void SetupAttackPhaseUI()
    {
        bossNextWaveText.gameObject.SetActive(false);
        skipBuyPhaseButton.gameObject.SetActive(false);
        phaseText.text = "Attack Phase";
        SetUIColor(red);
        countdownText.gameObject.SetActive(false);
        
        atkUIisSetup = true;
        buyUIisSetup = false;

        Time.timeScale = prevAtkPhaseTimeScale;
        SetSpeedButtons(prevAtkPhaseTimeScale == 1f); // Show x1 button if we're at normal speed
    }

    private void SetUIColor(Color color)
    {
        phaseText.color = color;
        enemiesLeftText.color = color;
        waveCounterText.color = color;
    }

    private void SetSpeedButtons(bool showX1)
    {
        x1SpeedButton.SetActive(showX1);  // Show x1 button when at normal speed
        x2SpeedButton.SetActive(!showX1); // Show x2 button when at fast speed
    }

    // Public methods for external access
    public int GetWaveNumber() => waveNumber;
    public void SetWaveNumber(int num) => waveNumber = num;
    public bool GetBossDefeated() => minibossDefeated;
    
    public void SetMinibossDefeated() => minibossDefeated = true;
    public void SetCutsceneInActive() => cutsceneActive = false;
    
    // Public properties for external access
    public float TimeBetweenWaves => timeBetweenWaves;
    public int[] MiniBossWaveNumbers => miniBossWaveNumbers;
    
    // Public methods to modify settings
    public void AddTimeBetweenWaves(float additionalTime) => timeBetweenWaves += additionalTime;
    
    public void ForceNextWaveBuy() 
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

    public void EnemyDestroy()
    {
        if (!waveCompletionScheduled)
        {
            waveCompletionScheduled = true;
            StartCoroutine(EndOfFrameWaveCompletionCheck());
        }
    }

    private IEnumerator EndOfFrameWaveCompletionCheck()
    {
        yield return new WaitForEndOfFrame();
        waveCompletionScheduled = false;
        CheckWaveCompletion();
    }

    public void SkipBuyPhase()
    {
        countdown = 0;
        GameManager.instance.attackModeBegin();
    }
}
