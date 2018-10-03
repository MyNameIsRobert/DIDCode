using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Written By Robert Clark
//Updates: 6/3/17 - Removed all Hitmarker references and variables, and moved them to PlayerPropterties Script
public class GunShoot : Shoot {


    #region Variable Declaration

    [Header("GunShoot Variables")]
    //HealthSinglePlayer script, set to the script attached to whatever gameobject is shot, used to do damge
    HealthSinglePlayer health;

    //How long the reload animation plays; delays shooting, weaponSwitching, and reloading; until the timer is 0
    [HideInInspector]
    public float reloadTimer;

    //Added to the maxAngleMultiplier, should be zero by default, used to increase bullet spread per gun
    [Tooltip("Should default to 0, is used to increase overall spread for a gun")]
    public float angleSpreadAdder = 1;

    //Is used to change the angleMultiplier to increase the bullet spread the longer you shoot, can be reworked, but is perfectly fine for now
    [HideInInspector]
    public float bulletSpreadTimer;

    //Used to delay weapon switching until the aiming animation is finshed playing
    float aimingTimer;

    //Is used in the weaponSwitch script, to stop weapon switching in certain circumstances
    [HideInInspector]
    public bool canSwitchWeapon;

    WeaponSwitch weaponSwitch;

    float appliedReloadSpeed;
    float appliedADSSpeed;

    GameObject reticle_Center;

    List<GameObject> reticle_OffCenters = new List<GameObject>();
    bool complexReticle = false;
    List<Vector2> reticle_OffCenter_Locations = new List<Vector2>();
    public float reticleIncreaseAmount = 1;
    public float reticleDecreaseSpeed = 5;




    [HideInInspector]
    public bool asSoonAsMagEmpty = false;
    [HideInInspector]
    public bool whenFiringEmptyMag = false;
    [HideInInspector]
    public bool whenButtonPressed = false;



    #endregion


    private void Awake()
    {
        shootType = ShootType.GunShoot;
    }

    public override void CopyFrom(Weapon weapon)
    {
        base.CopyFrom(weapon);
        if(weapon.weaponType == WeaponType.Gun)
        {
            Shoot tempShoot = (Shoot)weapon;
            if (tempShoot.shootType == ShootType.GunShoot)
            {
                GunShoot gunShoot = (GunShoot)tempShoot;
                angleSpreadAdder = gunShoot.angleSpreadAdder;
                reticleDecreaseSpeed = gunShoot.reticleDecreaseSpeed;
                reticleGameObject = gunShoot.reticleGameObject;
                reticleIncreaseAmount = gunShoot.reticleIncreaseAmount;
                reticleHasBloom = gunShoot.reticleHasBloom;
            }
        }

    }

    #region Start Method
    public override void Start () {

        base.Start();
        if (isPickup)
            return;
        if(player == null)
        {
            player = transform.root.gameObject;
        }
        if (playerProperties == null)
            playerProperties = player.GetComponent<PlayerProperties>();
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
    public override void Update()
    {
        base.Update();
        if (isPickup || !weaponEnabled)
            return;
        if (Time.timeScale == 0)
            return;
        
        reloadSpeed = playerProperties.handSpeed;

        appliedReloadSpeed = reloadSpeed * reloadSpeedModifier;
        appliedADSSpeed = reloadSpeed * aimSpeed;
        //If player is aiming, set the aiming timer relative to the reload speed
        if (aiming)
        {
            aimingTimer = .25f / appliedADSSpeed; //As reload speed increases, the aiming timer decreases, so it is equal to the animation speed
            playerProperties.SetAiming(true);
            anim.SetFloat("ADS Speed", appliedADSSpeed);
            anim.SetLayerWeight(2, .05f);
        }
        else
        {
            anim.SetLayerWeight(2, .2f);
        }
        //Sets canSwitchWeapon
        if (reloadTimer > 0)//If the player is still reloading
        {
            canSwitchWeapon = false;//They can't switch weapons
        }
        else
        {//The player is not reloading
            if (aimingTimer > 0)//But the player is either aiming, or the aiming animation is still playing
                canSwitchWeapon = false;//They can't switch weapons
            else//Not aiming or reloading
                canSwitchWeapon = true;//They can switch weapons
        }


        if(currentMagazine > magazineSize)
        {
            currentMagazine = magazineSize;
        }

        //Sets the angleMultiplier depending on whether or not the player is aiming
       if (reloadTimer <= 0)//If not reloading
        {
            if (Input.GetKey(control.aim))//If user presses the aim button
            {
                maxAngleMultiplier = 1 / aimingDivider; 
                aiming = true; //If the user is pressing the aim button, then aiming is true
                if(cam.fieldOfView > (30 / zoomAmount))
                    cam.fieldOfView -= 250 * Time.deltaTime;
            }
            else   //The user isn't pressing the aim button
            {
                aiming = false;
                if(cam.fieldOfView < 60)
                    cam.fieldOfView += 250 * Time.deltaTime;
            }
        }

        if (coroutinePlaying)
        {
            aiming = false;
        }

        //Plays the aiming animation depending on whether or not the player is aiming
        anim.SetBool("Aiming", aiming);
        maxSpreadAngle = defaultAngle; 
        maxSpreadAngle *= maxAngleMultiplier; //Multiplies the maxBulletSpreadAngle by the multiplier, which is changed throughout the script depending on different things
        maxSpreadAngle *= accuracyModifier;

        //Actually Shooting
        #region Shooting
        if (canFireAuto)
        {
            currentMagazine -= 1f; //Subract one from the current gun magazine
            lastShot = Time.time; //lastShot is set equal to the current time, to compare in for the next shot
            RaycastHit hit; //Creates a raycasthit called hit

            
            anim.SetTrigger("Shoot");//Triggers the firing animation

            //vvvvvvvv Complicated math to create a random ray inside a cone pointing forward vvvvv
            Vector3 fireDirection = cam.transform.forward;
            Quaternion fireRotation = Quaternion.LookRotation(fireDirection);
            Quaternion randomRotation = Random.rotation;

            fireRotation = Quaternion.RotateTowards(fireRotation, randomRotation, Random.Range(0f, maxSpreadAngle));

            //^^^^^^^ Complicated math to create a random ray inside a cone pointing forward ^^^^^^

            //Creates a new ray originating at the cameras position, and pointing in the random direction that was created above
            Ray ray = new Ray(cam.transform.position, fireRotation * Vector3.forward);

            //Plays the gunShot sound once
            aud.PlayOneShot(attackSound);

            if (Physics.Raycast(ray, out hit)) //If the raycast hits a collider, returns information about the collider in a variable called hit
            {
                var direction = cam.transform.position - hit.point; //Sets a direction that faces the player
                var tempParticle = Instantiate(hitParticle, hit.point, Quaternion.LookRotation(direction)); //Sets tempParticle equal to an instantiated hit particle facing the player
                tempParticle.GetComponent<SpawnBulletHoleImage>().SetHitRotation(hit.normal);
                tempParticle.GetComponent<SpawnBulletHoleImage>().SetHit(hit);
                Destroy(tempParticle, 1); //Destroys the particle object after a second, to get rid of unnecessary objects

                if (hit.collider.CompareTag("Enemy Body")) //if the object hit is tagged with "Enemy Body"
                {
                    if (hit.collider.transform.root.GetComponent<HealthSinglePlayer>()) //If the collider has a HealthSinglePlayer component attached
                    {
                        playerProperties.ShowHitmarker(false, (int)damage);
                        health = hit.collider.transform.root.GetComponent<HealthSinglePlayer>(); //Sets the health to the health script attached to the gameObject hit
                        health.TakeDamage(damage, !isSilenced); //Calls the takeDamage method using damage as the argument

                    }
                }
                else if (hit.collider.CompareTag("Enemy Head")) //If the object hit is tagged with "Enemy head"
                {
                    if (hit.collider.transform.root.GetComponent<HealthSinglePlayer>())
                    {
                        playerProperties.ShowHitmarker(true, (int)(damage * critMultiplier));
                        health = hit.collider.transform.root.GetComponent<HealthSinglePlayer>(); //Same as before
                        health.TakeDamage(damage * critMultiplier, !isSilenced); //Deals double damage

                    }
                }
            }
            StartRecoil();
        }
        #endregion

        UpdateReticle();

        //Reload call when "R" is pressed
        if (!coroutinePlaying) //Not reloading a
        {
            if (Input.GetKeyDown(control.reload) && (currentMagazine != magazineSize && playerAmmo.GetAmount(ammoType) != 0)) //Player presses reload button
            {
                Reload(true); //Call the reload method

            }
        }

        //Sets the angleMultiplier
        if (!aiming)
        { //If the player isn't aiming. Angle multiplier is changed higher up if the player is aiming
            playerProperties.SetAiming(false);
            if (Input.GetKey(control.fire))
            { //The fire button is being held down
                if (bulletSpreadTimer < 5)
                { //The bulletSpreadTimer is less than 5 (seconds)
                    bulletSpreadTimer += Time.deltaTime; //Continue to increase the timer
                }
                 if (bulletSpreadTimer < 6)
                { 
                    maxAngleMultiplier = angleSpreadAdder + bulletSpreadTimer; //Increase multiplier

                }

            }
            
            else
            { //The fire button isn't held down
                bulletSpreadTimer = 0;  //Set the timer to 0
                if (maxAngleMultiplier > 1)
                    maxAngleMultiplier -= Time.deltaTime; //Decrease the angleMultiplier over time
            }
        }

        //Sets magazine empty to whether or not currentMagaine is less than 1
        magazineEmpty = (currentMagazine < 1);

        if (magazineEmpty && asSoonAsMagEmpty && !coroutinePlaying)
            Reload(true);

        if (magazineEmpty)
            anim.SetBool("Gun Empty", true);
        else
            anim.SetBool("Gun Empty", false);
        

        //Decreasing general timers
        reloadTimer -= Time.deltaTime;
        aimingTimer -= Time.deltaTime;


        //Sending reloadSpeed float to the animator
       // armAnim.SetFloat("Reload Speed", reloadSpeed);
        anim.SetFloat("Reload Speed", appliedReloadSpeed);

        updateAmmo.ChangeAmountObject(ammoAmount, currentMagazine);
        weaponSwitch.SetCanSwitchWeapon(canSwitchWeapon);
        

    }


    protected override void UpdateReticle()
    {
        base.UpdateReticle();
        //Bullet spread timer must be inbetween 0 and 5. So, to turn it into a ratio of 0-1, we increase it by one, and then divide it by 6, so as not
        //to divide by 0. Then we add that product to reticleIncraseAmount
        float tempAmount = (((bulletSpreadTimer) / 5) * reticleIncreaseAmount) + 1;
        reticleUIElement.GetComponent<Image>().color = new Color(reticleUIElement.GetComponent<Image>().color.r, reticleUIElement.GetComponent<Image>().color.g, reticleUIElement.GetComponent<Image>().color.b, 0);
        if (reticleHasBloom)
        {

            for (int i = 0; i < reticle_OffCenters.Count; i++)
            {
                if (magazineEmpty || !Input.GetKey(control.fire))
                {
                    if(Mathf.Abs(reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.x) > Mathf.Abs(reticle_OffCenter_Locations[i].x))
                    {
                        if(reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.x > 0)
                        {
                            reticle_OffCenters[i].GetComponent<RectTransform>().localPosition = new Vector2(reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.x - Time.deltaTime * reticleDecreaseSpeed, reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.y);
                        }
                        else
                        {
                            reticle_OffCenters[i].GetComponent<RectTransform>().localPosition = new Vector2(reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.x + Time.deltaTime * reticleDecreaseSpeed, reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.y);
                        }
                    }
                    if(Mathf.Abs(reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.y) > Mathf.Abs(reticle_OffCenter_Locations[i].y))
                    {
                        if(reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.y > 0)
                        {
                            reticle_OffCenters[i].GetComponent<RectTransform>().localPosition = new Vector2(reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.x, reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.y - Time.deltaTime * reticleDecreaseSpeed);

                        }
                        else
                        {
                            reticle_OffCenters[i].GetComponent<RectTransform>().localPosition = new Vector2(reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.x, reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.y + Time.deltaTime * reticleDecreaseSpeed);
                        }
                    }
                    if(Mathf.Abs(reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.y) < reticle_OffCenter_Locations[i].y || Mathf.Abs(reticle_OffCenters[i].GetComponent<RectTransform>().localPosition.x) < reticle_OffCenter_Locations[i].x)
                    {
                        reticle_OffCenters[i].GetComponent<RectTransform>().localPosition = reticle_OffCenter_Locations[i];
                    }

                     
                }
                else if(canFireAuto)
                {
                    reticle_OffCenters[i].GetComponent<RectTransform>().localPosition = new Vector2(tempAmount * reticle_OffCenter_Locations[i].x, tempAmount * reticle_OffCenter_Locations[i].y);
                }
            }
        }

    }

    GameObject spawnedReticleTemp_ = null;

    protected override void OnWeaponDisabled()
    {
        base.OnWeaponDisabled();
        if (spawnedReticleTemp_ != null)
            Destroy(spawnedReticleTemp_);
        spawnedReticleTemp_ = null;
        reticle_Center = null;
        reticle_OffCenters.Clear();
        reticle_OffCenter_Locations.Clear();
    }
    protected override void OnWeaponEnabled()
    {
        base.OnWeaponEnabled();
        if (!reticleGameObject)
            return;
        //Debug.Log("Called on weapon enable");
       // Debug.Log(reticleGameObject + " " + reticleUIElement.transform);
        spawnedReticleTemp_ = Instantiate(reticleGameObject, reticleUIElement.transform);
        spawnedReticleTemp_.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        if (reticleGameObject.transform.childCount > 0)
        {
            for (int i = 0; i < reticleGameObject.transform.childCount; i++)
            {
                if (reticleGameObject.transform.GetChild(i).CompareTag("Reticle Center"))
                {
                    reticle_Center = spawnedReticleTemp_.transform.GetChild(i).gameObject;
                }
                else if (reticleGameObject.transform.GetChild(i).CompareTag("Reticle Off Center"))
                {
                    reticle_OffCenters.Add(spawnedReticleTemp_.transform.GetChild(i).gameObject);
                    Vector2 tempVect = new Vector2(spawnedReticleTemp_.transform.GetChild(i).GetComponent<RectTransform>().localPosition.x, spawnedReticleTemp_.transform.GetChild(i).GetComponent<RectTransform>().localPosition.y);
                    reticle_OffCenter_Locations.Add(tempVect);
                    complexReticle = true;
                }
            }
        }
    }
}


