using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAmmo : MonoBehaviour {
    [SerializeField]
    Image[] ammoImage;
    [SerializeField]
    Text[] amountText;

    PlayerAmmo ammo;
    WeaponSwitch weaponSwitch;
	// Use this for initialization
	void Start () {
		if(gameObject.name != "Ammo Count Panel")
        {
            Debug.LogWarning("Make sure this is attached to a panel with ammo dislpay UI objects in it!");
        }
        Image[] tempImages = GetComponentsInChildren<Image>();
        ammoImage = new Image[tempImages.Length - 1];
        for(int i = 1; i < tempImages.Length; i++)
        {
            ammoImage[i - 1] = tempImages[i];
        }

        amountText = GetComponentsInChildren<Text>();
        try
        {
            ammo = transform.root.GetComponent<PlayerAmmo>();
        }
        catch (MissingComponentException e)
        {
            Debug.LogError("Missing a player ammo in the root transform!!");
        }
        weaponSwitch = transform.root.GetComponent<WeaponSwitch>();
        
	}
	
	// Update is called once per frame
	void Update () {
        Shoot tempShoot = null;
        if(weaponSwitch.equippedWeapon != null)
        {
            if(weaponSwitch.equippedWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Gun)
            {
                tempShoot = weaponSwitch.equippedWeapon.GetComponent<Shoot>();
            }
        }
		for(int i = 0; i < 6; i++)
        {
            int tempAmmo = ammo.GetAmount((AmmoEnum.AmmoType)i);
            if(tempAmmo < 1)
            {
                Color c = ammoImage[i].color;
                c.a = .5f;
                ammoImage[i].color = c;
                amountText[i].text = tempAmmo.ToString();
                amountText[i].color = Color.red;
            }
            else
            {
                Color c = ammoImage[i].color;
                c.a = 1f;
                ammoImage[i].color = c;
                amountText[i].text = tempAmmo.ToString();
                amountText[i].color = Color.black;
            }
            if(tempShoot != null && tempShoot.ammoType == (AmmoEnum.AmmoType)i)
            {
                ammoImage[i].color = Color.blue;
            }
            else
            {
                ammoImage[i].color = Color.white;
            }
        }
	}
}
