using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : Weapon
{

    #region Variables
    [HideInInspector]
    public bool isADS = false;
    [HideInInspector]
    public Amount ammoAmount;
    [HideInInspector]
    public bool magazineEmpty;
    [Header("Shooting Properties")]
    public float fireRate = .1f;

    public float magazineSize = 20;

    public AmmoEnum.AmmoType ammoType;

    public float zoomAmount = 1;


    public float critMultiplier = 2;

    public float projectileForce = 10;

    [HideInInspector]
    public float currentMagazine;

    public float accuracyModifier = 1;

    public float aimingDivider = 2;
    [Space(10)]
    public float defaultAngle = 1;

    public float maxSpreadAngle;

    public float maxAngleMultiplier;
    [Space(10)]
    public float reloadSpeed = 1;

    public float reloadSpeedModifier = 1;

    public float aimSpeed = 1;

    public float kickBackAmount = 1;
    public float horizontalKickBack = 1;
    [Space(10)]
    public GameObject attachmentParent;
    public Attachment[] attachments = new Attachment[6];
    public Transform[] attachmentPos = new Transform[6];


    [Space(10)]
    public GameObject hitParticle;

    [HideInInspector]
    public Camera cam;

    [HideInInspector]
    public PlayerAmmo playerAmmo;
    [HideInInspector]
    public bool aiming;

    public enum ReloadType
    {
        AsSoonAsMagazineIsEmpty,
        WhenTryingToFireAnEmptyMagazine,
        OnlyWhenButtonIsPressed
    }



    public ReloadType howToReload;
    public enum ReloadStyle
    {
        WholeMagazine,
        OneAtATime
    }

    [HideInInspector]
    public bool reloaded = false;
    //[HideInInspector]
    public bool coroutinePlaying = false;

    public enum ShootType
    {
        GunShoot,
        ShotgunShoot,
        SemiAutoShoot,
        ChargeShoot,
        ProjectileShoot,
        BurstShoot
    }

    public ShootType shootType;

    //[HideInInspector]
    public bool canFireAuto = false;
    //[HideInInspector]
    public bool canFireSemiAuto = false;
    [HideInInspector]
    public float lastShot = 0;
    //[HideInInspector]
    public PlayerInputControls control;
    #endregion


    IEnumerator Recoil()
    {
        horizontalKickBack = (Random.value > .5f) ? 0 - horizontalKickBack : horizontalKickBack;
        float time = .12f;
        while (time > 0) {
            time -= Time.deltaTime;
            cam.transform.Rotate(Vector3.left, (kickBackAmount * Time.deltaTime));
            player.transform.Rotate(Vector3.up, horizontalKickBack * Time.deltaTime);
            yield return null;
        }

        yield return null;
    }

    protected override void UpdateReticle()
    {
        base.UpdateReticle();
        Debug.Log("calling Shoot update Reticle");
        CanvasGroup localCanvasGroup;
        if (!reticleUIElement.GetComponent<CanvasGroup>())
        {
            localCanvasGroup = reticleUIElement.AddComponent<CanvasGroup>();
        }
        else
            localCanvasGroup = reticleUIElement.GetComponent<CanvasGroup>();

        if (aiming)
        {
            localCanvasGroup.alpha = 0;
        }
        else
            localCanvasGroup.alpha = 1;
    }
    // Use this for initialization
    public override void Start()
    {
        if (isPickup)
            return;
        base.Start();

        cam = playerProperties.cam;
        playerAmmo = player.GetComponent<PlayerAmmo>();
        updateAmmo = player.transform.Find("SinglePlayer UI/HUD/Ammo Count").GetComponent<UpdateAmmo>();
        control = player.GetComponent<PlayerInputControls>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (Time.timeScale == 0 || isPickup)
            return;
        if(!weaponEnabled)
        {
            StopCoroutine("AnimReload");
            coroutinePlaying = false;
            return;
        }
        if (playerProperties != null)
            accuracyModifier = playerProperties.accuracyModifier;
        if (isADS)
            Debug.Log("Has aimed down sights");



        if (Input.GetKeyDown(control.fire) && !coroutinePlaying && currentMagazine > 0)
        {
            canFireSemiAuto = true;            
        }
        else
        {
            canFireSemiAuto = false;
        }

        if (Input.GetKey(control.fire) && !coroutinePlaying && currentMagazine > 0 && (Time.time > lastShot + fireRate))
        {
            canFireAuto = true;
            lastShot = Time.time;
        }
        else
        {
            canFireAuto = false;
        }

        if (aiming)
        {
            anim.SetLayerWeight(2, .1f);
            anim.SetLayerWeight(3, .1f);
        }
        else
        {
            anim.SetLayerWeight(2, .21f);
            anim.SetLayerWeight(3, .2f);      
            isADS = false;
        }
        UpdateReticle();
        

    }

    public void Reload()
    {
        //reloadTimer = (1f / reloadSpeed) + (.5f - (reloadSpeed / 10));
        if (currentMagazine < magazineSize)
        {
            int heldAmmo = playerAmmo.GetAmount(ammoType);
            reloadSpeed = playerProperties.handSpeed * reloadSpeedModifier;
            anim.SetTrigger("Reload");
            anim.SetFloat("Reload Speed", reloadSpeed);
            if (heldAmmo >= magazineSize)
            {
                playerAmmo.RemoveAmmo((int)magazineSize - (int)currentMagazine, ammoType);
                currentMagazine = magazineSize;
            }
            else if (heldAmmo > 0)
            {
                while (currentMagazine < magazineSize && heldAmmo > 0)
                {
                    currentMagazine++;
                    playerAmmo.RemoveAmmo(1, ammoType);
                    heldAmmo = playerAmmo.GetAmount(ammoType);
                }
            }
        }
    }
    public void Reload(bool animReload)
    {
        if (!animReload)
        {
            Reload();
        }
        else
        {
            StartCoroutine("AnimReload");
        }
    }
    public void Reload(ReloadStyle style)
    {
        switch (style)
        {
            case ReloadStyle.OneAtATime:
                break;
            case ReloadStyle.WholeMagazine:
                break;
        }
    }
    IEnumerator AnimReload()
    {
        Debug.Log("AnimReload called");
        coroutinePlaying = true;
        reloaded = false;
        //reloadTimer = 2;
        anim.SetTrigger("Reload");
        while (cam.fieldOfView < 60)
        {
            cam.fieldOfView += 250 * Time.deltaTime;
            yield return null;
        }
        maxAngleMultiplier = 1;
        yield return new WaitUntil(() => reloaded);
        Debug.Log("Reloaded");
        if (currentMagazine < magazineSize)
        {
            int heldAmmo = playerAmmo.GetAmount(ammoType);
            if (heldAmmo >= magazineSize)
            {
                playerAmmo.RemoveAmmo((int)magazineSize - (int)currentMagazine, ammoType);
                currentMagazine = magazineSize;
            }
            else if (heldAmmo > 0)
            {
                while (currentMagazine < magazineSize && heldAmmo > 0)
                {
                    currentMagazine++;
                    playerAmmo.RemoveAmmo(1, ammoType);
                    heldAmmo = playerAmmo.GetAmount(ammoType);
                }
            }
        }
        coroutinePlaying = false;
        yield return null;
    }
    public void StartRecoil()
    {
        StartCoroutine(Recoil());
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
    public void ChangeAimingAngleDivider(float amount)
    {
        aimingDivider += amount;
    }
    public void ChangeAudioVolume(float amountToAdd)
    {
        aud.volume += amountToAdd;
        if (aud.volume < .1f)
        {
            aud.volume = .1f;
        }
    }
    public void QuickMelee(float meleeTime)
    {
        Debug.Log("Called melee");
        anim.StopPlayback();
        anim.SetTrigger("Melee");
    }

    public void CopyFrom(Shoot shoot)
    {
        damage = shoot.damage;
        weaponType = shoot.weaponType;

        attackSound = shoot.attackSound;
        silencedAttackSound = shoot.silencedAttackSound;
        isSilenced = shoot.isSilenced;


        reticle = shoot.reticle;
        
        fireRate = shoot.fireRate;
        magazineSize = shoot.magazineSize;
        zoomAmount = shoot.zoomAmount;
        critMultiplier = shoot.critMultiplier;
        currentMagazine = shoot.currentMagazine;
        accuracyModifier = shoot.accuracyModifier;
        aimingDivider = shoot.aimingDivider;
        defaultAngle = shoot.defaultAngle;
        maxSpreadAngle = shoot.maxSpreadAngle;
        maxAngleMultiplier = shoot.maxAngleMultiplier;
        reloadSpeed = shoot.reloadSpeed;
        reloadSpeedModifier = shoot.reloadSpeedModifier;
        aimSpeed = shoot.aimSpeed;
        kickBackAmount = shoot.kickBackAmount;
        horizontalKickBack = shoot.horizontalKickBack;
        hitParticle = shoot.hitParticle;
        ammoType = shoot.ammoType;
        for(int i = 0; i < attachmentPos.Length; i++)
        {
            if (shoot.attachmentPos[i] != null && attachmentPos[i] != null)
            {
                attachmentPos[i].localPosition = shoot.attachmentPos[i].localPosition;
                attachmentPos[i].localRotation = shoot.attachmentPos[i].localRotation;
                attachmentPos[i].localScale = shoot.attachmentPos[i].localScale;
            }
        }
        for(int i = 0; i < attachments.Length; i++)
        {
            attachments[i] = shoot.attachments[i];
            if(attachments[i] != null  && attachments[i].itemName != "N/A" && shoot.attachmentPos[i] != null && attachmentPos[i] != null)
                SpawnAttachments(i);
        }

    }
    public void CopyFrom(Shoot shoot, bool spawnAttachments)
    {
        damage = shoot.damage;
        weaponType = shoot.weaponType;

        attackSound = shoot.attackSound;
        silencedAttackSound = shoot.silencedAttackSound;
        isSilenced = shoot.isSilenced;


        reticle = shoot.reticle;

        fireRate = shoot.fireRate;
        magazineSize = shoot.magazineSize;
        zoomAmount = shoot.zoomAmount;
        critMultiplier = shoot.critMultiplier;
        currentMagazine = shoot.currentMagazine;
        accuracyModifier = shoot.accuracyModifier;
        aimingDivider = shoot.aimingDivider;
        defaultAngle = shoot.defaultAngle;
        maxSpreadAngle = shoot.maxSpreadAngle;
        maxAngleMultiplier = shoot.maxAngleMultiplier;
        reloadSpeed = shoot.reloadSpeed;
        reloadSpeedModifier = shoot.reloadSpeedModifier;
        aimSpeed = shoot.aimSpeed;
        kickBackAmount = shoot.kickBackAmount;
        horizontalKickBack = shoot.horizontalKickBack;
        hitParticle = shoot.hitParticle;
        ammoType = shoot.ammoType;
        for (int i = 0; i < attachmentPos.Length; i++)
        {
            if (shoot.attachmentPos[i] != null && attachmentPos[i] != null)
            {
                attachmentPos[i].localPosition = shoot.attachmentPos[i].localPosition;
                attachmentPos[i].localRotation = shoot.attachmentPos[i].localRotation;
                attachmentPos[i].localScale = shoot.attachmentPos[i].localScale;
            }
        }
    }

    public override void CopyFrom(Weapon weapon)
    {
        base.CopyFrom(weapon);
        if(weapon.weaponType == WeaponType.Gun)
        {
            Shoot shoot = (Shoot)weapon;
            damage = shoot.damage;
            weaponType = shoot.weaponType;

            attackSound = shoot.attackSound;
            silencedAttackSound = shoot.silencedAttackSound;
            isSilenced = shoot.isSilenced;


            reticle = shoot.reticle;

            fireRate = shoot.fireRate;
            magazineSize = shoot.magazineSize;
            zoomAmount = shoot.zoomAmount;
            critMultiplier = shoot.critMultiplier;
            currentMagazine = shoot.currentMagazine;
            accuracyModifier = shoot.accuracyModifier;
            aimingDivider = shoot.aimingDivider;
            defaultAngle = shoot.defaultAngle;
            maxSpreadAngle = shoot.maxSpreadAngle;
            maxAngleMultiplier = shoot.maxAngleMultiplier;
            reloadSpeed = shoot.reloadSpeed;
            reloadSpeedModifier = shoot.reloadSpeedModifier;
            aimSpeed = shoot.aimSpeed;
            kickBackAmount = shoot.kickBackAmount;
            horizontalKickBack = shoot.horizontalKickBack;
            hitParticle = shoot.hitParticle;
            ammoType = shoot.ammoType;
            for (int i = 0; i < attachmentPos.Length; i++)
            {
                if (shoot.attachmentPos[i] != null && attachmentPos[i] != null)
                {
                    attachmentPos[i].localPosition = shoot.attachmentPos[i].localPosition;
                    attachmentPos[i].localRotation = shoot.attachmentPos[i].localRotation;
                    attachmentPos[i].localScale = shoot.attachmentPos[i].localScale;
                }
            }
        }
    }

    void SpawnAttachments(int index)
    {
        if (attachments[index] != null && attachments[index].itemName != "N/A")
        {
            GameObject go1 = Instantiate(attachments[index].attachmentGameObject, attachmentPos[index].position, attachmentPos[index].rotation);
            go1.transform.SetParent(attachmentParent.transform);
            go1.transform.localScale = attachmentPos[index].localScale;
            MoveToLayer(go1.transform, go1.transform.parent.gameObject.layer);
        }
    }
    public void SpawnAttachments()
    {
        
        for(int index = 0; index < attachments.Length; index++)
        {
            if (attachments[index] != null && attachments[index].itemName != "N/A")
            {
                GameObject go1 = Instantiate(attachments[index].attachmentGameObject, attachmentPos[index].position, attachmentPos[index].rotation);
                go1.transform.SetParent(attachmentParent.transform);
                go1.transform.localScale = attachmentPos[index].localScale;
                MoveToLayer(go1.transform, go1.transform.parent.gameObject.layer);
            }
        }
    }
    void MoveToLayer(Transform root, int layer)
    {
        Debug.Log("Moving " + root.name + " to " + layer + " layer");
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            MoveToLayer(child, layer);
    }

    public Attachment AddAttachment(Attachment newAttachment)
    {

        Attachment attachmentToReturn = null;

        switch (newAttachment.attachmentType)
        {
            case Attachment.AttachmentType.Sights:
                if (attachments[0] == null)
                {
                     attachments[0] = newAttachment;
                    SpawnAttachments(0);
                }
                else if ( attachments[0].itemName == "N/A")
                {
                    Debug.Log("This slot cannot have an attachment");
                    attachmentToReturn = new Attachment("N/A");
                }
                break;
            case Attachment.AttachmentType.Magazine:
                if ( attachments[1] == null)
                {
                     attachments[1] = newAttachment;
                    SpawnAttachments(1);
                }

                else if ( attachments[1].itemName == "N/A")
                {
                    Debug.Log("This slot cannot have an attachment");
                    attachmentToReturn = new Attachment("N/A");
                }
                break;
            case Attachment.AttachmentType.LeftBarrel:
                if ( attachments[2] == null)
                {
                     attachments[2] = newAttachment;
                    SpawnAttachments(2);
                }
                else if ( attachments[2].itemName == "N/A")
                {
                    Debug.Log("This slot cannot have an attachment");
                    attachmentToReturn = new Attachment("N/A");
                }
                break;
            case Attachment.AttachmentType.UnderBarrel:
                if ( attachments[3] == null)
                {
                     attachments[3] = newAttachment;
                    SpawnAttachments(3);
                }
                else if ( attachments[3].itemName == "N/A")
                {
                    Debug.Log("This slot cannot have an attachment");
                    attachmentToReturn = new Attachment("N/A");
                }
                break;
            case Attachment.AttachmentType.Muzzle:
                if ( attachments[4] == null)
                {
                     attachments[4] = newAttachment;
                    SpawnAttachments(4);
                }

                else if ( attachments[4].itemName == "N/A")
                {
                    Debug.Log("This slot cannot have an attachment");
                    attachmentToReturn = new Attachment("N/A");
                }
                break;
            case Attachment.AttachmentType.Stock:
                if ( attachments[5] == null)
                {
                     attachments[5] = newAttachment;
                    SpawnAttachments(5);
                }
                else if ( attachments[5].itemName == "N/A")
                {
                    Debug.Log("This slot cannot have an attachment");
                    attachmentToReturn = new Attachment("N/A");
                }
                break;
        }

        return attachmentToReturn;
    }

   
    public bool CanAttachAttachment(Attachment attachment)
    {
        bool canAttach = false;
        switch (attachment.attachmentType)
        {
            case Attachment.AttachmentType.Sights:
                if ( attachments[0] == null)
                {
                    canAttach = true;

                }
                else if ( attachments[0].itemName == "N/A")
                {

                }
                else
                {

                }
                break;
            case Attachment.AttachmentType.Magazine:
                if ( attachments[1] == null)
                {
                    canAttach = true;
                }
                else if ( attachments[1].itemName == "N/A")
                {

                }
                else
                {

                }
                break;
            case Attachment.AttachmentType.LeftBarrel:
                if ( attachments[2] == null)
                {
                    canAttach = true;
                }
                else if ( attachments[2].itemName == "N/A")
                {

                }
                else
                {

                }
                break;
            case Attachment.AttachmentType.UnderBarrel:
                if ( attachments[3] == null)
                {
                    canAttach = true;
                }
                else if ( attachments[3].itemName == "N/A")
                {

                }
                else
                {

                }
                break;
            case Attachment.AttachmentType.Muzzle:
                if ( attachments[4] == null)
                {
                    canAttach = true;
                }
                else if ( attachments[4].itemName == "N/A")
                {

                }
                else
                {

                }
                break;
            case Attachment.AttachmentType.Stock:

                if ( attachments[5] == null)
                {
                    canAttach = true;
                }
                else if ( attachments[5].itemName == "N/A")
                {

                }
                else
                {

                }
                break;
        }

        return canAttach;
    }
    protected override void OnWeaponDisabled()
    {
        base.OnWeaponDisabled();
    }
    protected override void OnWeaponEnabled()
    {
        base.OnWeaponEnabled();
    }
}
