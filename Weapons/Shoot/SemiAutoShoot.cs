using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class SemiAutoShoot : Shoot {

    #region Variable Declaration
    [SerializeField]
    [Tooltip("Whether or not the gun fires a projectile or hitscan")]
    private bool isProjectile = false;

    public GameObject projectileToBeShot;

    HealthSinglePlayer health;

    
    [SerializeField]
    [Tooltip("Where the projectile will spawn")]
    Transform projectileSpawn;

    public int numberOfPellets = 1;

    [HideInInspector]
    public float reloadTimer;

    float aimingTimer;

    [HideInInspector]
    public bool canSwitchWeapon;

    WeaponSwitch weaponSwitch;


    float appliedReloadSpeed;
    float appliedADSSpeed;

    [HideInInspector]
    public bool asSoonAsMagEmpty = false;
    [HideInInspector]
    public bool whenFiringEmptyMag = false;
    [HideInInspector]
    public bool whenButtonPressed = false;

    #endregion
    // Use this for initialization
    #region Start
    public override void Start()
    {
        base.Start();
        //currentGunMagazine = maxMagazineSize;
        if (isPickup)
            return;
        weaponSwitch = player.GetComponent<WeaponSwitch>();
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
    }
    #endregion

    private void Awake()
    {
        shootType = ShootType.SemiAutoShoot;
    }
    // Update is called once per frame
    public override void Update () {
        base.Update();
        if (isPickup)
            return;
        reloadSpeed = playerProperties.handSpeed;
        appliedReloadSpeed = reloadSpeed * reloadSpeedModifier;
        appliedADSSpeed = reloadSpeed * aimSpeed;

        if (currentMagazine > magazineSize)
        {
            currentMagazine = magazineSize;
        }

        #region Setting Animator Parameters
        anim.SetFloat("Reload Speed", appliedReloadSpeed);
        anim.SetFloat("ADS Speed", appliedADSSpeed);
        #endregion

        #region Setting Aiming
        if (aiming)
        {
            aimingTimer = .25f / reloadSpeed;
            playerProperties.SetAiming(true);
        }
        else
        {
            playerProperties.SetAiming(false);
        }
        #endregion

        #region Setting canSwitchWeapon

        if (reloadTimer > 0)
        {
            canSwitchWeapon = false;
        }
        else
        {
            if (aimingTimer > 0)
                canSwitchWeapon = false;
            else
                canSwitchWeapon = true;
        }
        #endregion

        #region Setting maxAngleMultiplier and FOV
        if (reloadTimer <= 0)
        {
            if (Input.GetKey(control.aim))
            {
                maxAngleMultiplier = 1 / aimingDivider;
                aiming = true;
                if (cam.fieldOfView > (30 / zoomAmount))
                {
                    cam.fieldOfView -= 250 * Time.deltaTime;
                }
            }
            else
            {
                maxAngleMultiplier = 1;
                aiming = false;
                if (cam.fieldOfView < 60)
                    cam.fieldOfView += 250f * Time.deltaTime;
            }
        }
        #endregion

        #region Setting gunAnim bool
        anim.SetBool("Aiming", aiming);
        #endregion

        #region Setting maxPelletSpreadAngle
        maxSpreadAngle = defaultAngle;
        maxSpreadAngle *= maxAngleMultiplier;
        maxSpreadAngle *= accuracyModifier;
        #endregion

        #region Shooting


        if (canFireSemiAuto)
        {
            lastShot = Time.time;
            //The player can shoot
            StartRecoil();
            currentMagazine -= 1f;//Using 1 ammo
            RaycastHit hit; //Used to return hit data from when one of the rays hits a collider

            anim.SetTrigger("Fire");
            anim.SetTrigger("Shoot");

            Ray[] pellets = new Ray[numberOfPellets];//Creating an array of pellet rays that will be randomly fired in a specifically sized cone

            if (!isProjectile)
            {
                for (int i = 0; i < numberOfPellets; i++) //Filling the array of Rays
                {
                    #region Creating a random Ray with an unchanging starting point within a cone
                    Vector3 fireDirection = cam.transform.forward;
                    Quaternion fireRotation = Quaternion.LookRotation(fireDirection);
                    Quaternion randomRotation = Random.rotation;

                    fireRotation = Quaternion.RotateTowards(fireRotation, randomRotation, Random.Range(0f, maxSpreadAngle));
                    #endregion
                    pellets[i] = new Ray(cam.transform.position, fireRotation * Vector3.forward);
                }



                aud.PlayOneShot(attackSound);
                float damageCounter = 0;

                bool isHeadShot = false, isBodyShot = false, hitSomething = false;
                #region Seeing if rays hit a collider and doing stuff with it
                for (int i = 0; i < numberOfPellets; i++)
                {
                    if (Physics.Raycast(pellets[i], out hit))
                    {
                        #region Instantiating Hit Particle facing player
                        var direction = cam.transform.position - hit.point;
                        var tempParticle = Instantiate(hitParticle, hit.point, Quaternion.LookRotation(direction)); //Spawns a hit particle where the ray intersects with a collider
                        tempParticle.GetComponent<SpawnBulletHoleImage>().SetHitRotation(hit.normal);
                        tempParticle.GetComponent<SpawnBulletHoleImage>().SetHit(hit);
                        Destroy(tempParticle, .5f);
                        #endregion

                        hitSomething = true;
                        if (hit.collider.CompareTag("Enemy Body") /*|| hit.collider.CompareTag("Enemy")*/)
                        {
                            #region If ray hits the body
                            if (hit.collider.transform.parent.GetComponent<HealthSinglePlayer>())//Making sure the collider has a health object attached
                            {
                                isBodyShot = true;
                                health = hit.collider.transform.parent.GetComponent<HealthSinglePlayer>();
                                health.TakeDamage(damage, !isSilenced);
                                damageCounter += damage;
                            }
                            else if (hit.collider.transform.root.GetComponent<HealthSinglePlayer>())//Making sure the collider has a health object attached
                            {
                                isBodyShot = true;
                                health = hit.collider.transform.root.GetComponent<HealthSinglePlayer>();
                                health.TakeDamage(damage, !isSilenced);
                                damageCounter += damage;
                            }
                            else
                            {
                                Debug.Log("No HealthSinglePlayer component is in the parent transform!!");//Debug
                            }
                            #endregion
                        }
                        else if (hit.collider.CompareTag("Enemy Head"))
                        {
                            #region If ray hits the head
                            if (hit.collider.transform.parent.GetComponent<HealthSinglePlayer>())
                            {
                                isHeadShot = true;
                                health = hit.collider.transform.parent.GetComponent<HealthSinglePlayer>();
                                health.TakeDamage(damage * critMultiplier, !isSilenced);
                                damageCounter += damage * critMultiplier;
                            }
                            else if (hit.collider.transform.root.GetComponent<HealthSinglePlayer>())//Making sure the collider has a health object attached
                            {
                                isHeadShot = true;
                                health = hit.collider.transform.root.GetComponent<HealthSinglePlayer>();
                                health.TakeDamage(damage * critMultiplier, !isSilenced);
                                damageCounter += damage * critMultiplier;
                            }
                            else
                            {
                                Debug.Log("No HealthSinglePlayer component is in the parent transform!!");//Debug
                            }
                            #endregion
                        }

                    }
                }

                if (isBodyShot && hitSomething)
                {
                    playerProperties.ShowHitmarker(false, (int)damageCounter);
                }
                if (isHeadShot && hitSomething)
                {
                    playerProperties.ShowHitmarker(true, (int)damageCounter);
                }
                #endregion
            }
            else
            {
                for (int i = 0; i < numberOfPellets; i++) //Filling the array of Rays
                {
                    #region Creating a random Ray with an unchanging starting point within a cone
                    Vector3 fireDirection = projectileSpawn.transform.forward;
                    Quaternion fireRotation = Quaternion.LookRotation(fireDirection);
                    Quaternion randomRotation = Random.rotation;

                    fireRotation = Quaternion.RotateTowards(fireRotation, randomRotation, Random.Range(0f, maxSpreadAngle));
                    #endregion
                    pellets[i] = new Ray(projectileSpawn.transform.position, fireRotation * Vector3.forward);
                }

                aud.PlayOneShot(attackSound);

                #region Spawning in Projectiles and Shooting
                for (int i = 0; i < numberOfPellets; i++)
                {
                    GameObject tempProjectile = Instantiate(projectileToBeShot, pellets[i].origin, Quaternion.LookRotation(pellets[i].direction));
                    tempProjectile.GetComponent<Rigidbody>().AddForce(pellets[i].direction * 100);
                    tempProjectile.SendMessage("SetDamage", damage);

                    tempProjectile.SendMessage("SetPlayerProperties", playerProperties);
                }
                #endregion

            }

        }


        #endregion

        #region If Player Presses Reload Button
        if (reloadTimer <= 0)
        {
            if (Input.GetKeyDown(control.reload))
                Reload();
        }
        #endregion

        #region Timers  
        reloadTimer -= Time.deltaTime;
        aimingTimer -= Time.deltaTime;
        #endregion

        #region Setting Magazine Empty
        magazineEmpty = currentMagazine < 1;
        #endregion

        #region Updating Update Ammo and Weapon Switch
        updateAmmo.ChangeAmountObject(ammoAmount, currentMagazine);
        weaponSwitch.SetCanSwitchWeapon(canSwitchWeapon);
        #endregion

    }

    public override void CopyFrom(Weapon weapon)
    {
        base.CopyFrom(weapon);
        if (weapon.weaponType == WeaponType.Gun)
        {
            Shoot tempShoot = (Shoot)weapon;
            try
            {
                SemiAutoShoot shoot = (SemiAutoShoot)tempShoot;
                isProjectile = shoot.isProjectile;
                projectileToBeShot = shoot.projectileToBeShot;
                numberOfPellets = shoot.numberOfPellets;
            }
            catch (System.InvalidCastException)
            {
                Debug.LogError("Weapon passed to CopyFrom is not the same as the weapon copying! Could not pass derived class variables");
            }
        }

    }
}
