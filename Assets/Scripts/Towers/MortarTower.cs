using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTower : MonoBehaviour, ITower
{
    // Start is called before the first frame updateS
    public float range = 10f;
    public float fireRate = 1f;
    public float projDamage = 1f;
    private float fireCountdown = 0f;
    public float gravity = -30;
    public float time = 1f;
    public bool enable;
    private bool wave = false;

    public float splashRadius = 5f; // SET IN PREFAB

    public AudioClip boom;
    AudioSource shoot;

    public string enemyTag = "Enemy";
    //current level of tower NOTE: 4a and 4b are both 4 as this is for upgrading and both can not be upgraded
    public int status;
    public int price;
    public TowerType towerT;
    public int upgradeCostA;
    public int upgradeCostB;
    public GameObject nextLevelOpa;
    public GameObject nextLevelOpb;
    public GameObject RangeCircle;
    public GameObject RangeTarget;
    private GameObject gameManager;

    public Transform firepoint;
    public Transform target;
    public GameObject Cannonball;
    public ParticleSystem firerateboost;
    public ParticleSystem firerateslow;

    [Header("Level 4B Things")]
    public bool isLvl4b;
    public Transform firepoint2;
    public Transform firepoint3;

    [Header("Level 4A Things")]
    public bool isLvl4a;

    string passiveChosen = "";

    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        shoot = GetComponent<AudioSource>();
        Vector3 scale = new Vector3(range * 2, .001f, range * 2);
        RangeCircle.transform.localScale = Vector3.zero;
        RangeCircle.transform.localScale += scale;
    }

    void Update()
    {
        if (gameManager != null && !GameManager.instance.tutorialMode)
        {
            wave = gameManager.GetComponent<WaveSpawner>().getWaveStatus();
        }
        
        if (target == null)
        {
            return;
        }

        if (fireCountdown <= 0f && enable && !wave && !GameManager.instance.tutorialMode)
        {
            Shoot();
            fireCountdown = 1 / fireRate;
        }

        fireCountdown -= Time.deltaTime;

    }

    void Shoot()
    {
        if (isLvl4b)
        {
            StartCoroutine("Shoot4b");
        }
        else if (isLvl4a)
        {
            StartCoroutine("Shoot4a");
        }
        else
        {
            GameObject projectile = Instantiate(Cannonball, firepoint.position, Quaternion.identity) as GameObject;
            
            Rigidbody cannonBallR = projectile.GetComponent<Rigidbody>();
            cannonBallR.velocity = CalculateVelocity();

            CannonballHit cannonBall = projectile.GetComponent<CannonballHit>();

            if (cannonBall != null)
            {
                cannonBall.Seek(projDamage);
            }

            shoot.PlayOneShot(boom, 1f);
        }
    }

    IEnumerator Shoot4b()
    {
        shoot.PlayOneShot(boom, .5f);
        GameObject projectile1 = Instantiate(Cannonball, firepoint.position, Quaternion.identity) as GameObject;
        Rigidbody cannonBallR1 = projectile1.GetComponent<Rigidbody>();
        cannonBallR1.velocity = CalculateVelocity();
        CannonballHit cannonBall1 = projectile1.GetComponent<CannonballHit>();
        if (cannonBall1 != null)
        {
            cannonBall1.Seek(projDamage);
        }
        yield return new WaitForSeconds(.33f);
        shoot.PlayOneShot(boom, .5f);
        GameObject projectile2 = Instantiate(Cannonball, firepoint2.position, Quaternion.identity) as GameObject;
        Rigidbody cannonBallR2 = projectile2.GetComponent<Rigidbody>();
        cannonBallR2.velocity = CalculateVelocity();
        CannonballHit cannonBall2 = projectile2.GetComponent<CannonballHit>();
        if (cannonBall2 != null)
        {
            cannonBall2.Seek(projDamage);
        }
        yield return new WaitForSeconds(.33f);
        shoot.PlayOneShot(boom, .5f);
        GameObject projectile3 = Instantiate(Cannonball, firepoint3.position, Quaternion.identity) as GameObject;
        Rigidbody cannonBallR3 = projectile3.GetComponent<Rigidbody>();
        cannonBallR3.velocity = CalculateVelocity();
        CannonballHit cannonBall3 = projectile3.GetComponent<CannonballHit>();
        if (cannonBall3 != null)
        {
            cannonBall3.Seek(projDamage);
        }
    }

    IEnumerator Shoot4a()
    {
        GameObject projectile = Instantiate(Cannonball, firepoint.position, Quaternion.identity) as GameObject;
            
            Rigidbody cannonBallR = projectile.GetComponent<Rigidbody>();
            cannonBallR.velocity = CalculateVelocity();

            CannonballHit cannonBall = projectile.GetComponent<CannonballHit>();

            if (cannonBall != null)
            {
                cannonBall.Seek(projDamage);
            }

            shoot.PlayOneShot(boom, 1f);
            yield return new WaitForSeconds(1f);
    }

    Vector3 CalculateVelocity() {
        Vector3 distance = target.position - firepoint.position;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;
        
        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
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
            ITower newMortar = inst.GetComponent<ITower>();
            inst.GetComponent<MortarTower>().setTargetPosition(target.position);
            if (status == 3) { newMortar.setPassiveChosen("Large Bomb"); }
            newMortar.Enable();
            newMortar.updatePrice(price);
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
            ITower newMortar = inst.GetComponent<ITower>();
            inst.GetComponent<MortarTower>().setTargetPosition(target.position);
            newMortar.setPassiveChosen("Triple Shot");
            newMortar.Enable();
            newMortar.updatePrice(price);
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

    public void setTargetPosition(Vector3 pos)
    {
        foreach(Transform child in transform)
        {
            if (child.name == "Target")
            {
                child.position = pos;
            }
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
    public int GetUpgradeCost() {return upgradeCostA; }
    public int GetStatus() { return status; }
    public int GetPrice() { return price; }
    public void updatePrice(int increase) { price += increase; }
    public float GetRange() { return range; }
    public Vector3 GetPosition() { return transform.position; }
    public TowerType getTowerType() { return towerT; }
    public TowerStats GetStats()
    {
        if (isLvl4b)
        {
            return new TowerStats(towerT, status, range, fireRate * 3, projDamage, price, upgradeCostA, upgradeCostB, nextLevelOpa, nextLevelOpb, 1, false, "", "", splashRadius); 
        }
        else if (status == 3)
        {
            return new TowerStats(towerT, status, range, fireRate, projDamage, price, upgradeCostA, upgradeCostB, nextLevelOpa, nextLevelOpb, 1, true, "Large Bomb", "Triple Shot", splashRadius);
        }
        else { return new TowerStats(towerT, status, range, fireRate, projDamage, price, upgradeCostA, upgradeCostB, nextLevelOpa, nextLevelOpb, 1, false, "", "", splashRadius); }
    }
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
        RangeTarget.gameObject.SetActive(true);
        RangeCircle.gameObject.SetActive(true);
    }

    public void DisableRange()
    {
        RangeTarget.gameObject.SetActive(false);
        RangeCircle.gameObject.SetActive(false);
    }
    #endregion


}

