using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = .1f;
    public bool shoot;
    public int attackDamage;
    private Transform target;
    private int wavepointIndex = 0;
    private Vector3 future_target_position;
    public float distancedMoved = 0f;
    public int path = 1;

    public ParticleSystem slowEffect;
    public ParticleSystem fastEffect;

    private GameObject gameManager;
    private TownHealth townHealth;
    private WaveSpawner waveSpawner;

    GameObject heathBar;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
        townHealth = gameManager.GetComponent<TownHealth>();
        waveSpawner = gameManager.GetComponent<WaveSpawner>();

        if (path == 1)
        {
            target = TrackWaypoints.points1[0];
        }
        else if (path == 2)
        {
            target = TrackWaypoints.points2[0];
        }
        else if (path == 3)
        {
            target = TrackWaypoints.points3[0];
        }
        else if (path == 4)
        {
            target = TrackWaypoints.points4[0];
        }
        else if (path == 5)
        {
            target = TrackWaypoints.points5[0];
        }
        else if (path == 6)
        {
            target = TrackWaypoints.points6[0];
        }

        heathBar = transform.Find("HealthBar").gameObject;
    }

    private void Update()
    {
        // archer script to stop and shoot
        if (wavepointIndex >= TrackWaypoints.points1.Length - 2 &&
            this.name == "PurpleArcher(Clone)"  ^
                this.name == "OrangeArcher(Clone)" ^
                this.name == "YellowArcher(Clone)" ^
                this.name == "BlueArcher(Clone)" ^
                this.name == "RedArcher(Clone)" ^
                this.name == "GreenArcher(Clone)")
        {
            Vector3 dir_temp = target.position - transform.position;
            Vector3 movement_temp = dir_temp.normalized * speed / 8 * Time.deltaTime;
            transform.Translate(movement_temp, Space.World);

            if (movement_temp != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(movement_temp * -1);
                heathBar.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            if (Vector3.Distance(transform.position, target.position) <= 0.5f)
            {
                GetNextWaypoint();
            }
        } else if (wavepointIndex >= TrackWaypoints.points1.Length - 1 &&
            this.name == "PurpleArcher(Clone)"  ^
                this.name == "OrangeArcher(Clone)" ^
                this.name == "YellowArcher(Clone)" ^
                this.name == "BlueArcher(Clone)" ^
                this.name == "RedArcher(Clone)" ^
                this.name == "GreenArcher(Clone)")
        {
            Vector3 dir_temp = target.position - transform.position;
            Vector3 movement_temp = dir_temp.normalized * speed / 8 * Time.deltaTime;
            transform.Translate(movement_temp, Space.World);

            if (movement_temp != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(movement_temp * -1);
                heathBar.transform.eulerAngles = new Vector3(0, 180, 0);
            }

            if (Vector3.Distance(transform.position, target.position) <= 0.5f)
            {
                townHealth.TakeDamage(attackDamage);
                waveSpawner.EnemyDestroy();
                Destroy(gameObject);
                return;
            }
        } 
        else 
        {
            Vector3 dir = target.position - transform.position;
            Vector3 movement = dir.normalized * speed * Time.deltaTime;
            transform.Translate(movement, Space.World);
            if (movement != Vector3.zero)
            {
                //transform.rotation = getRotation(movement * -1);
                transform.rotation = Quaternion.LookRotation(movement * -1);
                heathBar.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            distancedMoved += Mathf.Abs(movement.x) + Mathf.Abs(movement.z) + Mathf.Abs(movement.y);

            if (Vector3.Distance(transform.position, target.position) <= 0.5f)
            {
                GetNextWaypoint();
            }
        }
    }

    Quaternion getRotation(Vector3 movement)
    {
        float distToNext = Vector3.Distance(transform.position, target.position);
        Quaternion currRot = Quaternion.LookRotation(movement);

        if (distToNext <= 5f && wavepointIndex + 1 != TrackWaypoints.points1.Length - 1)
        {
            Vector3 nextDir = TrackWaypoints.points1[wavepointIndex + 1].position - TrackWaypoints.points1[wavepointIndex].position;
            Quaternion nextRot = Quaternion.LookRotation(nextDir.normalized * -1);
            return Quaternion.Lerp(nextRot, currRot, (distToNext / 5.0f));
        };
        return currRot;
    }

    // maybe make public later, because maybe there is some advanced troop that skips some track segments? idk
    private void GetNextWaypoint()
    {
        // once object reaches last waypoint
        if (wavepointIndex >= TrackWaypoints.points1.Length - 1)
        {
            waveSpawner.EnemyDestroy();
            townHealth.TakeDamage(attackDamage);
            Destroy(gameObject);
            return;
        }
        wavepointIndex++;
        if (path == 1)
        {
            target = TrackWaypoints.points1[wavepointIndex];
        }
        else if (path == 2)
        {
            target = TrackWaypoints.points2[wavepointIndex];
        } 
        else if (path == 3)
        {
            target = TrackWaypoints.points3[wavepointIndex];
        }
        else if (path == 4)
        {
            target = TrackWaypoints.points4[wavepointIndex];
        } 
        else if (path == 5)
        {
            target = TrackWaypoints.points5[wavepointIndex];
        } 
        else if (path == 6)
        {
            target = TrackWaypoints.points6[wavepointIndex];
        } 
    }

    public float getDistanceMoved()
    {
        return distancedMoved;
    }

    public void slow(float magnitude, float usetime)
    {
        float original = speed;
        speed /= magnitude;
        StartCoroutine(waitUseTimeSlow(usetime, original));
    }

    public void fast(float magnitude, float usetime)
    {
        float original = speed;
        speed *= magnitude;
        StartCoroutine(waitUseTimeFast(usetime, original));
    }

    IEnumerator waitUseTimeFast(float usetime, float original)
    {
        fastEffect.Play();
        yield return new WaitForSeconds(usetime);
        speed = original;
        fastEffect.Stop();
    }


    IEnumerator waitUseTimeSlow(float usetime, float original)
    {
        slowEffect.Play();
        yield return new WaitForSeconds(usetime);
        speed = original;
        slowEffect.Stop();
    }

    public void setSpeed(float _speed) {
        speed *=_speed; }
}
