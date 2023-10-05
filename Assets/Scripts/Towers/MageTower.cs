using DigitalRuby.LightningBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MageTower : MonoBehaviour, ITower
{
    // Start is called before the first frame update
    public float range = 10f;
    public float fireRate = 1f;
    public float projDamage = .01f;
    public int numTargets = 3;

    public string enemyTag = "Enemy";

    AudioSource shoot;

    public Dictionary<GameObject, GameObject> targets = new Dictionary<GameObject, GameObject>();

    public GameObject projectilePrefab;
    public GameObject firepoint;

    public GameObject nextLevelOpa;
    public GameObject nextLevelOpb;
    public GameObject RangeCircle;
    public int upgradeCost = 100;
    //current level of tower NOTE: 4a and 4b are both 4 as this is for upgrading and both can not be upgraded
    public int status;
    public int price;
    public TowerType towerT;
    public int upgradeCostA;
    public int upgradeCostB;

    private float fireCountdown = 1f;

    public bool enable = false;
    public bool upgrade; //for testing delete later

    public ParticleSystem firerateboost;
    public ParticleSystem firerateslow;

    string passiveChosen = "";

    void Start()
    {
        shoot = GetComponent<AudioSource>();
        InvokeRepeating("UpdateTarget", 0f, 0.1f); // calls update target 2 times a sec probably needs to be changed later
        //firerateboost = GetComponent<ParticleSystem>();
        Vector3 scale = new Vector3(range * 2, .001f, range * 2);
        RangeCircle.transform.localScale = Vector3.zero;
        RangeCircle.transform.localScale += scale;
    }

    // Update is called once per frame
    void Update()
    {
        if (targets.Count == 0 || Time.timeScale == 0)
        {
            if (shoot.isPlaying)
            {
                shoot.Pause();
            }
            return;
        } 
        else
        {
            if (!shoot.isPlaying)
            {
                shoot.Play();
            }
        }
        if (!enable && targets.Count != 0)
        {
            foreach (GameObject enemy in targets.Keys) { Destroy(targets[enemy]); }
            targets = new Dictionary<GameObject, GameObject>();
        }

        if (upgrade)
        {
            Upgrade(0);
        }


        if (fireCountdown <= 0f && enable)
        {
            Shoot();
            fireCountdown = 1 / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        foreach(GameObject enemy in targets.Keys)
        {
            if (enemy != null)
            {
                EnemyController enemyCont = enemy.GetComponent<EnemyController>();
                if (enemyCont == null) { GameManager.instance.GetComponent<Miniboss>().Hit(projDamage); }
                else { enemyCont.Hit(projDamage); }      
            }
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

        foreach (GameObject enemy in enemies)
        {
            if (enemy.CompareTag("Miniboss") && !GameManager.instance.GetComponent<Miniboss>().GetSpawned()) { continue; } 
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= range)
            {
                if (!targets.ContainsKey(enemy) && enable && targets.Keys.Count < numTargets)
                {
                    GameObject projectile = (GameObject)Instantiate(projectilePrefab, firepoint.transform.position, firepoint.transform.rotation);
                    LightningBoltScript lightningbolt = projectile.GetComponent<LightningBoltScript>();

                    if (lightningbolt != null)
                    {
                        lightningbolt.StartObject = firepoint;
                        lightningbolt.EndObject = enemy;

                    }
                    targets[enemy] = projectile;
                }
            }
            else
            {
                if (targets.ContainsKey(enemy))
                {
                    Destroy(targets[enemy]);
                    targets.Remove(enemy);
                }
            }
        }
        List<GameObject> toBeRemoved = new List<GameObject>();
        foreach(GameObject enemy in targets.Keys)
        {
            if(enemy == null)
            {
                toBeRemoved.Add(enemy);
                Destroy(targets[enemy]);
            }
        }
        foreach(GameObject enemy in toBeRemoved)
        {
            targets.Remove(enemy);
        }
    }


    public void Upgrade(int op)
    {
        if (op == 0)
        {
            GameObject inst = (GameObject)Instantiate(nextLevelOpa, transform.position, transform.rotation);
            ITower newMage = inst.GetComponent<ITower>();
            if (status == 3) { newMage.setPassiveChosen("Single Beam"); }
            newMage.Enable();
            newMage.updatePrice(price);
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
            ITower newMage = inst.GetComponent<ITower>();
            newMage.setPassiveChosen("Multi Beam");
            newMage.Enable();
            newMage.updatePrice(price);
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
    public Material getMaterial()
    {
        return this.gameObject.GetComponent<Renderer>().material;
    }

    public bool getEnabled()
    {
        return enable;
    }
    public string getPassiveChosen() { return passiveChosen; }
    public void setPassiveChosen(string setter) { passiveChosen = setter; }
    public int GetUpgradeCost() { return upgradeCost; }
    public int GetStatus() { return status;}
    public int GetPrice() {return price; }
    public void updatePrice(int increase) { price += increase; }
    public TowerType getTowerType () { return towerT; }
    public TowerStats GetStats() { 
        if (status == 3) { return new TowerStats(towerT, status, range, fireRate, projDamage, price, upgradeCostA, upgradeCostB, nextLevelOpa, nextLevelOpb, numTargets, true, "Single Beam", "Multi Beam"); }
        else { return new TowerStats(towerT, status, range, fireRate, projDamage, price, upgradeCostA, upgradeCostB, nextLevelOpa, nextLevelOpb, numTargets, false, "", ""); }

    }
    public GameObject GetGameObject() { return gameObject;}
    #endregion

    #region Enable Disable
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
