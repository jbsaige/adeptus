﻿using UnityEngine;
using System.Collections;

public class WeaponOneSounds
{
    public AudioClip audio_hit_1, audio_hit_2;
}
public class WeaponOne : MonoBehaviour 
{
    // ranged weapons are actually fire button one weapons and could include melee weapons; so instead of "Gun", make a "WeaponOne" class for weapon one/button one.
    public enum WeaponOneType { Semi, Burst, Auto, Rock, KrakenWave, Thunderbolt, FireSpit, BehemothRoar, Sing, LightningBolt, FireBurst, JuggerCharge, Drain, Gaze, FireBreath, PoisonGas, AdeptFire, AdeptWater, AdeptEarth, AdeptAir, DragonAttackOne, DragonAttackTwo, DragonAttackThree, DragonAttackFour };
    public WeaponOneType weaponOneType;
    public float weaponID;
    public float rpm;
    public float rangedAttackSpeed;
    public float damage = 1;
    public LayerMask collisionMask;
    public WeaponOneSounds audioClips;

    public Transform projectileSpawn;
    private LineRenderer tracerRound;
   

    private float secondsBetweenShots;
    private float nextPossibleShootTime;

    // New Weapon system based on breadcrumb and mixed with this:
    public Rigidbody rangedProjectilePrefab;
    public GameObject specialPrefab;

    // Attack rates
    private float rangedAttackNext = 0.0f;
    private float rangedAttackRate = 10000.0f;
    private float meleeAttackNext = 0.0f;
    private float meleeAttackRate = 1.0f;
    private AudioSource audioSource;
   

    void Start()
    {
        

        secondsBetweenShots = 1000/rpm;
        if (GetComponent<LineRenderer>())
        {
            tracerRound = GetComponent<LineRenderer>();
        }
    }


    // old shoot method
    public void Shoot()
    {
        if (CanShoot())
        {
            Ray ray = new Ray(projectileSpawn.position, projectileSpawn.forward);
            RaycastHit hit;

            float shotDistance = 20;

            if (Physics.Raycast(ray, out hit, shotDistance, collisionMask))
            {
                shotDistance = hit.distance;

                if(hit.collider.GetComponent<Entity>())
                {
                    Debug.Log("Entity is taking damage: " + damage.ToString());
                    hit.collider.GetComponent<Entity>().TakeDamage(damage);
                }
            }

            //Debug.DrawRay(ray.origin, ray.direction * shotDistance, Color.red, 1);

            nextPossibleShootTime = Time.time + secondsBetweenShots;

            GetComponent<AudioSource>().Play();

            if (tracerRound)
            {
                StartCoroutine("RenderTracerRound", ray.direction * shotDistance);
            }
        }
    }

    public void ShootContinuous()
    {
        if (weaponOneType == WeaponOneType.Auto)
        {
            Shoot();
        }
    }

    private bool CanShoot()
    {
        bool canShoot = true;

        if (Time.time < nextPossibleShootTime)
        {
            canShoot = false;
        }

        return canShoot;
    }

    public void RangedAttack()
    {
        //if (Time.time > rangedAttackNext)
        if(CanShoot())
        {
            //rangedAttackNext = Time.time + rangedAttackRate;
            //rangedAttackNext = Time.time + secondsBetweenShots;
            nextPossibleShootTime = Time.time + secondsBetweenShots;
            Rigidbody rangedAttackForm = Instantiate(rangedProjectilePrefab, transform.position + transform.forward + transform.up, transform.rotation) as Rigidbody;
            rangedAttackForm.AddForce(transform.forward * rangedAttackSpeed);
            audioSource.clip = audioClips.audio_hit_1;
        }
    }

    public void ContinuousAttack()
    {
        if (weaponOneType == WeaponOneType.Sing || weaponOneType == WeaponOneType.Drain)
        //if (weaponOneType == WeaponOneType.Sing)
        {
            RangedAttack();
        }
    }

    IEnumerator RenderTracerRound(Vector3 hitLocation)
    {
        tracerRound.enabled = true;
        tracerRound.SetPosition(0, projectileSpawn.position);
        tracerRound.SetPosition(1, projectileSpawn.position + hitLocation);
        yield return null;
        tracerRound.enabled = false;
    }

}
