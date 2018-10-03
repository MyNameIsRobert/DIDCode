using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Description
/* Shoots after charging for a certain amount of seconds. Has ability to increase projectile speed and damage the longer it is held down
 * HITSCAN or PROJECTILE
 * Written By: Robert Clark
 * 6/26/17
 *
 * 
 */
#endregion
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class ChargeShoot: Shoot {

    #region Variable Declaration
    [Header("Charge Shoot")]
    //The health script of any enemy that is hit by a bullet
    HealthSinglePlayer health;

    [Tooltip("The sound the weapon is making when charging up")]
    [SerializeField]
    public AudioClip chargeSound;


    [SerializeField]
    [Tooltip("Whether or not the shotgun fires a hitscan or a projectile. Takes projectile from WeaponProperties")]
    private bool isProjectile = false;

    public GameObject projectileToBeShot;

    public float projectileSpeed;

    [SerializeField]
    float appliedProjectileSpeed;

    [SerializeField]
    [Tooltip("How long, in seconds, it takes to fully charge the weapon")]
    public float timeToFire;

    [SerializeField]
    float timeToFireTimer;

    [SerializeField]
    [Tooltip("When this is set to true, it allows the weapon to fire/release before it is fully charged. If it is false, the weapon will not fire until it is fully charge")]
    public bool fireEarly;

    [SerializeField]
    [Tooltip("How much the damage is ramped up, per second, while the shot is being charged")]
    public float rampUpDamageAmount;

    [SerializeField]
    [Tooltip("How much the fire speed is ramped up, per second, while the shot is being charged. Only applies to projectile weapons.")]
    public float rampUpSpeedAmount;

    [SerializeField]
    [Tooltip("If the gun is projectile type, this is where the projectile will spawn from. Put at the end of the gun.")]
    Transform projectileSpawn;

    [SerializeField]
    float appliedDamage;

    public int numberOfPellets;    

    [HideInInspector]
    public float reloadTimer;

    //Used to delay weapon switching until the aimin animation is finished playing
    float aimingTimer;

    [HideInInspector]
    public bool canSwitchWeapon;

    WeaponSwitch weaponSwitch;

    bool soundCounter = true;

    [HideInInspector]
    public bool asSoonAsMagEmpty = false;
    [HideInInspector]
    public bool whenFiringEmptyMag = false;
    [HideInInspector]
    public bool whenButtonPressed = false;

    #endregion
    // When the game starts, or when the gun is initialized onto the player

    #region Start Function
    public override void Start()
    {
        base.Start();
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


    // Update is called once per frame
    #region Update Function
    public override void Update()
    {
        base.Update();
        
        if(isPickup || Time.timeScale == 0)
        {
            return;
        }
        if (!weaponEnabled)
            return;

        anim.SetFloat("Charge Speed", 1 / timeToFire); 

        #region Setting Properties related To PlayerProperties
        if (playerProperties == null)
            Debug.Log("No PlayerProperties attached");
        else
        {
            reloadSpeed = playerProperties.handSpeed;

            if (currentMagazine > magazineSize)
            {
                currentMagazine = magazineSize;
            }
            if (aiming)
            {
                aimingTimer = .25f / reloadSpeed;
                playerProperties.SetAiming(true);
            }
            else
            {
                playerProperties.SetAiming(false);
            }
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
        #endregion

        #region Shooting
        #region Valdating the player meets the shoot requirements
        if (canFireSemiAuto || timeToFireTimer > 0) //Player presses and holds the shoot button, or they have already held the shoot button
        {

            if (timeToFireTimer >= timeToFire)
            {
                if (Input.GetKeyUp(control.fire))//Player releases the shoot button
                {
                    StartRecoil();
                    currentMagazine -= 1f;//Using 1 ammo
                    lastShot = Time.time; //Setting lastShot equal to current time
                    timeToFireTimer = 0;
                    RaycastHit hit; //Used to return hit data from when one of the rays hits a collider
                    soundCounter = true;
                    anim.SetTrigger("Fire");
                    anim.SetTrigger("Shoot");
                    anim.SetBool("Charging", false);
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

                                Destroy(tempParticle, .5f);
                                #endregion

                                hitSomething = true;
                                if (hit.collider.CompareTag("Enemy Body") || hit.collider.CompareTag("Enemy"))
                                {
                                    #region If ray hits the body
                                    if (hit.collider.transform.parent.GetComponent<HealthSinglePlayer>())//Making sure the collider has a health object attached
                                    {
                                        isBodyShot = true;
                                        health = hit.collider.transform.parent.GetComponent<HealthSinglePlayer>();
                                        health.TakeDamage(appliedDamage, !isSilenced);
                                        damageCounter += appliedDamage;
                                    }
                                    else if (hit.collider.transform.root.GetComponent<HealthSinglePlayer>())//Making sure the collider has a health object attached
                                    {
                                        isBodyShot = true;
                                        health = hit.collider.transform.root.GetComponent<HealthSinglePlayer>();
                                        health.TakeDamage(appliedDamage, !isSilenced);
                                        damageCounter += appliedDamage;
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
                                        health.TakeDamage(appliedDamage * critMultiplier, !isSilenced);
                                        damageCounter += appliedDamage * critMultiplier;
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
                            tempProjectile.GetComponent<Rigidbody>().AddForce(pellets[i].direction * appliedProjectileSpeed, ForceMode.Impulse);
                            tempProjectile.SendMessage("SetDamage", appliedDamage);
                            tempProjectile.GetComponent<Projectile>().damage = appliedDamage;
                            tempProjectile.GetComponent<Projectile>().playerProperties = playerProperties;
                            tempProjectile.GetComponent<Projectile>().critHitDamage = critMultiplier;
                            if (tempProjectile.GetComponent<Projectile>().projectileType == Projectile.Type.Explosive)
                            {
                                ExplosiveProjectile tempExplosive = (ExplosiveProjectile)tempProjectile.GetComponent<Projectile>();
                                tempExplosive.blastRadius = 10;
                            }

                        }
                        #endregion

                    }
                }
            }
            else
            {
                timeToFireTimer += Time.deltaTime;
                appliedDamage += rampUpDamageAmount * Time.deltaTime;
                appliedProjectileSpeed += rampUpSpeedAmount * Time.deltaTime;
                if (Input.GetKeyUp(control.fire))
                {
                    aud.Stop();
                    Debug.Log("Released button");
                    if (fireEarly)
                    {
                        StartCoroutine("Recoil");
                        currentMagazine -= 1f;//Using 1 ammo
                        lastShot = Time.time; //Setting lastShot equal to current time
                        RaycastHit hit; //Used to return hit data from when one of the rays hits a collider
                        soundCounter = true;
                        anim.SetTrigger("Fire");
                        anim.SetTrigger("Shoot");
                        anim.SetBool("Charging", false);
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
                                    if (hit.collider.CompareTag("Enemy Body") || hit.collider.CompareTag("Enemy"))
                                    {
                                        #region If ray hits the body
                                        if (hit.collider.transform.parent.GetComponent<HealthSinglePlayer>())//Making sure the collider has a health object attached
                                        {
                                            isBodyShot = true;
                                            health = hit.collider.transform.parent.GetComponent<HealthSinglePlayer>();
                                            health.TakeDamage(appliedDamage);
                                            damageCounter += appliedDamage;
                                        }
                                        else if (hit.collider.transform.root.GetComponent<HealthSinglePlayer>())//Making sure the collider has a health object attached
                                        {
                                            isBodyShot = true;
                                            health = hit.collider.transform.root.GetComponent<HealthSinglePlayer>();
                                            health.TakeDamage(appliedDamage);
                                            damageCounter += appliedDamage;
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
                                            health.TakeDamage(appliedDamage * critMultiplier);
                                            damageCounter += appliedDamage * critMultiplier;
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
                                tempProjectile.GetComponent<Rigidbody>().AddForce(pellets[i].direction * appliedProjectileSpeed, ForceMode.Impulse);
                                tempProjectile.SendMessage("SetDamage", appliedDamage);
                                tempProjectile.SendMessage("SetBlastRadius", 10);
                                tempProjectile.SendMessage("SetPlayerProperties", playerProperties);
                            }
                            #endregion

                        }
                    }
                    else
                    {
                        anim.SetBool("Charging", false);
                    }
                    timeToFireTimer = 0;
                }
                Debug.Log("Holding down while less than charge time");
                if (!aud.isPlaying)
                {
                    if (soundCounter)
                    {
                        soundCounter = false;
                        aud.PlayOneShot(chargeSound);
                    }
                }
                anim.SetBool("Charging", true);
            }
        }
        else
        {
            Debug.Log("Player is not holding shoot button or the timer is 0");
            appliedDamage = damage;
            appliedProjectileSpeed = projectileSpeed;
            timeToFireTimer = 0;
            anim.SetBool("Charging", false);
        }
        #endregion

        #region If Player Presses Reload Button
        if (!coroutinePlaying && currentMagazine != magazineSize && playerAmmo.GetAmount(ammoType) > 0)
        {
            if (Input.GetKeyDown(control.reload))
                Reload(false);
        }
        #endregion

        #region Timers  
        reloadTimer -= Time.deltaTime;
        aimingTimer -= Time.deltaTime;
        #endregion

        #region Setting Magazine Empty
        magazineEmpty = currentMagazine < 1;
        #endregion

        #region Calling reload If Magazine empty is true and asSoonAsEmpty is true
        if (magazineEmpty && asSoonAsMagEmpty)
            Reload(false);
#endregion

        #region Updating Update Ammo and Weapon Switch
        updateAmmo.ChangeAmountObject(ammoAmount, currentMagazine);
        weaponSwitch.SetCanSwitchWeapon(canSwitchWeapon);
        #endregion

       

    }
    #endregion

#endregion
    public override void CopyFrom(Weapon weapon)
    {
        base.CopyFrom(weapon);
        if (weapon.weaponType == WeaponType.Gun)
        {
            Shoot tempShoot = (Shoot)weapon;
            try
            {
                ChargeShoot shoot = (ChargeShoot)tempShoot;
                chargeSound = shoot.chargeSound;
                isProjectile = shoot.isProjectile;
                projectileToBeShot = shoot.projectileToBeShot;
                numberOfPellets = shoot.numberOfPellets;
                timeToFire = shoot.timeToFire;
                rampUpDamageAmount = shoot.rampUpDamageAmount;
                rampUpSpeedAmount = shoot.rampUpSpeedAmount;
                fireEarly = shoot.fireEarly;
                projectileSpeed = shoot.projectileSpeed;
            }
            catch (System.InvalidCastException)
            {
                Debug.LogError("Weapon passed to CopyFrom is not the same as the weapon copying! Could not pass derived class variables");
            }
        }

    }
}
