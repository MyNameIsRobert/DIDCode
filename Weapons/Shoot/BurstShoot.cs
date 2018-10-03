using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))] 
[RequireComponent(typeof(Animator))]
public class BurstShoot : Shoot {

    IEnumerator Burst()
    {
        int count = 0;
        int tempBulletsPerBurst;
        if (bulletsPerBurst > currentMagazine)
        {
            tempBulletsPerBurst = (int)currentMagazine;
        }
        else
            tempBulletsPerBurst = bulletsPerBurst;
        burstRoutinePlaying = true;
        while(count < tempBulletsPerBurst)
        {

                burstRateTimer -= Time.deltaTime;
                count++;
                burstRateTimer = burstFireRate;
                currentMagazine -= 1;
                RaycastHit hit; //Used to return hit data from when one of the rays hits a collider
            StartRecoil();
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
                    tempProjectile.GetComponent<Rigidbody>().AddForce(pellets[i].direction * projectileForce, ForceMode.Impulse);
                    //tempProjectile.GetComponent<Rigidbody>().mass =  .GetProjectileWeight();
                    Projectile proj = tempProjectile.GetComponent<Projectile>();
                    proj.damage = damage;
                    proj.critHitDamage = critMultiplier;
                    proj.playerProperties = playerProperties;
                }
                #endregion

            }

            yield return new WaitForSeconds(burstFireRate);
            
        }
        burstRoutinePlaying = false;
        yield return null;
    }

    #region Variables
    [Header("Burst Properties")]
    #region Burst Variables
    [SerializeField]
    [Tooltip("How many times to fire for each burst")]
    public int bulletsPerBurst;

    [SerializeField]
    [Tooltip("Time between bullets in a burst. Not the same as fireRate. Defaults to .1")]
    public float burstFireRate = .1f;

    
    float burstRateTimer;
#endregion

    #region Components


    HealthSinglePlayer health;




    WeaponSwitch weaponSwitch;

   


    #endregion

    #region Projectile Variables
    [Space(10f)]
    [Tooltip("Does the gun shoot projectiles or use hitscan?")]
    [SerializeField]
    bool isProjectile = false;

    [SerializeField]
    GameObject projectileToBeShot;

    [SerializeField]
    [Tooltip("Where the projectile will spawn from")]
    Transform projectileSpawn;


    #endregion

    #region Shooting Variables

    [SerializeField]
    int numberOfPellets = 1;
    



    [HideInInspector]
    public float reloadTimer;

    bool burstRoutinePlaying = false;
    

    #endregion

    #region Angle Variables
   


    float aimingTimer;

    #endregion



    private void Awake()
    {
        shootType = ShootType.BurstShoot;
    }

    [HideInInspector]
    public bool asSoonAsMagEmpty = false;
    [HideInInspector]
    public bool whenFiringEmptyMag = false;
    [HideInInspector]
    public bool whenButtonPressed = false;

    [HideInInspector]
    public bool canSwitchWeapon;

    #endregion

    // Use this for initialization
    public override void Start () {
        base.Start();
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

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if(Time.timeScale == 0 || isPickup || !weaponEnabled)
        {
            return;
        }

        if (currentMagazine > magazineSize)
        {
            currentMagazine = magazineSize;
        }
       

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

        #region Setting maxSpreadAngle
        maxSpreadAngle = defaultAngle;
        maxSpreadAngle *= maxAngleMultiplier;
        #endregion

        #region Shooting
        if(canFireSemiAuto && !burstRoutinePlaying)                
            StartCoroutine("Burst");
        #endregion

        if(Input.GetKeyDown(control.fire) && magazineEmpty)
            if (whenFiringEmptyMag)
                Reload();
        


        #region If Player Presses Reload Button
        if (reloadTimer <= 0)
        {
            if (Input.GetKeyDown(control.reload) && currentMagazine != magazineSize)
                Reload();
        }
        #endregion

        #region Timers  
        reloadTimer -= Time.deltaTime;
        aimingTimer -= Time.deltaTime;
        #endregion

        #region Setting Magazine Empty
        magazineEmpty = (currentMagazine < 1);
        #endregion

        if (magazineEmpty && asSoonAsMagEmpty)
            Reload();

        #region Updating Update Ammo and Weapon Switch
        updateAmmo.ChangeAmountObject(ammoAmount, currentMagazine);
        weaponSwitch.SetCanSwitchWeapon(canSwitchWeapon);
        #endregion

    }

    public void IncreaseBurstAmount(int amountToIncrease)
    {
        float tempTime = bulletsPerBurst * burstFireRate;
        Debug.Log("increased burst amount");
        bulletsPerBurst += amountToIncrease;
        burstFireRate = tempTime / bulletsPerBurst;
        
    }
    public void IncreaseBurstAmount(float amountToIncrease)
    {
        float tempTime = bulletsPerBurst * burstFireRate;
        Debug.Log("increased burst amount");
        bulletsPerBurst += (int)amountToIncrease;
        burstFireRate = tempTime / bulletsPerBurst;
    }
    public override void CopyFrom(Weapon weapon)
    {
        base.CopyFrom(weapon);
        if (weapon.weaponType == WeaponType.Gun)
        {
            Shoot tempShoot = (Shoot)weapon;
            try
            {
                BurstShoot tempBurst = (BurstShoot)tempShoot;
                burstFireRate = tempBurst.burstFireRate;
                bulletsPerBurst = tempBurst.bulletsPerBurst;
                isProjectile = tempBurst.isProjectile;
                projectileForce = tempBurst.projectileForce;
                projectileToBeShot = tempBurst.projectileToBeShot;
                numberOfPellets = tempBurst.numberOfPellets;
            }
            catch(System.InvalidCastException)
            {
                Debug.LogError("Weapon passed to CopyFrom is not the same as the weapon copying! Could not pass derived class variables");
            }
        }

    }
    
}

 


