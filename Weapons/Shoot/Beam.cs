using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : Shoot
{

    #region Variables
    public float ammoUsePerSecond = 1;

    public float radiusOfBeam;

    public float distance;

    public float hangingDelay;

    public float lingeringDamagePerSecond;
    public float lingeringDamageTime;

    public GameObject particleToSpawn;
    public Transform particleSpawnPoint;

    bool asSoonAsMagEmpty = false;
    bool whenButtonPressed = false;
    bool whenFiringEmptyMag = false;

    WeaponSwitch weaponSwitch;

    HealthSinglePlayer health;
    float totalDamage;
    [SerializeField]
    float totalDamageHitmarkerResetTime = .2f;
    [SerializeField]
    float particleResetTime = .2f;
    bool hitmarkerCoroutineRunning = false;
    bool particleCoroutineRunning = false;
    bool isShooting = false;
    Quaternion particleRotation;
    #endregion
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        switch (howToReload)
        {
            case ReloadType.AsSoonAsMagazineIsEmpty:
                asSoonAsMagEmpty = true;
                break;
            case ReloadType.OnlyWhenButtonIsPressed:
                whenButtonPressed = true;
                break;
            case ReloadType.WhenTryingToFireAnEmptyMagazine:
                whenFiringEmptyMag = true;
                break;
        }
        weaponSwitch = player.GetComponent<WeaponSwitch>();
    }


    IEnumerator ShowDPSHitmarker()
    {
        bool quit = false;
        hitmarkerCoroutineRunning = true;
        while (!quit)
        {
            yield return new WaitForSeconds(totalDamageHitmarkerResetTime);
            if(totalDamage > 0)
            {
                playerProperties.ShowHitmarker(false, (int)totalDamage);
            }
            else
            {
                quit = true;
            }
            totalDamage = 0;
        }
        hitmarkerCoroutineRunning = false;
        yield return null;
    }
    IEnumerator SpawnParticle()
    {
        bool quit = false;
        particleCoroutineRunning = true;
        while (!quit)
        {
            yield return new WaitForSeconds(particleResetTime);
            if (isShooting)
            {
               GameObject go = Instantiate(particleToSpawn, particleSpawnPoint.position, particleRotation);
                Destroy(go, 3);
            }
            else
            {
                quit = true;
            }
        }
        particleCoroutineRunning = false;
        yield return null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Input.GetKey(control.fire))
        {
            if(currentMagazine > 0)
            {            
                if (!coroutinePlaying)
                {
                    if (!aud.isPlaying)
                    {
                        aud.PlayOneShot(attackSound);
                    }
                    particleRotation = particleSpawnPoint.transform.rotation;
                    currentMagazine -= (Time.deltaTime * ammoUsePerSecond);
                    anim.SetBool("Firing", true);

                    #region Beam Is a Ray
                    if(radiusOfBeam > 0)
                    {
                        RaycastHit hit;
                        if (Physics.SphereCast(cam.transform.position, radiusOfBeam, cam.transform.forward, out hit, distance))
                        {
                            RaycastHit particleHit;
                            Physics.Raycast(cam.transform.position, cam.transform.forward, out particleHit);

                            var direction = cam.transform.position - hit.point;
                            var tempParticle = Instantiate(hitParticle, hit.point, Quaternion.LookRotation(direction)); //Spawns a hit particle where the ray intersects with a collider

                            Destroy(tempParticle, .5f);

                            particleRotation = Quaternion.LookRotation(particleHit.point - particleSpawnPoint.transform.position);
                            Debug.Log("Capsule hit collider: " + hit.collider.name);
                            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Enemy Body") || hit.collider.CompareTag("Enemy Head"))
                            {
                                RaycastHit lineOfSight;
                                if(Physics.Raycast(cam.transform.position, hit.transform.position - cam.transform.position, out lineOfSight))
                                {
                                    if(lineOfSight.collider.GetInstanceID() == hit.collider.GetInstanceID())
                                    {
                                        if (hit.collider.transform.root.GetComponent<HealthSinglePlayer>())
                                        {
                                            health = hit.collider.transform.root.GetComponent<HealthSinglePlayer>();
                                            health.TakeDamage(damage * Time.deltaTime);
                                            if(lingeringDamagePerSecond > 0)
                                            {
                                                health.DamageOverTime(lingeringDamagePerSecond, lingeringDamageTime);
                                            }
                                            if (!hitmarkerCoroutineRunning)
                                            {
                                                StartCoroutine(ShowDPSHitmarker());
                                            }
                                            totalDamage += damage * Time.deltaTime;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region Beam is a Cylinder
                    else
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, distance))
                        {
                            var direction = cam.transform.position - hit.point;
                            var tempParticle = Instantiate(hitParticle, hit.point, Quaternion.LookRotation(direction)); //Spawns a hit particle where the ray intersects with a collider

                            Destroy(tempParticle, .5f);
                            particleRotation = Quaternion.LookRotation(hit.point - particleSpawnPoint.transform.position);
                            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Enemy Body") || hit.collider.CompareTag("Enemy Head"))
                            {
                                if (hit.collider.transform.root.GetComponent<HealthSinglePlayer>())
                                {
                                    health = hit.collider.transform.root.GetComponent<HealthSinglePlayer>();
                                    health.TakeDamage(damage * Time.deltaTime);
                                    if (lingeringDamagePerSecond > 0)
                                    {
                                        health.DamageOverTime(lingeringDamagePerSecond, lingeringDamageTime);
                                    }
                                    if (!hitmarkerCoroutineRunning)
                                    {
                                        StartCoroutine(ShowDPSHitmarker());
                                    }
                                    totalDamage += damage * Time.deltaTime;
                                }
                            }
                        }
                    } 
                    #endregion

                    isShooting = true;
                    if (!particleCoroutineRunning)
                    {
                        StartCoroutine(SpawnParticle());
                    }
                }
            }
            else
            {
                Reload();
            }
        }
        else
        {
            anim.SetBool("Firing", false);
            isShooting = false;
        }

        updateAmmo.ChangeAmountObject(ammoAmount, currentMagazine);

    }


}
