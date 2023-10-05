using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArcherTowerLvl12 : MonoBehaviour
{
    // Start is called before the first frame update
    public float range = 10f;
    public float fireRate = 1f;
    public float projDamage = 1f;

    public string enemyTag = "Enemy";

    public AudioClip bowShot;
    AudioSource shoot;

    public Transform target;

    public GameObject projectilePrefab;
    public Transform firepoint;
    public bool enable = false;

    public int price;
    public TowerType towerT;

    public GameObject ArcherToRotate;
    public GameObject RangeCircle;

    public GameObject nextLevel;
    public int upgradeCost = 100;

    //current level of tower NOTE: 4a and 4b are both 4 as this is for upgrading and both can not be upgraded
    public int status;

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

    void Update()
    {
        if (target == null)
        {
            return;
        }

        ArcherToRotate.transform.rotation = Quaternion.LookRotation(ArcherToRotate.transform.position - target.position);


        if (fireCountdown <= 0f && enable)
        {
            Shoot();
            fireCountdown = 1 / fireRate;
        }

        fireCountdown -= Time.deltaTime;

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
        GameObject inst = (GameObject)Instantiate(nextLevel, transform.position, transform.rotation);
        ITower newArch = inst.GetComponent<ITower>();
        newArch.Enable();
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

    public Material getMaterial()
    {
        return this.gameObject.GetComponent<Renderer>().material;
    }

    public bool getEnabled()
    {
        return enable;
    }

    #region Getters

    public string getPassiveChosen() { return passiveChosen; }
    public void setPassiveChosen(string setter) { passiveChosen = setter; }
    public int GetUpgradeCost() { return upgradeCost; }
    public int GetStatus() { return status; }
    public int GetPrice() { return price; }
    public TowerType getTowerType() { return towerT; }
    public TowerStats GetStats() { return new TowerStats(towerT, status, range, fireRate, projDamage, price, upgradeCost, 0, nextLevel, null, 1, false, "", ""); }
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
