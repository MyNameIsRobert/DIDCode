using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof (AudioSource))]
[RequireComponent(typeof(Animator))]
public class ProjectileShoot : MonoBehaviour {

    #region Variables

    #region Components

    //The Animation controller attached to the gun
    public Animator gunAnim;
    //The PlayerProperties Script attached to the root player
    PlayerProperties playerPropterties;

    UpdateAmmo updateAmmo;

    WeaponSwitch weaponSwitch;

    Rigidbody projectileRigidBody;


    //The root player gameObject
    GameObject player;

    #endregion

    #region Projectile Properties

    GameObject projectilePrefab;
    
    [Tooltip("Where the projectile is fired from.")]
    public Transform projectileSpawnTransform;
        
    float projectileDamage;

    float shootSpeed = 100f;

    float projectileMass = .1f;

    [Tooltip("How big of an area the explosion effects.")]
    public float blastRadius;

    #endregion

    #region Shooting Variables

    #region External

    [Tooltip("The size of the cone that the pellets fire in")]
    public float maxProjectileSpreadAngle = 1;

    public float defaultAngle = 1;

    [Tooltip("The accuracy increase aiming grants the player. Makes the cone this much smaller when aiming")]
    public float aimingAngleDivider = 2;

    [Tooltip("The ammo that the gun uses, should be attached as a child of the gun")]
    public Amount ammoAmount;

    public float kickBackAmount = .1f, kickBackSpeed = 10, kickBackTime = .1f;
    public float kickBackRecoverWaitTime = .1f, kickBackRecoverSpeed = 10, kickBackRecoverTime = .1f;
    #endregion

    #region Internal
    //The main player camera
    Camera cam;

    //The audio controller attached to the gun
    [SerializeField]
    AudioSource aud;

    [Tooltip("The sound the gun makes when it shoots")]
    public AudioClip gunShot;
    //Defaults to 1 shot a second
    float fireRate = 1;

    //The time since the last shot was fired
    float lastShot;


    //The amount the camera zooms in, .5 is the smallest it can go
    float zoomAmount = 0;

    [HideInInspector]
    //The amount of ammo currently in the gun's magazine
    public float currentGunMagazine;

    //Defaults to 4, can be increased through the IncreaseMagazineSize() method
    float maxMagazineSize;

    [HideInInspector]
    public bool magazineEmpty;

    [HideInInspector]
    public float reloadTimer;

    //Set by playerProperties script, used internally and in Animator
    float reloadSpeed;

    float maxAngleMutliplier = 1;

    [HideInInspector]
    public bool aiming = false;

    //Used to delay weapon switching until the aimin animation is finished playing
    float aimingTimer;

    [HideInInspector]
    public bool canSwitchWeapon;

    #endregion

    #endregion

    #endregion

    // Use this for initialization
    void Start () {

        aud = GetComponent<AudioSource>();
        player = transform.root.gameObject;
        weaponSwitch = player.GetComponent<WeaponSwitch>();
        gunAnim = GetComponent<Animator>();
        updateAmmo = player.transform.Find("SinglePlayer UI/HUD/Ammo Count").GetComponent<UpdateAmmo>();
        if (ammoAmount == null)
            ammoAmount = transform.Find("Ammo").GetComponent<Amount>();
 
        cam = player.transform.Find("Camera").GetComponent<Camera>();
        playerPropterties = player.GetComponent<PlayerProperties>();
       

       
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        reloadSpeed = playerPropterties.handSpeed;

        #region Setting Aiming
        if (aiming)
        {
            aimingTimer = .25f / reloadSpeed;
            playerPropterties.SetAiming(true);
        }
        else
        {
            playerPropterties.SetAiming(false);
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
            if (Input.GetAxis("Aim") != 0)
            {
                maxAngleMutliplier = 1 / aimingAngleDivider;
                aiming = true;
                if (cam.fieldOfView > (30 / zoomAmount))
                {
                    cam.fieldOfView -= 250 * Time.deltaTime;
                }
            }
            else
            {
                maxAngleMutliplier = 1;
                aiming = false;
                if (cam.fieldOfView < 60)
                    cam.fieldOfView += 250f * Time.deltaTime;
            }
        }
        #endregion

        #region Setting gunAnim bool
        gunAnim.SetBool("Aiming", aiming);
        #endregion

        #region Setting maxPelletSpreadAngle
        maxProjectileSpreadAngle = defaultAngle;
        maxProjectileSpreadAngle *= maxAngleMutliplier;
        #endregion

        #region Shooting
        #region Valdating the player meets the shoot requirements
        if (Input.GetButton("Fire")) //Player presses the shoot button
        {
            if (currentGunMagazine > 0) //Player has enough ammo
            {
                if (reloadTimer <= 0) //Player is not reloading
                {
                    if (Time.time > fireRate + lastShot) //They are within the fireRate
                    #endregion
                    {
                        StartCoroutine("Recoil");
                        //The player can shoot
                        currentGunMagazine -= 1f;//Using 1 ammo
                        lastShot = Time.time; //Setting lastShot equal to current time
                        
                        gunAnim.SetTrigger("Fire");
                        gunAnim.SetTrigger("Shoot");



                        #region Creating a random Ray with an unchanging starting point within a cone
                        Ray cameraRay = new Ray(transform.position, transform.forward);
                        Vector3 fireDirection;
                        RaycastHit hit;
                        if(Physics.Raycast(cameraRay, out hit))
                        {
                            if (!aiming)
                                fireDirection = hit.point - projectileSpawnTransform.transform.position;
                            else
                                fireDirection = hit.point - transform.root.gameObject.transform.position;
                        }
                        else
                        {
                            fireDirection = projectileSpawnTransform.transform.forward;
                        }

                            
                            Quaternion fireRotation = Quaternion.LookRotation(fireDirection);
                            Quaternion randomRotation = Random.rotation;

                            fireRotation = Quaternion.RotateTowards(fireRotation, randomRotation, Random.Range(0f, maxProjectileSpreadAngle));
                            Ray fireRay = new Ray(projectileSpawnTransform.transform.position, fireRotation * Vector3.forward);
                        #endregion

                        projectileRigidBody = projectilePrefab.GetComponent<Rigidbody>();

                        aud.PlayOneShot(gunShot);

                        #region Shooting Projectile in Direction of Ray

                        GameObject tempProjectile = Instantiate(projectilePrefab, projectileSpawnTransform.transform.position, (Quaternion)projectileSpawnTransform.rotation);
                        tempProjectile.SendMessage("SetDamage", projectileDamage);
                        tempProjectile.SendMessage("SetBlastRadius", blastRadius);
                        tempProjectile.SendMessage("SetPlayerProperties", playerPropterties);

                        projectileRigidBody = tempProjectile.GetComponent<Rigidbody>();
                        projectileRigidBody.mass = projectileMass;
                        projectileRigidBody.AddForce(fireRay.direction * shootSpeed);

                        Destroy(tempProjectile, 20f);
                        #endregion


                    }
                }
            }
        }
        #endregion

        #region If Player Presses Reload Button
        if (reloadTimer <= 0)
        {
            if (Input.GetButtonDown("Reload"))
                Reload();
        }
        #endregion

        #region Timers  
        reloadTimer -= Time.deltaTime;
        aimingTimer -= Time.deltaTime;
        #endregion

        #region Setting Magazine Empty
        magazineEmpty = currentGunMagazine < 1;
        #endregion

        #region Updating Update Ammo and Weapon Switch
        updateAmmo.ChangeAmountObject(ammoAmount, currentGunMagazine);
        weaponSwitch.SetCanSwitchWeapon(canSwitchWeapon);
        #endregion

        if (magazineEmpty)
        {
            gunAnim.SetBool("Empty", true);
        }
        else
        {
            gunAnim.SetBool("Empty", false);
        }
    }

    void Reload()
    {
        gunAnim.SetTrigger("Reload");
        gunAnim.SetFloat("Reload Speed", reloadSpeed);

        reloadTimer = (1f / reloadSpeed) + (.5f - (reloadSpeed / 10));
        if (currentGunMagazine < maxMagazineSize)
        {
            if (ammoAmount.amountOf >= maxMagazineSize)
            {
                ammoAmount.amountOf -= (maxMagazineSize - currentGunMagazine);
                currentGunMagazine = maxMagazineSize;
            }
            else if (ammoAmount.amountOf > 0)
            {
                while (currentGunMagazine < maxMagazineSize && ammoAmount.amountOf > 0)
                {
                    currentGunMagazine++;
                    ammoAmount.amountOf--;
                }
            }
        }

    }

    public void AddToDefaultAngle(float amountToAdd)
    {
        defaultAngle += amountToAdd;
    }
    public void SubtractFromDefaultAngle(float amountToSubtract)
    {
        defaultAngle -= amountToSubtract;
    }
    public void MultiplyDefaultAngle(float amountToMultiply)
    {
        defaultAngle *= amountToMultiply;
    }
    public void DivideDefaultAngle(float amountToDivide)
    {
        defaultAngle /= amountToDivide;
    }

    public void QuickMelee(float meleeTime)
    {
        Debug.Log("Called melee");
        gunAnim.StopPlayback();
        reloadTimer = meleeTime;
        gunAnim.SetTrigger("Melee");
    }
}
