using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour {
    [Header("Base Properties")]
    public WeaponType weaponType;

    public bool isPickup;

    public float damage;

    public AudioClip attackSound;

    public AudioClip silencedAttackSound;

    public bool isSilenced = false;
    [HideInInspector]
    public Animator anim;
    //[HideInInspector]
    public GameObject player;

    [HideInInspector]

    public PlayerProperties playerProperties;
    [HideInInspector]
    public UpdateAmmo updateAmmo;
    [HideInInspector]
    public GameObject ammo;

    public GameObject minimapIndicator;

    [Header("Reticle Options")]
    [SerializeField]
    protected GameObject reticleGameObject;

    [HideInInspector]
    public Sprite reticle;
    
    [SerializeField]
    protected bool reticleHasBloom;
    protected GameObject reticleUIElement;
    [HideInInspector]
    public AudioSource aud;
    [HideInInspector]
    public bool weaponEnabled;
    

    public enum WeaponType
    {
        Gun, 
        Melee
    }

    // Use this for initialization
    public virtual void Start() {
        if (isPickup)
            return;
        Transform tempObject = FindParent.FindParentTransformWithTag("Player", transform);
        while (FindParent.FindParentTransformWithTag("Player", tempObject) != null)
        {
            tempObject = FindParent.FindParentTransformWithTag("Player", tempObject);
        }
        Debug.Log(FindParent.callAmount);
        player = tempObject.gameObject;
        playerProperties = player.GetComponent<PlayerProperties>();
        reticleUIElement = player.transform.Find("SinglePlayer UI/Reticle").gameObject;

        if (anim == null)
            anim = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
        updateAmmo = player.transform.Find("SinglePlayer UI/HUD/Ammo Count").GetComponent<UpdateAmmo>();   
        
        if(reticleGameObject && reticleGameObject.GetComponent<SpriteRenderer>())
        {
            Debug.Log("There is a reticle gameObject attached to this weapon");

            reticle = reticleGameObject.GetComponent<SpriteRenderer>().sprite;
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    protected virtual void UpdateReticle()
    {
        //Debug.Log("Calling Weapon UpdateReticle");
        
        if(!weaponEnabled)
        {
            reticleUIElement.GetComponent<Image>().sprite = null;
            if(reticleUIElement.GetComponent<CanvasGroup>())
            {
                reticleUIElement.GetComponent<CanvasGroup>().alpha = 0;
            }
            else
            {
                reticleUIElement.AddComponent<CanvasGroup>();
            }
        }
        else if(!reticleHasBloom)
        {
            reticleUIElement.GetComponent<Image>().sprite = reticle;
            if (reticleUIElement.GetComponent<CanvasGroup>())
            {
                reticleUIElement.GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                reticleUIElement.AddComponent<CanvasGroup>();
            }
        }
        else
        {
            reticleUIElement.GetComponent<Image>().sprite = reticle;
            if (reticleUIElement.GetComponent<CanvasGroup>())
            {
                reticleUIElement.GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                reticleUIElement.AddComponent<CanvasGroup>();
            }
        }

    }
    
    public virtual void CopyFrom(Weapon weapon)
    {
        minimapIndicator = weapon.minimapIndicator;
        weaponType = weapon.weaponType;
        reticle = weapon.reticle;
        damage = weapon.damage;
        attackSound = weapon.attackSound;
        silencedAttackSound = weapon.silencedAttackSound;
        isSilenced = weapon.isSilenced;
        reticleGameObject = weapon.reticleGameObject;
        reticleHasBloom = weapon.reticleHasBloom;

    }
    public void EnableWeapon()
    {
        //Debug.Log("Calling Enable weapon, and weaponEnabled = " + weaponEnabled);
        if (weaponEnabled == false)
        {
            OnWeaponEnabled();
        }
        weaponEnabled = true;

    }
    public void DisableWeapon()
    {
        //Debug.Log("Calling disable weapon, and weaponEnabled = " + weaponEnabled);
        if (weaponEnabled == true)
        {
            OnWeaponDisabled();
        }
        weaponEnabled = false;
    }
    public void DisableRenderers()
    {
        //Debug.Log("renderers disabled");
        GameObject weapon = gameObject;
        Renderer[] renderer = weapon.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderer)
        {
            r.enabled = false;
        }
    }
    public void EnableRenderers()
    {

    }
    protected virtual void OnWeaponEnabled()
    {
        return;
    }
    protected virtual void OnWeaponDisabled()
    {
        return;
    }
}
