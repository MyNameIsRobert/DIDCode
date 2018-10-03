//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class WeaponProperties : MonoBehaviour
//{
//    #region Variables

//    GameObject go;

//    float playerAccuracyModifer = 1;

//    #region Class Variables
//    [SerializeField]
//    DefaultVariables defaultVariables = new DefaultVariables();
//    [SerializeField]
//    ProjectileVariables projectileVariables = new ProjectileVariables();
//    [SerializeField]
//    ShotgunVariables shotgunVariables = new ShotgunVariables();
//    [SerializeField]
//    MaxAndMinVariables maxAndMinVariables = new MaxAndMinVariables();
//    [SerializeField]
//    AttachmentVariables attachmentVariables = new AttachmentVariables();
//    [SerializeField]
//    MeleeVariables meleeVariables = new MeleeVariables();
//    GameObject rootPlayer;
    
//#endregion
//    class LazerVariables: System.Object
//    {
//        float lazerLength;
//    }
//    #region Default Variables
//    [System.Serializable]
//    class DefaultVariables : System.Object
//    {
//        [SerializeField]
//        [Tooltip("The time between each shot. So 1 would mean one shot per second, and .5 would mean two shots per second")]
//        public float fireRate = .1f;
//        [SerializeField]
//        [Tooltip("The amount of ammo that can fit in a magazine")]
//        public float maxMagazineSize = 10;
//        [SerializeField]
//        [Tooltip("The amount of damage the projectile/pellets/bullets do, depending on which Shoot Method is Used")]
//        public float damage = 10;
//        [SerializeField]
//        [Tooltip("The amount the camera zooms in when aiming")]
//        public float zoomAmount = 1;
//        [SerializeField]
//        [Tooltip("The limit to the amount of ammo the player can hold for this specific gun")]
//        public float ammoLimit = 100;
//        [SerializeField]
//        [Tooltip("UNUSED")]
//        public float damageFallOff = 1;
//        [SerializeField]
//        [Tooltip("The multiplier applied to damage if it's a headshot")]
//        public float critHitMultiplier = 2;
//        [SerializeField]
//        [Tooltip("The current magazine. When changed in the editor, only affects how much ammo is in the magazine when picked up")]
//        public float currentMagazine = 10;
//        [SerializeField]
//        [Tooltip("The modifier applied as the last calculation before the bullet is fired, so it is the final overall modifier to accuracy")]
//        public float accuracyModifier = 1;
//        [SerializeField]
//        [Tooltip("How much the accuracy spread is divided by when aiming down sigts")]
//        public float aimingAngleDivider = 2;
//        [SerializeField]
//        [Tooltip("The default accuracy of the gun. The accuracy before all modifers have been applied")]
//        public float defaultAngle = 1;
//        [SerializeField]
//        public float reloadSpeedModifier = 1;
//        [SerializeField]
//        public float aDSSpeedModifier = 1;
//        [SerializeField]
//        public AudioClip defaultShootSound;
//        [SerializeField]
//        public AudioClip silencedShootSound;
//        [SerializeField]
//        public bool isSilenced = false;
//        [SerializeField]
//        public float kickBackAmount = 1;

//    }
//    #endregion

//    #region Projectile Variables
//    [System.Serializable]
//    class ProjectileVariables : System.Object
//    {
//        [SerializeField]
//        public float projectileSpeed;
//        [SerializeField]
//        public float projectileWeight;
//        [SerializeField]
//        [Tooltip("The projectile fired by the gun. Should have it's own script that applies damage.")]
//        public GameObject projectile;
//        [SerializeField]
//        public float blastRadius;
//    }
//    #endregion
    
//    #region Shotgun Variables
//    [System.Serializable]
//    class ShotgunVariables : System.Object
//    {
//        [SerializeField]
//        [Tooltip("The amount of pellets fired per shot. If it's not a shotgun weapon, set to 1 just to be safe.")]
//        public int numOfPellets;
//    }
//    #endregion

//    #region Melee Weapon Variables
//    [System.Serializable]
//    class MeleeVariables : System.Object
//    {
//        public float swingTime = .25f;
//        public bool canCleave = true;
//        public float damagePerSecondWhileCleaving = 100;
//    }

//#endregion

//    #region Max and Mins
//    [System.Serializable]
//    class MaxAndMinVariables : System.Object { 
//    [SerializeField]
//    [Tooltip("The minimum the firerate can be. Used to limit the weapon upgrades")]
//    public float minFireRate;

//    [SerializeField]
//    [Tooltip("The max that the maxMagazineSize can be. Used to limit how big the player can make a clip")]
//    public float maximumMaxMagazineSize;

//    [SerializeField]
//    [Tooltip("The max damage a bullet/pellet/projectile can have.")]
//    public float maxDamage;

//    [SerializeField]
//    [Tooltip("The maximum amount that the gun can zoom in. Defaults to zero, and probably won't need to be changed")]
//    public float maxZoomAmount = 2f;

//    [SerializeField]
//    [Tooltip("The maximum the ammo limit can be set to. So the player doesn't get infinite ammo unless the game/gun allows it.")]
//    public float maxAmmoLimit;

//    [SerializeField]
//    [Tooltip("UNUSED")]
//    public float maxDamageFallOff;

//    [SerializeField]
//    [Tooltip("The maximum the headshot/critical hit multiplier can be")]
//    public float maxCritHitMultiplier;

//    [SerializeField]
//    [Tooltip("The maximum blast radius for a projectile weapon")]
//    public float maxBlastRadius;

//    [SerializeField]
//    [Tooltip("The maximum number of pellets in a shotgun b")]
//    public int maxNumOfPellets;
//    }
//    #endregion

//    #region AttachmentVariables
//    [System.Serializable]
//    class AttachmentVariables
//    {
//        [Tooltip("0 is Sights 1 is magazine 2 is left barrel 3 is Underbarrel 4 is muzzle 5 is stock")]
//        public Attachment[] attachments;
//        public Transform[] attachmentPos;

//        public GameObject attachmentParent;
//    }
//#endregion

//    #region GUI
//    [SerializeField]
//    [Tooltip("The icon that will show on the minimap")]
//    GameObject minimapInidcator;
//    [SerializeField]
//    [Tooltip("The UI element in the center of the screen that guides the player in shooting")]
//    Sprite reticle;
//    #endregion

//    private void Reset()
//    {
        
//            attachmentVariables.attachments = new Attachment[6];
//            attachmentVariables.attachmentPos = new Transform[6];

//    }
//    #endregion
//    private void Start()
//    {

//        FindAttachmentParent();


//        if(attachmentVariables.attachments.Length != 6)
//        {
//            attachmentVariables.attachments = new Attachment[6];
//        }
//        if (attachmentVariables.attachmentPos.Length != 6)
//        {
//            attachmentVariables.attachmentPos = new Transform[6];
//        }

//        if (transform.root.gameObject.CompareTag("Player"))
//        {
//            rootPlayer = transform.root.gameObject;
//        }
//        if(rootPlayer != null)
//            playerAccuracyModifer = rootPlayer.GetComponent<PlayerProperties>().accuracyModifier;
//        //SpawnAttachments();

//        defaultVariables.accuracyModifier *= playerAccuracyModifer;
//    }
//    private void Update()
//    {
//        if (rootPlayer != null)
//            playerAccuracyModifer = rootPlayer.GetComponent<PlayerProperties>().accuracyModifier;
//    }

//    #region Get Methods
//    public float GetLazerLength()
//    {

//    }
//    public float GetSwingTime()
//    {
//        return meleeVariables.swingTime;
//    }
//    public bool GetCanCleave()
//    {
//        return meleeVariables.canCleave;
//    }
//    public float GetDamagePerSecondWhileCleaving()
//    {
//        return meleeVariables.damagePerSecondWhileCleaving;
//    }
//    public float GetFireRate()
//    {
//        return defaultVariables.fireRate;
//    }
//    public float GetMaxMagazineSize()
//    {
//        return defaultVariables.maxMagazineSize;
//    }
//    public float GetDamage()
//    {
//        return defaultVariables.damage;
//    }
//    public float GetZoomAmount()
//    {
//        return defaultVariables.zoomAmount;
//    }
//    public float GetAmmoLimit()
//    {
//        return defaultVariables.ammoLimit;
//    }
//    public float GetDamageFallOff()
//    {
//        return defaultVariables.damageFallOff;
//    }
//    public float GetCritHitMultiplier()
//    {
//        return defaultVariables.critHitMultiplier;
//    }
//    public float GetCurrentMagazine()
//    {
//        return defaultVariables.currentMagazine;
//    }
//    public float GetAccuracyModifier()
//    {
//        return defaultVariables.accuracyModifier;
//    }
//    public float GetAimingAngleDivider()
//    {
//        return defaultVariables.aimingAngleDivider;
//    }
//    public float GetDefaultAngle()
//    {
//        return defaultVariables.defaultAngle;
//    }
//    public float GetReloadSpeedModifier()
//    {
//        return defaultVariables.reloadSpeedModifier;
//    }
//    public float GetADSSpeedModifier()
//    {
//        return defaultVariables.aDSSpeedModifier;
//    }
//    public float GetProjectileSpeed()
//    {
//        return projectileVariables.projectileSpeed;
//    }
//    public float GetProjectileWeight()
//    {
//        return projectileVariables.projectileWeight;
//    }
//    public GameObject GetProjectile()
//    {
//        return projectileVariables.projectile;
//    }
//    public float GetBlastRadius()
//    {
//        return projectileVariables.blastRadius;
//    }
//    public int GetNumOfPellets()
//    {
//        return shotgunVariables.numOfPellets;
//    }
//    public float GetMinFireRate()
//    {
//        return maxAndMinVariables.minFireRate;      
//    }
//    public float GetMaximumMaxMagazineSize()
//    {
//        return maxAndMinVariables.maximumMaxMagazineSize;
//    }
//    public float GetMaxDamage()
//    {
//        return maxAndMinVariables.maxDamage;
//    }
//    public float GetMaxZoomAmount()
//    {
//        return maxAndMinVariables.maxZoomAmount;
//    }
//    public float GetMaxAmmoLimit()
//    {
//        return maxAndMinVariables.maxAmmoLimit;
//    }
//    public float GetMaxDamageFallOff()
//    {
//        return maxAndMinVariables.maxDamageFallOff;
//    }
//    public float GetMaxCritHitMultiplier()
//    {
//        return maxAndMinVariables.maxCritHitMultiplier;
//    }
//    public float GetMaxBlastRadius()
//    {
//        return maxAndMinVariables.maxBlastRadius;
//    }
//    public int GetMaxNumOfPellets()
//    {
//        return maxAndMinVariables.maxNumOfPellets;
//    }
//    public GameObject GetMinimapIndicator()
//    {
//        return minimapInidcator;
//    }
//    public Sprite GetReticle()
//    {
//        return reticle;
//    }
//    public GameObject GetAttachmentParent()
//    {
//        return attachmentVariables.attachmentParent;
//    }
//    public Attachment[] GetAttachments()
//    {
//        return attachmentVariables.attachments;
//    }
//    public Transform[] GetAttachmentPos()
//    {
//        return attachmentVariables.attachmentPos;
//    }
//    public float GetKickBackAmount()
//    {
//        return defaultVariables.kickBackAmount;
//    }
//    #endregion

//    #region Set Methods
//    public void SetLazerLength(float legn)
//    {

//    }
//    public void SetFireRate(float newFireRate)
//    {
//        defaultVariables.fireRate = newFireRate;
//    }
//    public void SetMaxMagazineSize(float newMaxMagazine)
//    {
//        Debug.Log(newMaxMagazine);
//        defaultVariables.maxMagazineSize = newMaxMagazine;
//    }
//    public void SetDamage(float newDamage)
//    {
//        defaultVariables.damage = newDamage;
//    }
//    public void SetZoomAmount(float newZoom)
//    {
//        defaultVariables.zoomAmount = newZoom;
//    }
//    public void SetAmmoLimit(float newLimit)
//    {
//        defaultVariables.ammoLimit = newLimit;
//    }
//    public void SetDamageFallOff(float newFallOff)
//    {
//        defaultVariables.damageFallOff = newFallOff;
//    }
//    public void SetCritHitMultiplier(float newMultiplier)
//    {
//        defaultVariables.critHitMultiplier = newMultiplier;
//    }
//    public void SetCurrentMagazine(float newMagazine)
//    {
//        defaultVariables.currentMagazine = newMagazine;
//    }
//    public void SetProjectileSpeed(float newSpeed)
//    {
//        projectileVariables.projectileSpeed = newSpeed;
//    }
//    public void SetProjectileWeight(float newWeight)
//    {
//        projectileVariables.projectileWeight = newWeight;
//    }
//    public void SetProjectile(GameObject newProjectile)
//    {
//        projectileVariables.projectile = newProjectile;
//    }
//    public void SetBlastRadius(float newRadius)
//    {
//        projectileVariables.blastRadius = newRadius;
//    }
//    public void SetNumOfPellets(int newPellets)
//    {
//        shotgunVariables.numOfPellets = newPellets;
//    }
//    public void SetMinimapIndicator(GameObject newminmap)
//    {
//        minimapInidcator = newminmap;
//    }
//    public void SetReticle(Sprite newReticle)
//    {
//        reticle = newReticle;
//    }



//    #endregion

//    #region Increase Methods
//    public bool IncreaseLazerLength(float am)
//    {

//    }
//    /// <summary>
//    /// Returns false if the fireRate has reached it's min
//    /// </summary>
//    /// <param name="amount">The amount to be added to fireRate. Can be negative.</param>
//    /// <returns></returns>
//    public bool IncreaseFireRate(float amount)
//    {
//        defaultVariables.fireRate += amount;
//        if(defaultVariables.fireRate < maxAndMinVariables.minFireRate)
//        {
//            defaultVariables.fireRate = maxAndMinVariables.minFireRate;
//            return false;
//        }
//        return true;
//    }
//    /// <summary>
//    /// Returns false if the MaxMagazineSize has reached it's max
//    /// </summary>
//    /// <param name="amount">The amount to be added to the maxMagazine size. Can be negative.</param>
//    /// <returns></returns>
//    public bool IncreaseMaxMagazineSize(float amount)
//    {
//        defaultVariables.maxMagazineSize += amount;
//        defaultVariables.maxMagazineSize = (int)(defaultVariables.maxMagazineSize + .5f);
//        if(defaultVariables.maxMagazineSize > maxAndMinVariables.maximumMaxMagazineSize)
//        {
//            defaultVariables.maxMagazineSize = maxAndMinVariables.maximumMaxMagazineSize;
//            return false;
//        }
//        return true;
//    }
//    /// <summary>
//    /// Returns false if the Damage has reached it's max
//    /// </summary>
//    /// <param name="amount">The amount to be added to Damage. Can be negative.</param>
//    /// <returns></returns>
//    public bool IncreaseDamage(float amount)
//    {
//        defaultVariables.damage += amount;
//        if(defaultVariables.damage > maxAndMinVariables.maxDamage)
//        {
//            defaultVariables.damage = maxAndMinVariables.maxDamage;
//            return false;
//        }
//        return true;
//    }
//    /// <summary>
//    /// Returns false if the defaultVariables.zoomAmount has reached it's max
//    /// </summary>
//    /// <param name="amount">The amount to be added to the defaultVariables.zoomAmount. Can be negative.</param>
//    /// <returns></returns>
//    public bool IncreaseZoomAmount(float amount)
//    {
//        defaultVariables.zoomAmount += amount;
//        if(defaultVariables.zoomAmount > maxAndMinVariables.maxZoomAmount)
//        {
//            defaultVariables.zoomAmount = maxAndMinVariables.maxZoomAmount;
//            return false;
//        }
//        return true;
//    }
//    /// <summary>
//    /// Returns false if the Ammo Limit has reached it's max
//    /// </summary>
//    /// <param name="amount">The amount to be added to the ammo limit. Can be negative.</param>
//    /// <returns></returns>
//    public bool IncreaseAmmoLimit(float amount)
//    {
//        defaultVariables.ammoLimit += amount;
//        if (defaultVariables.ammoLimit > maxAndMinVariables.maxAmmoLimit)
//        {
//            defaultVariables.ammoLimit = maxAndMinVariables.maxAmmoLimit;
//            return false;
//        }
//        return true;
//    }
//    /// <summary>
//    /// Returns false if the DamageFalloff has reached it's max
//    /// </summary>
//    /// <param name="amount">The amount to be added to the damageFallOff. Can be negative.</param>
//    /// <returns></returns>
//    public bool IncreaseDamageFallOff(float amount)
//    {
//        defaultVariables.damageFallOff += amount;
//        if(defaultVariables.damageFallOff > maxAndMinVariables.maxDamageFallOff)
//        {
//            defaultVariables.damageFallOff = maxAndMinVariables.maxDamageFallOff;
//            return false;
//        }
//        return true;
//    }
//    public bool IncreaseAccuracyModifier(float amount)
//    {
//        defaultVariables.accuracyModifier += amount;
//        defaultVariables.accuracyModifier *= playerAccuracyModifer;
//        return true;
//    }
//    /// <summary>
//    /// Returns false if the CriticalHitMultiplier has reached it's max
//    /// </summary>
//    /// <param name="amount">The amount to be added to the critical hit multiplier. Can be negative.</param>
//    /// <returns></returns>
//    public bool IncreaseCritHitMultiplier(float amount)
//    {
//        defaultVariables.critHitMultiplier += amount;
//        if(defaultVariables.critHitMultiplier > maxAndMinVariables.maxCritHitMultiplier)
//        {
//            defaultVariables.critHitMultiplier = maxAndMinVariables.maxCritHitMultiplier;
//            return false;
//        }
//        return true;
//    }
//    public bool IncreaseAccuracyModifer(float amount)
//    {
//        defaultVariables.accuracyModifier += amount;
//        return true;
//    }
//    public bool IncreaseAimingAngleDivider(float amount)
//    {
//        defaultVariables.aimingAngleDivider += amount;
//        return true;
//    }
//    public bool IncreaseReloadSpeedModifier(float amount)
//    {
//        defaultVariables.reloadSpeedModifier += amount;
//        return true;
//    }
//    public bool IncreaseADSSpeedModifier(float amount)
//    {
//        defaultVariables.aDSSpeedModifier += amount;
//        return true;
//    }
//    public bool IncreaseDefaultAngle(float amount)
//    {
//        defaultVariables.defaultAngle += amount;
//        return true;
//    }
//    public void IncreaseProjectileSpeed(float amount)
//    {
//        projectileVariables.projectileSpeed += amount;
//    }
//    public void IncreaseProjectileWeight(float amount)
//    {
//        projectileVariables.projectileWeight += amount;
//    }
//    /// <summary>
//    /// Returns false if the Blast Radius has reached it's max
//    /// </summary>
//    /// <param name="amount">The amount to be added to the blast radius. Can be negative.</param>
//    /// <returns></returns>
//    public bool IncreaseBlastRadius(float amount)
//    {
//        projectileVariables.blastRadius += amount;
//        if(projectileVariables.blastRadius > maxAndMinVariables.maxBlastRadius)
//        {
//            projectileVariables.blastRadius = maxAndMinVariables.maxBlastRadius;
//            return false;
//        }
//        return true;
//    }
//    /// <summary>
//    /// Returns false if the num of pellts has reached it's max
//    /// </summary>
//    /// <param name="amount">The amount to be added to the number of pellets. Can be negative.</param>
//    /// <returns></returns>
//    public bool IncreaseNumOfPellets(float amount)
//    {
//        shotgunVariables.numOfPellets += (int)amount;
//        if (shotgunVariables.numOfPellets > maxAndMinVariables.maxNumOfPellets)
//        {
//            shotgunVariables.numOfPellets = maxAndMinVariables.maxNumOfPellets;
//            return false;
//        }
//        return true;
//    }
//    public bool IncreaseKickBackAmount(float amount)
//    {
//        defaultVariables.kickBackAmount += amount;
//        return true;
//    }
//    #endregion

//   public void UpdateAllVariables(WeaponProperties oldVariables)
//    {
//        //start of default variables
//        Debug.Log("Update all variables was called");
//        defaultVariables.fireRate = oldVariables.GetFireRate();
//        defaultVariables.maxMagazineSize = oldVariables.GetMaxMagazineSize();
//        defaultVariables.damage = oldVariables.GetDamage();
//        defaultVariables.zoomAmount = oldVariables.GetZoomAmount();
//        defaultVariables.ammoLimit = oldVariables.GetAmmoLimit();
//        defaultVariables.damageFallOff = oldVariables.GetDamageFallOff();
//        defaultVariables.critHitMultiplier = oldVariables.GetCritHitMultiplier();
//        defaultVariables.currentMagazine = oldVariables.GetCurrentMagazine();
//        defaultVariables.accuracyModifier = oldVariables.GetAccuracyModifier();
//        defaultVariables.aimingAngleDivider = oldVariables.GetAimingAngleDivider();
//        defaultVariables.defaultAngle = oldVariables.GetDefaultAngle();
//        defaultVariables.reloadSpeedModifier = oldVariables.GetReloadSpeedModifier();
//        defaultVariables.aDSSpeedModifier = oldVariables.GetADSSpeedModifier();
//        defaultVariables.kickBackAmount = oldVariables.GetKickBackAmount();
   

//        // end of default variables

//        // start of projectile variables 

//        projectileVariables.projectileSpeed = oldVariables.GetProjectileSpeed();
//        projectileVariables.projectileWeight = oldVariables.GetProjectileWeight();
//        projectileVariables.projectile = oldVariables.GetProjectile();
//        projectileVariables.blastRadius = oldVariables.GetBlastRadius();

//        //end of projectile variables 

//        // start of shotgun variable(s)

//        shotgunVariables.numOfPellets = oldVariables.GetNumOfPellets();

//        //end of shotgun variable(s)

//        //start of max and min variables

//        maxAndMinVariables.minFireRate = oldVariables.GetMinFireRate();
//        maxAndMinVariables.maximumMaxMagazineSize = oldVariables.GetMaximumMaxMagazineSize();
//        maxAndMinVariables.maxDamage = oldVariables.GetMaxDamage();
//        maxAndMinVariables.maxZoomAmount = oldVariables.GetMaxZoomAmount();
//        maxAndMinVariables.maxAmmoLimit = oldVariables.GetMaxAmmoLimit();
//        maxAndMinVariables.maxDamageFallOff = oldVariables.GetMaxDamageFallOff();
//        maxAndMinVariables.maxCritHitMultiplier = oldVariables.GetMaxCritHitMultiplier();
//        maxAndMinVariables.maxBlastRadius = oldVariables.GetMaxBlastRadius();
//        maxAndMinVariables.maxNumOfPellets = oldVariables.GetMaxNumOfPellets();
//        // end of max and min variables
//        for(int i = 0; i < attachmentVariables.attachments.Length; i++)
//        {
//            attachmentVariables.attachments[i] = oldVariables.GetAttachments()[i];
//            attachmentVariables.attachmentPos[i].localPosition = oldVariables.GetAttachmentPos()[i].localPosition;
//            attachmentVariables.attachmentPos[i].localScale = oldVariables.GetAttachmentPos()[i].localScale;
//            attachmentVariables.attachmentPos[i].localRotation = oldVariables.GetAttachmentPos()[i].localRotation;
//        }

//        //Melee Variables
//        meleeVariables.swingTime = oldVariables.GetSwingTime();
//        meleeVariables.canCleave = oldVariables.GetCanCleave();
//        meleeVariables.damagePerSecondWhileCleaving = oldVariables.GetDamagePerSecondWhileCleaving();

        
//        //SpawnAttachments();
//        FindAttachmentParent();
//        //oldVariables.DespawnAttachments();
//        //UpdateAttachments();

       

//        minimapInidcator = oldVariables.GetMinimapIndicator();
//        reticle = oldVariables.GetReticle();
 
//    }

//    void FindAttachmentParent()
//    {
//        #region Finding Attachment Parent
//        if (attachmentVariables.attachmentParent == null)
//        {
//            if (gameObject.transform.Find("Attachments") != null)
//                attachmentVariables.attachmentParent = gameObject.transform.Find("Attachments").gameObject;
//        }
//        if (attachmentVariables.attachmentParent == null)
//        {
//            for (int i = 0; i < gameObject.transform.childCount; i++)
//            {
//                GameObject tempObject = gameObject.transform.GetChild(i).gameObject;
//                if (tempObject.name != "Attachments")
//                {
//                    for (int j = 0; j < tempObject.transform.childCount; j++)
//                    {
//                        GameObject tempObject2 = tempObject.transform.GetChild(j).gameObject;
//                        if (tempObject2.name == "Attachments")
//                        {
//                            attachmentVariables.attachmentParent = tempObject2;
//                            break;
//                        }
//                        else if (tempObject2.transform.Find("Attachments") != null)
//                        {
//                            attachmentVariables.attachmentParent = tempObject2.transform.Find("Attachments").gameObject;
//                        }
//                    }
//                }
//                else
//                {
//                    attachmentVariables.attachmentParent = tempObject;
//                    break;
//                }

//            }
//        }
//        if (attachmentVariables.attachmentParent == null)
//        {
//            Debug.Log("No attachment parent found on: " + gameObject.name + "! Please make sure the parent is named \"Attachments\"! If this error continues" +
//                        " to show up even with it correctly named, manually attach the object through the inspector");
//        }
//        #endregion
//    }
    

   
//    public bool CanAttachAttachment(Attachment newAttachment)
//    {
//        bool canAttach = false;
//        switch (newAttachment.attachmentType)
//        {
//            case Attachment.AttachmentType.Sights:
//                if (attachmentVariables.attachments[0] == null)
//                {
//                    canAttach = true;

//                }
//                else if ( attachmentVariables.attachments[0].itemName == "N/A")
//                {
                    
//                }
//                else
//                {
                    
//                }
//                break;
//            case Attachment.AttachmentType.Magazine:
//                if (attachmentVariables.attachments[1] == null)
//                {
//                    canAttach = true;
//                }
//                else if (attachmentVariables.attachments[1].itemName == "N/A")
//                {
                    
//                }
//                else
//                {
                    
//                }
//                break;
//            case Attachment.AttachmentType.LeftBarrel:
//                if (attachmentVariables.attachments[2] == null)
//                {
//                    canAttach = true;
//                }
//                else if (attachmentVariables.attachments[2].itemName == "N/A")
//                {
                    
//                }
//                else
//                {
                    
//                }
//                break;
//            case Attachment.AttachmentType.UnderBarrel:
//                if (attachmentVariables.attachments[3] == null)
//                {
//                    canAttach = true;
//                }
//                else if (attachmentVariables.attachments[3].itemName == "N/A")
//                {
                    
//                }
//                else
//                {
                    
//                }
//                break;
//            case Attachment.AttachmentType.Muzzle:
//                if (attachmentVariables.attachments[4] == null)
//                {
//                    canAttach = true;
//                }
//                else if (attachmentVariables.attachments[4].itemName == "N/A")
//                {
                    
//                }
//                else
//                {
                    
//                }
//                break;
//            case Attachment.AttachmentType.Stock:

//                if (attachmentVariables.attachments[5] == null)
//                {
//                    canAttach = true;
//                }
//                else if (attachmentVariables.attachments[5].itemName == "N/A")
//                {

//                }
//                else
//                {
                    
//                }
//                break;
//        }

//        return canAttach;
//    }


//    void DespawnAttachment(GameObject attachmentGameObject)
//    {
        
//        Destroy(attachmentVariables.attachmentParent.transform.Find(attachmentGameObject.name).gameObject);
//    }
//    void DespawnAttachments()
//    {
//        for(int i = 0; i < attachmentVariables.attachments.Length; i++)
//        {
//            if(attachmentVariables.attachments[i] != null && attachmentVariables.attachments[i].itemName != "N/A")
//            {
//                GameObject objectToDestroy = attachmentVariables.attachmentParent.transform.Find(attachmentVariables.attachments[i].attachmentGameObject.name).gameObject;
//                if (objectToDestroy != null)
//                    Destroy(objectToDestroy);
                
//            }
//        }
//    }


//}
