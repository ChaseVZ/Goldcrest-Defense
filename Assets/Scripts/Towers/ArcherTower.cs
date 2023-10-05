using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArcherTower : MonoBehaviour, ITower
{
    public float range;
    public float fireRate;
    public float projDamage;

    public string enemyTag = "Enemy";

    public AudioClip bowShot;
    AudioSource shoot;

    public Transform target;

    public GameObject projectilePrefab;
    public Transform firepoint;
    public Transform firepoint2;
    public bool enable = false;

    // tower specific info (placed on prefab)
    public int status;
    public int price;
    public TowerType towerT;
    public int upgradeCostA; // use for single paths too
    public int upgradeCostB;
    public bool hasTwoPaths;
    public int numArchers;
    public bool archersVisible;
    public bool Is4b = false;

    public GameObject ArcherToRotate;
    public GameObject ArcherToRotate2;
    public GameObject RangeCircle;
    public GameObject nextLevelOpa; // use for single paths too
    public GameObject nextLevelOpb;

    private float fireCountdown = 0f;

    public ParticleSystem firerateboost;
    public ParticleSystem firerateslow;

    string passiveChosen = "";

    void Start()
    {
        shoot = GetComponent<AudioSource>();
        InvokeRepeating("UpdateTarget", 0f, 0.1f); // calls update target 2 times a sec probably needs to be changed later
        Vector3 scale = new Vector3(range * 2, .001f, range * 2);
        RangeCircle.transform.localScale = Vector3.zero;
        RangeCircle.transform.localScale += scale;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) { return; } // nothing to shoot

        if (archersVisible) { OrientateArchers(); }

        if (fireCountdown <= 0f && enable)
        {
            if (numArchers == 1) { Shoot(); }
            else if (numArchers == 2) { ShootDouble(); }
            fireCountdown = 1 / fireRate;
        }

        fireCountdown -= Time.deltaTime;

    }
    void OrientateArchers()
    {
        if (numArchers > 0) { ArcherToRotate.transform.rotation = Quaternion.LookRotation(ArcherToRotate.transform.position - target.position); }
        if (numArchers > 1) { ArcherToRotate2.transform.rotation = Quaternion.LookRotation(ArcherToRotate2.transform.position - target.position); }
    }

    void Shoot()
    {
        shoot.PlayOneShot(bowShot, 1f);
        GameObject projectile = (GameObject)Instantiate(projectilePrefab, firepoint.position, firepoint.rotation);
        ArcherProjectile archProjectile = projectile.GetComponent<ArcherProjectile>();

        if (archProjectile != null)
        {
            archProjectile.Seek(target, projDamage);
        }
    }

    void ShootDouble()
    {
        shoot.PlayOneShot(bowShot, 1f);
        GameObject projectile1 = (GameObject)Instantiate(projectilePrefab, firepoint.position, firepoint.rotation);
        GameObject projectile2 = (GameObject)Instantiate(projectilePrefab, firepoint2.position, firepoint2.rotation);
        ArcherProjectile archProjectile1 = projectile1.GetComponent<ArcherProjectile>();
        ArcherProjectile archProjectile2 = projectile2.GetComponent<ArcherProjectile>();

        if (archProjectile1 != null)
        {
            archProjectile1.Seek(target, projDamage);
        }

        if (archProjectile2 != null)
        {
            archProjectile2.Seek(target, projDamage);
        }
    }

    //allows you to see range when selected in editor can use this to show in game
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Miniboss");

        enemies = enemies.Concat(bosses).ToArray();

        //float shortestDistance = Mathf.Infinity;
        float firstEnemy = 0;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();

            float enemyMove = 0;
            if (enemyMovement == null)  /* if enemy is a boss and is spawned */
            {
                if (GameManager.instance.GetComponent<Miniboss>().GetSpawned()) { enemyMove = GameManager.instance.GetComponent<Miniboss>().GetDistancedMoved(); }
            }
            else { enemyMove = enemyMovement.getDistanceMoved(); }

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (firstEnemy < enemyMove && distance <= range)
            {
                firstEnemy = enemyMove;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    public void Upgrade(int op)
    {
        if (op == 0)
        {
            GameObject inst = (GameObject)Instantiate(nextLevelOpa, transform.position, transform.rotation);
            ITower newArch = inst.GetComponent<ITower>();
            if (status == 3) { newArch.setPassiveChosen("Sniper"); }
            newArch.Enable();
            newArch.updatePrice(price);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 20f))
            {
                if (hit.collider.transform.gameObject.tag == "Ground")
                {
                    hit.transform.gameObject.GetComponent<Node>().SetTower(inst);
                }
            }
            Destroy(gameObject);
        }
        else
        {
            GameObject inst = (GameObject)Instantiate(nextLevelOpb, transform.position, transform.rotation);
            ITower newArch = inst.GetComponent<ITower>();
            newArch.setPassiveChosen("Dual Archer");
            newArch.Enable();
            newArch.updatePrice(price);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 20f))
            {
                if (hit.collider.transform.gameObject.tag == "Ground")
                {
                    hit.transform.gameObject.GetComponent<Node>().SetTower(inst);
                }
            }
            Destroy(gameObject);
        }
    }

    #region Helpers
    public void boostFirerate(float magnitude, float usetime)
    {
        float original = fireRate;
        fireRate *= magnitude;
        StartCoroutine(waitUseTime(usetime, original));
    }

    IEnumerator waitUseTime(float usetime, float original)
    {
        firerateboost.Play();
        yield return new WaitForSeconds(usetime);
        firerateboost.Stop();
        fireRate = original;
    }

    public void Slow(float magnitude, float usetime)
    {
        float original = fireRate;
        fireRate /= magnitude;
        StartCoroutine(waitUseTimeSlow(usetime, original));
    }

    IEnumerator waitUseTimeSlow(float usetime, float original)
    {
        firerateslow.Play();
        yield return new WaitForSeconds(usetime);
        firerateslow.Stop();
        fireRate = original;
    }
    #endregion

    #region Getters
    public Material getMaterial(){ return this.gameObject.GetComponent<Renderer>().material; }
    public bool getEnabled(){ return enable; }
    public string getPassiveChosen() { return passiveChosen; }
    public void setPassiveChosen(string setter) { passiveChosen = setter; }
    public int GetUpgradeCost() { return upgradeCostA; }
    public int GetStatus() { return status; }
    public int GetPrice() { return price; }
    public void updatePrice(int increase) { price += increase; }
    public TowerType getTowerType() { return towerT; }
    public TowerStats GetStats() { 
        if (status == 3) { return new TowerStats(towerT, status, range, fireRate, projDamage, price, upgradeCostA, upgradeCostB, nextLevelOpa, nextLevelOpb, 1, true, "Sniper", "Dual Archer"); }
        else if (Is4b) { return new TowerStats(towerT, status, range, fireRate, projDamage * 2, price, upgradeCostA, upgradeCostB, nextLevelOpa, nextLevelOpb, 1, true, "Sniper", "Dual Archer"); }
        else { return new TowerStats(towerT, status, range, fireRate, projDamage, price, upgradeCostA, 0, nextLevelOpa, null, 1, false, "", ""); }
    }
    public GameObject GetGameObject() { return gameObject; }
    #endregion

    # region Enable Disable
    public void Enable()
    {
        enable = true;
    }

    public void Disable()
    {
        enable = false;
    }

    public void EnableRange()
    {
        RangeCircle.gameObject.SetActive(true);
    }

    public void DisableRange()
    {
        RangeCircle.gameObject.SetActive(false);
    }
    #endregion
}
