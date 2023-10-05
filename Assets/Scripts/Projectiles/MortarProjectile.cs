// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MortarProjectile : MonoBehaviour
// {
//     private Transform target;

//     //Speed of projectile
//     public float speed = 50f;

//     //The effect when a projectile hits object
//     public GameObject impactEffect;

//     private float strength;

//     public void Seek(Transform _target, float _strength)
//     {
//         target = _target;
//         strength = _strength;
//     }

//     // Update is called once per frame

//     // call this once and for every time*delta time until HItTarget, translate
//     void Update()
//     {
//         if (target == null)
//         {
//             Destroy(gameObject);
//             return;
//         }

//         EnemyMovement enemyposition = target.GetComponent<EnemyMovement>();

//         Vector3 dir_new = enemyposition.PredictNextPosition();
//         Vector3 dir = target.position - transform.position;
//         // Debug.Log("current: " + dir);
//         // Debug.Log("new: " + dir_new);
//         float distanceThisFrame = speed * Time.deltaTime;

//         //when projectile hits target
//         if(dir_new.magnitude <= distanceThisFrame)
//         {
//             HitTarget();
//             return;
//         }

//         transform.Translate(dir_new.normalized * distanceThisFrame, Space.World);

//     }

//     void HitTarget()
//     {
//         EnemyController archProjectile = target.GetComponent<EnemyController>();
//         archProjectile.Hit(strength);
//         GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
//         Destroy(effectIns, 2f); //Destroys particle system after 2 sec
//         Destroy(gameObject);
//     }
// }
