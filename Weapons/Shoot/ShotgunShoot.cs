using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Description
    /*This script shoots a gun in a shotgun style. Random weapon spread, multiple pellets per shot.
     * Each pellet will do it's own amount of damage.
     * HITSCAN or PROJECTILE
     * Written By: Robert Clark
     * 6/26/17
     *
     * 
     */
#endregion

public class ShotgunShoot : Shoot {

    #region Variable Declaration


    [Header("Shotgun Variables")]
    //The health script of any enemy that is hit by a bullet
    HealthSinglePlayer health;
    [SerializeField]
    bool semiAuto = false;

    [SerializeField]
    [Tooltip("Whether or not the shotgun fires a hitscan or a projectile. Takes projectile from WeaponProperties")]
    private bool isProjectile = false;
    [SerializeField]
    public GameObject projectileToBeShot;

    [SerializeField]
    [Tooltip("If the gun is projectile type, this is where the projectile will spawn from. Put at the end of the gun.")]
    Transform projectileSpawn;

    //The number of pellets per round shot from the gun"
    //Defaults to 20 pellets
    [SerializeField]
    public int numberOfPellets = 20;
    


    [HideInInspector]
    public float reloadTimer;

    //Used to delay weapon switching until the aimin animation is finished playing
    float aimingTimer;

    [HideInInspector]
    public bool canSwitchWeapon;



    WeaponSwitch weaponSwitch;


    #endregion
    // When the game starts, or when the gun is initialized onto the player


    #region Start Function
   public override void Start()
    {
        base.Start();
        if (isPickup)
            return;
        
        
        if (ammoAmount == null)
            ammoAmount = transform.Find("Ammo").GetComponent<Amount>();
        weaponSwitch = player.GetComponent<WeaponSwitch>();

    }
    #endregion


    // Update is called once per frame
    #region Update Function
    public override void Update()
    {
        if (isPickup)
            return;
        base.Update();
        if (Time.timeScale == 0)
            return;
        reloadSpeed = playerProperties.handSpeed;

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

        #region Shooting
        if ((semiAuto && canFireSemiAuto) || (!semiAuto && canFireAuto))
        {
            Debug.Log("Firing");
            StartRecoil();
            //The player can shoot
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
                        if (hit.collider.CompareTag("Enemy Body") || hit.collider.CompareTag("Enemy"))
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
                            else if (hit.collider.transform.root.GetComponent<HealthSinglePlayer>())
                            {
                                isHeadShot = true;
                                health = hit.collider.transform.root.GetComponent<HealthSinglePlayer>();
                                health.TakeDamage(damage * critMultiplier, !isSilenced);
                                damageCounter += damage * critMultiplier;
                            }
                            else
                            {
                                Debug.Log("No HealthSinglePlayer component attached!");
                            }
                            #endregion
                        }

                    }
                }

                if (isBodyShot && hitSomething)
                {
                    Debug.Log("Total Shotgun Damage Done: " + (int)damageCounter);
                    playerProperties.ShowHitmarker(false, (int)damageCounter);
                }
                if (isHeadShot && hitSomething)
                {
                    Debug.Log("Total Shotgun Damage Done: " + (int)damageCounter);
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
                    tempProjectile.GetComponent<Rigidbody>().AddForce(pellets[i].direction * projectileForce);
                    tempProjectile.SendMessage("SetDamage", damage);
                    tempProjectile.SendMessage("SetPlayerProperties", playerProperties);
                }
                #endregion

            } 
        }


                    
                
            
        
        #endregion

        #region If Player Presses Reload Button
        if (!coroutinePlaying)
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
    #endregion



    private void Awake()
    {
        shootType = ShootType.ShotgunShoot;
    }


    public override void CopyFrom(Weapon weapon)
    {
        base.CopyFrom(weapon);
        if (weapon.weaponType == WeaponType.Gun)
        {
            Shoot tempShoot = (Shoot)weapon;
            try
            {
                ShotgunShoot gunShoot = (ShotgunShoot)tempShoot;
                projectileToBeShot = gunShoot.projectileToBeShot;
                numberOfPellets = gunShoot.numberOfPellets;
                semiAuto = gunShoot.semiAuto;
            }
            catch (System.InvalidCastException)
            {
                Debug.LogError("Weapon passed to CopyFrom is not the same as the weapon copying! Could not pass derived class variables");
            }
        }

    }
}
