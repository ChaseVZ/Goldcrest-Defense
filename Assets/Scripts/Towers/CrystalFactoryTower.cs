using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalFactoryTower : MonoBehaviour, ITower
{
    public float range = 10f;
    public float fireRate = 1f;
    public int moneyGain = 10;
    private float fireCountdown = 0f;
    public float duration = 10f; // time that projectile lasts (set at Start) (MUST UPDATE IN PREFAB AFTER CHANGING PROJ VALUE OTHERWISE MENU WONT UPDATE RETROACTIVELY)
    //public float time = 1f;
    public bool enable;

    //current level of tower NOTE: 4a and 4b are both 4 as this is for upgrading and both can not be upgraded
    public int status;
    public int price;
    public GameObject projectilePrefab;
    public TowerType towerT;
    public int upgradeCostA;
    public int upgradeCostB;
    public GameObject nextLevelOpa;
    public GameObject nextLevelOpb;
    public GameObject RangeCircle;

    public Transform firepoint;
    public ParticleSystem firerateboost;
    public ParticleSystem firerateslow;

    private WaveSpawner waveSpawner;

    [Header("Lvl4 Stuff")]
    public bool isLvl4;
    public ParticleSystem resourcePS;
    private ResourceManager resourceManager;

    string passiveChosen = "";

    private void Awake()
    {
        if (projectilePrefab && projectilePrefab.GetComponent<CrystalProjectile>())
            duration = projectilePrefab.GetComponent<CrystalProjectile>().destroyTimer;
    }

    void Start()
    {
        Vector3 scale = new Vector3(range * 2, .001f, range * 2);
        RangeCircle.transform.localScale = Vector3.zero;
        RangeCircle.transform.localScale += scale;
        waveSpawner = GameObject.FindGameObjectWithTag("GameManager").GetComponent<WaveSpawner>();

        if (isLvl4)
        {
            resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        }
    }

    void Update()
    {
        //if (targets.Count == 0)
        //{
        //    return;
        //}
        bool isBuy = false;
        if (!GameManager.instance.tutorialMode)
        {
            isBuy = waveSpawner.getWaveStatus();
        }


        if (fireCountdown <= 0f && enable && !isBuy && !GameManager.instance.tutorialMode)
        {
            if(!isLvl4)
            {
                Shoot();
            }
            else
            {
                passiveAbility();
            }
            fireCountdown =  1 / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firepoint.position, Quaternion.identity);
        CrystalProjectile crystalProjectile = projectile.GetComponent<CrystalProjectile>();
        Vector3 target = Random.insideUnitCircle;
        target = new Vector3(target.x, 0, target.y);
        //Attempt to keep kinda random if in inner 2/5 of circle which is an estimate for where the tower is
        if(target.x < .4f && target.x > -.4f && target.z < .4f && target.z > -.4f)
        {
            if(target.x > 0)
            {
                target.x += .4f;
            }
            else
            {
                target.x -= .4f;
            }
            if (target.x > 0)
            {
                target.z += .4f;
            }
            else
            {
                target.z -= .4f;
            }
        }
        target *= range;
        target += transform.position;
        if (crystalProjectile != null)
        {
            crystalProjectile.Seek(target, moneyGain);
        }
    }

    void passiveAbility()
    {
        resourcePS.Play();
        resourceManager.addResource(moneyGain);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void Upgrade(int op)
    {
        if (op == 0)
        {
            GameObject inst = (GameObject)Instantiate(nextLevelOpa, transform.position, transform.rotation);
            ITower newFactory = inst.GetComponent<ITower>();
            if (status == 3) { newFactory.setPassiveChosen("Passive"); }
            newFactory.Enable();
            newFactory.updatePrice(price);
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
    public int GetUpgradeCost() { return upgradeCostA; }
    public int GetStatus() { return status; }
    public int GetPrice() { return price; }
    public void updatePrice(int increase) { price += increase; }
    public float GetRange() { return range; }
    public Vector3 GetPosition() { return transform.position; }
    public TowerType getTowerType() { return towerT; }
    public TowerStats GetStats() { return new TowerStats(towerT, status, range, fireRate, moneyGain, price, upgradeCostA, upgradeCostB, nextLevelOpa, nextLevelOpb, 1, false, "", "", duration);} // always 1 path
    public GameObject GetGameObject() { return gameObject; }
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
