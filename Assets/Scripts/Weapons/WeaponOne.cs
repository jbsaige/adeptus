using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class WeaponOne : MonoBehaviour 
{
    // ranged weapons are actually fire button one weapons and could include melee weapons; so instead of "Gun", make a "WeaponOne" class for weapon one/button one.
    public enum WeaponOneType { Semi, Burst, Auto, Rock, KrakenWave, Thunderbolt, FireSpit, BehemothPunch, Sing, LightningBolt, FireBurst, JuggerCharge, Drain, Gaze, FireBreath, PoisonGas, AdeptFire, AdeptWater, AdeptEarth, AdeptAir, DragonAttackOne, DragonAttackTwo, DragonAttackThree, DragonAttackFour };
    public WeaponOneType weaponOneType;
    public float weaponID;
    public float rpm;
    public float damage = 1;
    public LayerMask collisionMask;

    public Transform projectileSpawn;
    private LineRenderer tracerRound;

    private float secondsBetweenShots;
    private float nextPossibleShootTime;

    void Start()
    {
        secondsBetweenShots = 120/rpm;
        if (GetComponent<LineRenderer>())
        {
            tracerRound = GetComponent<LineRenderer>();
        }
    }

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
                    hit.collider.GetComponent<Entity>().TakeDamage(damage);
                }
            }

            //Debug.DrawRay(ray.origin, ray.direction * shotDistance, Color.red, 1);

            nextPossibleShootTime = Time.time + secondsBetweenShots;

            audio.Play();

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

    IEnumerator RenderTracerRound(Vector3 hitLocation)
    {
        tracerRound.enabled = true;
        tracerRound.SetPosition(0, projectileSpawn.position);
        tracerRound.SetPosition(1, projectileSpawn.position + hitLocation);
        yield return null;
        tracerRound.enabled = false;
    }

}
