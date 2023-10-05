using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Random = System.Random;

public class Miniboss : MonoBehaviour
{

    // TODO: public magnitudes
    //       public ranges
    public static Miniboss instance;
    Dictionary<string, Transform[]> spawnPoints;
    Dictionary<GameObject, Material> setEnableTowerMats;
    int path;
    Random rnd;

    [Header("Boss 1")]
    [SerializeField] GameObject boss1;
    [SerializeField] float speed1;
    [SerializeField] float atkCooldown1;
    [SerializeField] float speedUpTime;
    [SerializeField] float health1;
    [SerializeField] float speedMagnitude;
    [SerializeField] string boss1_name;
    [SerializeField] int boss1_moneyDrop;

    [Header("Boss 2")]
    [SerializeField] GameObject boss2;
    [SerializeField] float speed2;
    [SerializeField] float atkCooldown2;
    [SerializeField] float slownessTime;
    [SerializeField] float health2;
    [SerializeField] float slowMagnitude;
    [SerializeField] string boss2_name;
    [SerializeField] int boss2_moneyDrop;

    [Header("Boss 3")]
    [SerializeField] GameObject boss3;
    [SerializeField] float speed3;
    [SerializeField] float atkCooldown3;
    [SerializeField] float health3;
    [SerializeField] string boss3_name;
    [SerializeField] int moneyStealAmount;
    [SerializeField] TextMeshProUGUI moneyStolen;
    [SerializeField] int boss3_moneyDrop;

    [Header("Final Boss")]
    [SerializeField] GameObject boss4;
    [SerializeField] float speed4;
    [SerializeField] float atkCooldown4;
    [SerializeField] float towerDisabledTime;
    [SerializeField] float health4;
    [SerializeField] string boss4_name;
    [SerializeField] int maxNumTowers;
    [SerializeField] int boss4_moneyDrop; /* game ends after so doesnt matter */
    [SerializeField] Material disableTowerMat;
    

    public Slider healthBarGO;
    public Image GeneralPurpose_healthBar;
    public TextMeshProUGUI healthBar_text;
    float currBossMaxHealth;
    float currBossDistanceMoved;
    AudioSource currBossAtkAudio;
    AudioSource deathAudio;

    GameObject[] bosses;
    float[] mvmSpeeds;
    float[] healths;
    string[] names;
    int[] moneyDrops;

    /* when they each end of path */
    /* TODO: might want to make a dmg for each boss */
    float universalDMG = 30f;
    float universalAtkCooldown = 4f;
    bool universalAtk = false;

    TownHealth townHealth;
    WaveSpawner waveSpawner;
    ResourceManager resourceManager;

    GameObject currBoss;
    ParticleSystem currPS;
    ParticleSystem currAtkPS;
    Transform spawnpoint;
    Transform targetPos;

    bool spawned = false;
    bool justSpawned = false;
    bool waitingForCooldown = false;

    [SerializeField] bool forceBossSpawn = false;

    int bossNum = 0;
    int wavepointIndex = 0;

    int isVictory;
    int isAttack;
    int isAttackCastle;
    int isStrike;
    int isAttackCastleStriking;
    int isDeath;
    Animator animator;

    [SerializeField] GameObject abilitiesGO;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    #region Spawn
    public void spawnMiniboss()
    {
        if (spawned) { Death(); }
        currBoss = Instantiate(bosses[bossNum], spawnpoint.position, spawnpoint.rotation).gameObject;
        animator = currBoss.GetComponent<Animator>();
        currBossAtkAudio = currBoss.GetComponent<AudioSource>();
        currPS = currBoss.gameObject.transform.Find("ShockWave").gameObject.transform.GetChild(bossNum).GetComponent<ParticleSystem>();
        currAtkPS = currBoss.gameObject.transform.Find("root").gameObject.transform.Find("pelvis").gameObject.transform.Find("Weapon").gameObject.transform.GetChild(0).gameObject.transform.Find("AttackPS").GetComponent<ParticleSystem>();
        currBossMaxHealth = healths[bossNum];
        healthBarGO.gameObject.SetActive(true);
        GeneralPurpose_healthBar.fillAmount = 1;
        healthBarGO.value = 100;
        healthBar_text.text = names[bossNum] + ": " + healths[bossNum] + "/" + currBossMaxHealth;   
        bossNum++;
        spawned = true;
        justSpawned = true;
        currBossDistanceMoved = 0;

        if (bossNum == 3) { moneyStolen.gameObject.SetActive(true); }
        //if (bossNum == 4) { animator.SetBool(isAttackCastleStriking, true); }
    }
    #endregion

    #region Tower call Functions
    public void Hit(float dmg)
    {
        if (spawned)
        {
            healths[bossNum - 1] -= dmg;
            GeneralPurpose_healthBar.fillAmount = healths[bossNum - 1] / currBossMaxHealth;
            healthBarGO.value = GeneralPurpose_healthBar.fillAmount * 100;
            //healthBar_text.text = names[bossNum - 1] + ": " + Mathf.Round(healths[bossNum - 1] * 100f) / 100f + "/" + currBossMaxHealth;   // 2 decimal places
            healthBar_text.text = names[bossNum - 1] + ": " + Mathf.Round(healths[bossNum - 1]) + "/" + currBossMaxHealth;

            if (healths[bossNum - 1] <= 0) { Death(); }
        }
    }

    public float GetDistancedMoved() { return currBossDistanceMoved; }

    public bool GetSpawned() { return spawned; }
    #endregion

    void Start()
    {
        townHealth = GameObject.Find("GameManager").GetComponent<TownHealth>();
        resourceManager = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();
        deathAudio = GameObject.Find("BossDeath").GetComponent<AudioSource>();

        rnd = new Random();
        path = rnd.Next(1,7);

        spawnPoints = new Dictionary<string, Transform[]>();
        spawnPoints.Add("1", TrackWaypoints.points1);
        spawnPoints.Add("2", TrackWaypoints.points2);
        spawnPoints.Add("3", TrackWaypoints.points3);
        spawnPoints.Add("4", TrackWaypoints.points4);
        spawnPoints.Add("5", TrackWaypoints.points5);
        spawnPoints.Add("6", TrackWaypoints.points6);

        setEnableTowerMats = new Dictionary<GameObject, Material>();

        spawnpoint = spawnPoints[path.ToString()][0];
        targetPos = spawnPoints[path.ToString()][0];
        bosses = new GameObject[] { boss1, boss2, boss3, boss4 };
        mvmSpeeds = new float[] { speed1, speed2, speed3, speed4 };
        healths = new float[] { health1, health2, health3, health4 };
        names = new string[] { boss1_name, boss2_name, boss3_name, boss4_name };
        moneyDrops = new int[] { boss1_moneyDrop, boss2_moneyDrop, boss3_moneyDrop, boss4_moneyDrop };

        isVictory = Animator.StringToHash("Victory");
        isAttack = Animator.StringToHash("Attack");
        isStrike = Animator.StringToHash("Striking");
        isDeath = Animator.StringToHash("Death");
        isAttackCastle = Animator.StringToHash("AttackCastle");
        isAttackCastleStriking = Animator.StringToHash("AttackCastleStriking");

        waveSpawner = GameObject.Find("GameManager").GetComponent<WaveSpawner>();

        currBossDistanceMoved = 0;
    }

    void Update()
    {
        if (spawned) 
        { 
            if (justSpawned) { justSpawned = false; StartCoroutine(attack(atkCooldown1));  return; } 
            if (universalAtk) { AttakingCastle(); return; } /* dont move or do abilities when doing dmg (NOTE: might want to change) */

            if (!animator.GetBool(isAttack)) { minibossMove(); } /* have boss stop while attacking */
            if (bossNum == 1) { miniBoss1_attack(); }
            else if (bossNum == 2) { miniBoss2_attack(); }
            else if (bossNum == 3) { miniBoss3_attack(); }
            else if (bossNum == 4) { miniBoss4_attack(); }
        }

        if (forceBossSpawn) { spawnMiniboss(); forceBossSpawn = false; }
    }

    #region Attack Functions
    void miniBoss1_attack()
    {
        if (animator.GetBool(isAttack)) /* ATTACK: wait for strike, strike, wait for linger, walk, wait for cooldown */
        {
            if (!currBossAtkAudio.isPlaying) { currBossAtkAudio.Play(); }
            waitingForCooldown = true;
            if (animator.GetBool(isStrike)) /* STRIKE: Particles + buff */
            {
                attack1_helper();
                animator.SetBool(isStrike, false);
            }
        }
        else /* if not attacking, start cooldown */
        {
            if (waitingForCooldown) { StartCoroutine(attack(atkCooldown1)); }
        }         
    }

    void attack1_helper()
    {
        currPS.Play();
        currAtkPS.Play();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(enemy.transform.position, currBoss.transform.position) <= 50f)
            {
                enemy.GetComponent<EnemyMovement>().fast(speedMagnitude, speedUpTime);
            }
        }
    }

    void miniBoss2_attack()
    {
        if (animator.GetBool(isAttack)) /* ATTACK: wait for strike, strike, wait for linger, walk, wait for cooldown */
        {
            if (!currBossAtkAudio.isPlaying) { currBossAtkAudio.Play(); }
            waitingForCooldown = true;
            if (animator.GetBool(isStrike)) /* STRIKE: Particles + buff */
            {
                currPS.Play();
                currAtkPS.Play();
                GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
                foreach (GameObject tower in towers)
                {
                    if (Vector3.Distance(tower.transform.position, currBoss.transform.position) <= 50f)
                    {
                        tower.GetComponent<ITower>().Slow(slowMagnitude, slownessTime); 
                    }
                }
                animator.SetBool(isStrike, false);
            }
        }
        else /* if not attacking, start cooldown */
        {
            if (waitingForCooldown) { StartCoroutine(attack(atkCooldown2)); }
        }
    }

    void miniBoss3_attack()
    {
        if (animator.GetBool(isAttack)) /* ATTACK: wait for strike, strike, wait for linger, walk, wait for cooldown */
        {
            if (!currBossAtkAudio.isPlaying) { currBossAtkAudio.Play(); }
            waitingForCooldown = true;
            if (animator.GetBool(isStrike)) /* STRIKE: Particles + buff */
            {
                currPS.Play();
                currAtkPS.Play();
                int stolen = resourceManager.spendResource(moneyStealAmount);
                moneyStolen.text = moneyStolen.text.Replace("$", "");
                moneyStolen.text = "$" + (int.Parse(moneyStolen.text) + stolen).ToString();

                animator.SetBool(isStrike, false);
            }
        }
        else /* if not attacking, start cooldown */
        {
            if (waitingForCooldown) { StartCoroutine(attack(atkCooldown3)); }
        }
    }

    void miniBoss4_attack()
    {
        if (animator.GetBool(isAttack)) /* ATTACK: wait for strike, strike, wait for linger, walk, wait for cooldown */
        {
            if (!currBossAtkAudio.isPlaying) { currBossAtkAudio.Play(); }
            waitingForCooldown = true;
            if (animator.GetBool(isStrike)) /* STRIKE: Particles + buff */
            {
                currPS.Play();
                currAtkPS.Play();

                var array = GameObject.FindGameObjectsWithTag("Tower");
                List<GameObject> towers = new List<GameObject> (array);
                towers.Sort(ByDistance);     

                int numDisabled = 0;

                foreach (GameObject tower in towers)
                {
                    if (Vector3.Distance(tower.transform.position, currBoss.transform.position) <= 50f)
                    {
                        tower.GetComponent<ITower>().Disable();
                        setDisabledTower(tower);
                        numDisabled++;
                    }

                    if (numDisabled == maxNumTowers) { break; }
                }


                StartCoroutine(waitDisable(towerDisabledTime));

                animator.SetBool(isStrike, false);
            }
        }
        else /* if not attacking, start cooldown */
        {
            if (waitingForCooldown) { StartCoroutine(attack(atkCooldown4)); }
        }
    }

    /* for now just change tower material */
    void setDisabledTower(GameObject tower)
    {
        setEnableTowerMats.Add(tower, tower.GetComponent<Renderer>().material);
        tower.GetComponent<Renderer>().material = disableTowerMat;

        List<GameObject> itemsToChange = new List<GameObject>();  /* shouldnt alter a transform directly so use this list */
        foreach (Transform child in tower.transform)
        {
            itemsToChange.Add(child.gameObject);
            if (child.name == "TB_Soldier_Archer_Violet" || child.name == "TB_Soldier_Archer_Yellow" || child.name == "TB_Soldier_Archer_Red" || child.name == "TB_Soldier_Archer_Red (1)") /* grab all the parts of the archer */
            {
                foreach (Transform subcomp in child.transform)
                {
                    itemsToChange.Add(subcomp.gameObject);
                }
            }
        }

        foreach (GameObject child in itemsToChange)
        {
            if (!child.CompareTag("ParticleSystem") && child.name != "Cylinder" && child.GetComponent<Renderer>())
            {
                setEnableTowerMats.Add(child, child.GetComponent<Renderer>().material);
                child.GetComponent<Renderer>().material = disableTowerMat;
            }
        }

    }

    /* for now just restore tower mat */
    void setEnabledTower()
    {
        foreach (KeyValuePair<GameObject, Material> pair in setEnableTowerMats)
        {
            pair.Key.GetComponent<Renderer>().material = pair.Value;
            if (pair.Key.CompareTag("Tower")) { pair.Key.GetComponent<ITower>().Enable(); }     
        }

        setEnableTowerMats = new Dictionary<GameObject, Material>();
    }

    #region AtkHelpers
    IEnumerator attack(float cooldown)
    {
        waitingForCooldown = false;
        yield return new WaitForSeconds(cooldown);
        animator.SetBool(isAttack, true);
    }

    IEnumerator waitDisable(float usetime)
    {
        yield return new WaitForSeconds(usetime);
        setEnabledTower();
    }

    int ByDistance(GameObject a, GameObject b)
    {
        var dstToA = Vector3.Distance(currBoss.transform.position, a.transform.position);
        var dstToB = Vector3.Distance(currBoss.transform.position, b.transform.position);
        return dstToA.CompareTo(dstToB);
    }
    #endregion

    #endregion

    #region Movement 
    void minibossMove()
    {
        Vector3 dir = targetPos.position - currBoss.transform.position;
        Vector3 movement = dir.normalized * mvmSpeeds[bossNum-1] * Time.deltaTime;
        currBoss.transform.Translate(movement, Space.World);

        currBossDistanceMoved += Mathf.Abs(movement.x) + Mathf.Abs(movement.z) + Mathf.Abs(movement.y);


        if (movement != Vector3.zero)
        {
            currBoss.transform.rotation = getRotation(movement);
        }

        if (Vector3.Distance(currBoss.transform.position, targetPos.position) <= 0.5f)
        {
            GetNextWaypoint();
        }
    }
    #endregion

    #region Helper Functions
    Quaternion getRotation(Vector3 movement)
    {
        float distToNext = Vector3.Distance(currBoss.transform.position, targetPos.position);
        Quaternion currRot = Quaternion.LookRotation(movement);

        if (distToNext <= 5f && wavepointIndex+1 != spawnPoints[path.ToString()].Length - 1)
        {
            Vector3 nextDir = spawnPoints[path.ToString()][wavepointIndex + 1].position - spawnPoints[path.ToString()][wavepointIndex].position;
            Quaternion nextRot = Quaternion.LookRotation(nextDir.normalized);
            return Quaternion.Lerp(nextRot, currRot, (distToNext / 5.0f));
        };
        return currRot;
    }

    void GetNextWaypoint()
    {
        if (wavepointIndex >= spawnPoints[path.ToString()].Length - 2) 
        {
            universalAtk = true;
            waitingForCooldown = true;
            StopAllCoroutines();
            animator.SetBool(isAttack, false);
            animator.SetBool(isStrike, false);
            animator.SetBool(isAttackCastle, true);
            AttakingCastle();
            return;
        }
        else
        {
            wavepointIndex++;
            targetPos = spawnPoints[path.ToString()][wavepointIndex];
        }
    }

    void Victory()
    {
        animator.SetBool(isVictory, true);
    }

    void AttakingCastle()
    {
        if (animator.GetBool(isAttackCastleStriking))
        {
            townHealth.TakeDamage(universalDMG);
            if (townHealth.currentHealth <= 0) { Victory(); }

            waitingForCooldown = true;
            animator.SetBool(isAttackCastleStriking, false);

            // if (bossNum == 1) { attack1_helper(); }
        }

        if (!animator.GetBool(isAttackCastle) && waitingForCooldown)
        {
            StartCoroutine(waitAttack(universalAtkCooldown)); 
        }
    }

    IEnumerator waitAttack(float usetime)
    {
        waitingForCooldown = false;
        yield return new WaitForSeconds(usetime);

        if(!animator.GetBool(isVictory)) { animator.SetBool(isAttackCastle, true); }
    }

    void Death()
    {
        if (bossNum == 1) { abilitiesGO.GetComponent<Abilities>().setUnlockAbility1(); }
        else if (bossNum == 2) { abilitiesGO.GetComponent<Abilities>().setUnlockAbility2(); }
        else if (bossNum == 3) 
        { 
            abilitiesGO.GetComponent<Abilities>().setUnlockAbility3();
            moneyStolen.text = moneyStolen.text.Replace("$", "");
            resourceManager.addResource(int.Parse(moneyStolen.text));
            moneyStolen.gameObject.SetActive(false); 
        }

        animator.SetBool(isDeath, true);
        spawned = false;
        wavepointIndex = 0;
        Destroy(currBoss, 4f);
        targetPos = spawnPoints[path.ToString()][0];
        StopAllCoroutines();

        if (animator.GetBool(isAttack)) { animator.SetBool(isAttack, false); }
        if (animator.GetBool(isStrike)) { animator.SetBool(isStrike, false); }
        if (currBossAtkAudio.isPlaying) { currBossAtkAudio.Stop(); }

        deathAudio.Play();

        waveSpawner.enemyDestroy();
        healthBarGO.gameObject.SetActive(false);

        resourceManager.addResource(moneyDrops[bossNum - 1]);

        universalAtk = false;

        waveSpawner.timeBetweenWaves += 10;
    }

    #endregion


}
